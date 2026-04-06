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
                    // 1. Verificar admin existente
                    // ============================
                    var admins = await _userManager.GetUsersInRoleAsync("Administrador");

                    // ============================
                    // 2. Validar dominio
                    // ============================
                    bool isCompanyEmail = user.Email != null &&
                        user.Email.EndsWith("@multiserviciosb.com");

                    // ============================
                    // 3. Buscar empleado en BD
                    // ============================
                    var empleado = await _context.Empleados
                        .FirstOrDefaultAsync(e => e.CorreoElectronicoEmpleado == user.Email);

                    // ============================
                    // 4. LÓGICA DE ROLES
                    // ============================
                    if (!admins.Any() && isCompanyEmail)
                    {
                        await _userManager.AddToRoleAsync(user, "Administrador");
                    }
                    else if (isCompanyEmail && empleado != null && !empleado.TieneUsuario)
                    {
                        await _userManager.AddToRoleAsync(user, "Empleado");

                        empleado.TieneUsuario = true;
                        empleado.UserId = user.Id;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Cliente");
                    }

                    // ============================
                    // LOGIN AUTOMÁTICO
                    // ============================
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}