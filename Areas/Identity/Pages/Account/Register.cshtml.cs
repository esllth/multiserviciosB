using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiservicioB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MultiservicioB.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var user = new IdentityUser
            {
                UserName = Input.Email,
                Email = Input.Email
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            // ============================
            // NORMALIZACIÓN
            // ============================
            string email = user.Email!.ToLower();
            bool isCompanyEmail = email.EndsWith("@multiserviciosb.com");

            // ============================
            // CONSULTAS
            // ============================
            bool adminExists = (await _userManager.GetUsersInRoleAsync("Administrador")).Any();

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.CorreoElectronicoEmpleado.ToLower() == email);

            // ============================
            // VALIDACIONES DE NEGOCIO
            // ============================

            // 🔒 Caso: intenta registrarse como empleado pero no existe en BD
            if (isCompanyEmail && empleado == null && adminExists)
            {
                ModelState.AddModelError("", "No estás autorizado como empleado.");
                return Page();
            }

            // 🔒 Caso: empleado ya registrado
            if (empleado != null && empleado.TieneUsuario)
            {
                ModelState.AddModelError("", "Este empleado ya tiene una cuenta.");
                return Page();
            }

            // ============================
            // ASIGNACIÓN DE ROLES
            // ============================

            if (!adminExists && isCompanyEmail)
            {
                // 🔥 PRIMER ADMIN DEL SISTEMA
                await _userManager.AddToRoleAsync(user, "Administrador");
            }
            else if (isCompanyEmail && empleado != null)
            {
                // 🔥 EMPLEADO VÁLIDO
                await _userManager.AddToRoleAsync(user, "Empleado");

                empleado.TieneUsuario = true;
                empleado.UserId = user.Id;
                empleado.EstadoEmpleado = "Activo";

                _context.Update(empleado);
                await _context.SaveChangesAsync();
            }
            else
            {
                // 🔥 CLIENTE NORMAL
                await _userManager.AddToRoleAsync(user, "Cliente");
            }

            // ============================
            // LOGIN AUTOMÁTICO
            // ============================
            await _signInManager.SignInAsync(user, isPersistent: false);

            return LocalRedirect(returnUrl);
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}

