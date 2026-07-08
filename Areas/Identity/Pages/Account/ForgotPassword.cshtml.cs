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
        private readonly IConfiguration _configuration;

        public ForgotPasswordModel(
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            IOptions<SmtpOptions> smtpOptions,
            ILogger<ForgotPasswordModel> logger,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _smtpOptions = smtpOptions.Value;
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
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
                var callbackPath = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code });
                var publicBaseUrl = GetPublicBaseUrl();
                var callbackUrl = BuildAbsoluteUrl(publicBaseUrl, callbackPath);

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
                    var logoUrl = BuildAbsoluteUrl(publicBaseUrl, Url.Content("~/images/Logo/logo.png"));

                    await _emailSender.SendEmailAsync(
                        email,
                        "Restablecimiento de contraseña - Multiservicio Bolívar",
                        BuildPasswordResetEmail(callbackUrl, logoUrl));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "No se pudo enviar el correo de recuperación.");
                }

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }

        private string GetPublicBaseUrl()
        {
            var configuredBaseUrl =
                _configuration["APP_BASE_URL"] ??
                _configuration["PUBLIC_BASE_URL"] ??
                _configuration["App:PublicBaseUrl"];

            if (!string.IsNullOrWhiteSpace(configuredBaseUrl))
            {
                return configuredBaseUrl.TrimEnd('/');
            }

            return $"{Request.Scheme}://{Request.Host}";
        }

        private static string BuildAbsoluteUrl(string baseUrl, string relativeUrl)
        {
            return $"{baseUrl.TrimEnd('/')}/{relativeUrl.TrimStart('/')}";
        }

        private static string BuildPasswordResetEmail(string callbackUrl, string logoUrl)
        {
            var encodedCallbackUrl = HtmlEncoder.Default.Encode(callbackUrl);
            var encodedLogoUrl = HtmlEncoder.Default.Encode(logoUrl);

            return $$"""
            <!DOCTYPE html>
            <html lang="es">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Restablecimiento de Contraseña</title>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }

                    .container {
                        background-color: #ffffff;
                        margin: 0 auto;
                        padding: 20px;
                        max-width: 600px;
                        border-radius: 8px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }

                    h1 {
                        color: #333333;
                        font-size: 24px;
                        line-height: 1.2;
                        margin: 0;
                    }

                    p {
                        color: #555555;
                    }

                    .reset-box {
                        margin: 20px 0;
                        padding: 15px;
                        background-color: #f0f0f0;
                        border-left: 4px solid #4CAF50;
                        font-size: 18px;
                        font-weight: bold;
                        text-align: center;
                    }

                    .reset-link {
                        color: #2e7d32;
                        text-decoration: none;
                    }

                    .footer {
                        margin-top: 30px;
                        color: #777777;
                        font-size: 12px;
                        text-align: center;
                    }

                    .header-table {
                        width: 100%;
                        margin-bottom: 20px;
                    }

                    .header-table td {
                        vertical-align: middle;
                    }

                    .header-logo {
                        text-align: right;
                        width: 110px;
                    }

                    .header-logo img {
                        display: block;
                        width: 88px;
                        max-width: 88px;
                        height: auto;
                        margin-left: auto;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <table class="header-table">
                        <tr>
                            <td>
                                <h1>Restablecimiento de Contraseña</h1>
                            </td>
                            <td class="header-logo">
                                <img src="{{encodedLogoUrl}}" alt="Logo Multiservicio Bolívar">
                            </td>
                        </tr>
                    </table>

                    <p>Hola,</p>
                    <p>Recibimos una solicitud para restablecer la contraseña de tu cuenta en <strong>Multiservicio Bolívar</strong>.</p>
                    <p>Usa el siguiente enlace para crear una nueva contraseña:</p>

                    <div class="reset-box">
                        <a class="reset-link" href="{{encodedCallbackUrl}}">Restablecer contraseña</a>
                    </div>

                    <p>Este enlace expira en 30 minutos.</p>
                    <p><b>Si no solicitaste este cambio</b>, puedes ignorar este correo o contactar a nuestro soporte.</p>
                    <p>Gracias,</p>
                    <p>El equipo de Multiservicio Bolívar</p>

                    <div class="footer">
                        <p>Este correo fue enviado automáticamente. Por favor, no respondas a este mensaje.</p>
                        <p>&copy; {{DateTime.UtcNow.Year}} Multiservicio Bolívar. Todos los derechos reservados.</p>
                    </div>
                </div>
            </body>
            </html>
            """;
        }
    }
}
