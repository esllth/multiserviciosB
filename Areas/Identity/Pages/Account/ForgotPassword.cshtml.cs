// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using MultiservicioB.Services;

namespace MultiservicioB.Areas.Identity.Pages.Account
{
    [EnableRateLimiting("authentication")]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SmtpOptions _smtpOptions;
        private readonly ILogger<ForgotPasswordModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public ForgotPasswordModel(
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            IOptions<SmtpOptions> smtpOptions,
            ILogger<ForgotPasswordModel> logger,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _smtpOptions = smtpOptions.Value;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var email = Input.Email.Trim();
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || !await _userManager.HasPasswordAsync(user))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                if (string.IsNullOrWhiteSpace(_smtpOptions.Host) ||
                    string.IsNullOrWhiteSpace(_smtpOptions.FromEmail))
                {
                    _logger.LogError(
                        "Se solicitó recuperación de contraseña, pero SMTP no está configurado.");
                    if (_environment.IsDevelopment())
                    {
                        TempData["DevPasswordResetLink"] = callbackUrl;
                    }

                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                try
                {
                    await _emailSender.SendEmailAsync(
                        email,
                        "Recuperación de contraseña - Multiservicios Bolívar",
                        $"""
                        <p>Hola,</p>
                        <p>Recibimos una solicitud para restablecer la contraseña de su cuenta en Multiservicios Bolívar.</p>
                        <p><a href="{HtmlEncoder.Default.Encode(callbackUrl)}">Restablecer contraseña</a></p>
                        <p>Este enlace expira en 30 minutos. Si usted no solicitó este cambio, puede ignorar este correo.</p>
                        """);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "No se pudo enviar el correo de recuperación.");
                }

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
