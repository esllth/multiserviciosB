using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using MultiservicioB.Models;

namespace MultiservicioB.Areas.Identity.Pages.Account
{
    [EnableRateLimiting("authentication")]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILoginSecurityService _loginSecurity;
        private readonly IEmailSender _emailSender;
        private readonly SmtpOptions _smtpOptions;
        private readonly AuthenticationSecurityOptions _securityOptions;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            ILoginSecurityService loginSecurity,
            IEmailSender emailSender,
            IOptions<SmtpOptions> smtpOptions,
            IOptions<AuthenticationSecurityOptions> securityOptions,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _loginSecurity = loginSecurity;
            _emailSender = emailSender;
            _smtpOptions = smtpOptions.Value;
            _securityOptions = securityOptions.Value;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "El correo electrónico es requerido")]
            [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "La contraseña es requerida")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = "";

            public string CaptchaAnswer { get; set; } = "";
            public string CaptchaToken { get; set; } = "";
        }

        public bool ShowCaptcha { get; private set; }
        public string CaptchaQuestion { get; private set; } = "";

        public void OnGet()
        {
            PrepareCaptchaIfRequired("", GetClientKey());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PrepareCaptchaIfRequired(Input.Email, GetClientKey());
                return Page();
            }

            string email = Input.Email.Trim().ToLower();
            var clientKey = GetClientKey();
            var risk = _loginSecurity.GetRisk(clientKey, email);
            await _loginSecurity.DelayAsync(risk, HttpContext.RequestAborted);

            if (risk.RequiresCaptcha &&
                !_loginSecurity.ValidateCaptcha(Input.CaptchaToken, Input.CaptchaAnswer))
            {
                _loginSecurity.RecordFailure(clientKey, email);
                ModelState.AddModelError("", "Credenciales inválidas");
                PrepareCaptchaIfRequired(email, clientKey);
                return Page();
            }

            // 1. Buscar si el usuario existe en la seguridad de Identity
            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                _loginSecurity.RecordFailure(clientKey, email);
                _logger.LogWarning("Inicio de sesión fallido desde {ClientKey}", clientKey);
                ModelState.AddModelError("", "Credenciales inválidas");
                PrepareCaptchaIfRequired(email, clientKey);
                return Page();
            }

            // ==========================================================================
            // FILTRO DE SEGURIDAD INTERNO [RMGUS-005]
            // ==========================================================================

            // Verificamos de primero si el usuario tiene el rol de Administrador
            bool esAdmin = await _userManager.IsInRoleAsync(user, "Administrador");
            bool esCliente = await _userManager.IsInRoleAsync(user, "Cliente");

            if (esCliente)
            {
                var estadoCliente = await _context.Clientes
                    .Where(c => c.Correo != null && c.Correo.ToLower() == email)
                    .Select(c => c.Estado)
                    .FirstOrDefaultAsync();
                if (estadoCliente != null &&
                    !estadoCliente.Equals("Activo", StringComparison.OrdinalIgnoreCase))
                {
                    _loginSecurity.RecordFailure(clientKey, email);
                    ModelState.AddModelError("", "La cuenta se encuentra desactivada.");
                    PrepareCaptchaIfRequired(email, clientKey);
                    return Page();
                }
            }

            // REGLA SUPREMA: Si es ADMIN, se salta olímpicamente cualquier validación de empleado
            if (!esAdmin)
            {
                // Solo si NO es admin y usa correo de la empresa, validamos como empleado técnico
                if (email.EndsWith("@multiserviciosb.com"))
                {
                    var empleado = await _context.Empleados
                        .FirstOrDefaultAsync(e => e.CorreoElectronicoEmpleado.ToLower() == email);

                    // Si no está pre-cargado en la tabla o fue desactivado por gerencia
                    if (empleado == null || !EstadosEmpleado.PuedeAcceder(empleado))
                    {
                        _loginSecurity.RecordFailure(clientKey, email);
                        _logger.LogWarning(
                            "Acceso rechazado para empleado inactivo {UserId} desde {ClientKey}",
                            user.Id,
                            clientKey);
                        ModelState.AddModelError("", "Credenciales inválidas");
                        PrepareCaptchaIfRequired(email, clientKey);
                        return Page();
                    }

                }
            }

            // 2. Verificar la contraseña si superó los filtros correspondientes
            var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, false);

            if (!result.Succeeded)
            {
                _loginSecurity.RecordFailure(clientKey, email);
                _logger.LogWarning("Contraseña incorrecta para {UserId} desde {ClientKey}", user.Id, clientKey);
                ModelState.AddModelError("", "Credenciales inválidas");
                PrepareCaptchaIfRequired(email, clientKey);
                return Page();
            }

            if (esAdmin &&
                _securityOptions.RequireEmailCodeForAdministrators &&
                IsSmtpConfigured())
            {
                var challenge = _loginSecurity.CreateAdminEmailChallenge(user.Id);
                try
                {
                    await _emailSender.SendEmailAsync(
                        user.Email!,
                        "Código de acceso administrativo",
                        $"Su código de acceso es <strong>{challenge.Code}</strong>. Expira en 10 minutos.");
                    _logger.LogInformation("Código de acceso administrativo enviado para {UserId}", user.Id);
                    return RedirectToPage("./AdminEmailVerification", new { challengeId = challenge.ChallengeId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "No se pudo enviar el código administrativo para {UserId}", user.Id);
                    ModelState.AddModelError(
                        "",
                        "No se pudo completar la verificación de seguridad. Intente nuevamente.");
                    PrepareCaptchaIfRequired(email, clientKey);
                    return Page();
                }
            }

            if (esAdmin && _securityOptions.RequireEmailCodeForAdministrators)
            {
                _logger.LogWarning(
                    "La verificación administrativa por correo no se aplicó porque SMTP no está configurado.");
            }

            await _signInManager.SignInAsync(user, false);
            _loginSecurity.RecordSuccess(clientKey, email);

            return RedirectToAction("Dashboard", "Home");
        }

        private void PrepareCaptchaIfRequired(string email, string clientKey)
        {
            if (!_loginSecurity.GetRisk(clientKey, email).RequiresCaptcha)
            {
                return;
            }

            var challenge = _loginSecurity.CreateCaptcha();
            ShowCaptcha = true;
            CaptchaQuestion = challenge.Question;
            Input.CaptchaToken = challenge.Token;
        }

        private string GetClientKey() =>
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        private bool IsSmtpConfigured() =>
            !string.IsNullOrWhiteSpace(_smtpOptions.Host) &&
            !string.IsNullOrWhiteSpace(_smtpOptions.FromEmail);
    }
}
