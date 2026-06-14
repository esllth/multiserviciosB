using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using MultiservicioB.Services;

namespace MultiservicioB.Areas.Identity.Pages.Account
{
    [EnableRateLimiting("authentication")]
    public class AdminEmailVerificationModel : PageModel
    {
        private readonly ILoginSecurityService _loginSecurity;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AdminEmailVerificationModel> _logger;

        public AdminEmailVerificationModel(
            ILoginSecurityService loginSecurity,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AdminEmailVerificationModel> logger)
        {
            _loginSecurity = loginSecurity;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        [Required]
        public string ChallengeId { get; set; } = "";

        public IActionResult OnGet() =>
            string.IsNullOrWhiteSpace(ChallengeId) ? RedirectToPage("./Login") : Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = _loginSecurity.ValidateAdminEmailChallenge(ChallengeId, Code);
            var user = userId == null ? null : await _userManager.FindByIdAsync(userId);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Administrador"))
            {
                _logger.LogWarning("Código administrativo inválido");
                ModelState.AddModelError("", "El código es inválido o expiró.");
                return Page();
            }

            await _signInManager.SignInAsync(user, false);
            _loginSecurity.RecordSuccess(
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                user.Email ?? "");
            return RedirectToAction("Dashboard", "Home");
        }
    }
}
