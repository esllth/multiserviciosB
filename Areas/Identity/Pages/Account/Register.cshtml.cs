using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiservicioB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
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

            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = Input.Email,
                    Email = Input.Email
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // ============================
                    // NORMALIZACIÓN
                    // ============================
                    string email = user.Email!.ToLower();
                    bool isCompanyEmail = email.EndsWith("@multiserviciosb.com");

                    // ============================
                    // CONSULTAS (EFICIENTES)
                    // ============================
                    var adminExists = (await _userManager.GetUsersInRoleAsync("Administrador")).Any();

                    var empleado = await _context.Empleados
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.CorreoElectronicoEmpleado.ToLower() == email);

                    // ============================
                    // VALIDACIONES DE SEGURIDAD
                    // ============================

                    // 🔒 Si intenta registrarse como empleado pero NO existe en BD → bloquear
                    if (isCompanyEmail && empleado == null && adminExists)
                    {
                        ModelState.AddModelError("", "No estás autorizado como empleado.");
                        return Page();
                    }

                    // ============================
                    // ASIGNACIÓN DE ROLES
                    // ============================

                    if (!adminExists && isCompanyEmail)
                    {
                        // 🔥 PRIMER ADMIN
                        await _userManager.AddToRoleAsync(user, "Administrador");
                    }
                    else if (isCompanyEmail && empleado != null && !empleado.TieneUsuario)
                    {
                        // 🔥 EMPLEADO VÁLIDO
                        await _userManager.AddToRoleAsync(user, "Empleado");

                        // actualizar empleado
                        empleado.TieneUsuario = true;
                        empleado.UserId = user.Id;

                        _context.Update(empleado);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // 🔥 CLIENTE
                        await _userManager.AddToRoleAsync(user, "Cliente");
                    }

                    // ============================
                    // LOGIN
                    // ============================
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}