using System.Globalization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace MultiservicioB.Services
{
    public class LoginSecurityService : ILoginSecurityService
    {
        private const string CaptchaPurpose = "MultiservicioB.LoginCaptcha.v1";
        private readonly IMemoryCache _cache;
        private readonly IDataProtector _protector;
        private readonly AuthenticationSecurityOptions _options;

        public LoginSecurityService(
            IMemoryCache cache,
            IDataProtectionProvider dataProtectionProvider,
            IOptions<AuthenticationSecurityOptions> options)
        {
            _cache = cache;
            _protector = dataProtectionProvider.CreateProtector(CaptchaPurpose);
            _options = options.Value;
        }

        public LoginRisk GetRisk(string clientKey, string email)
        {
            var failures = Math.Max(GetFailures($"ip:{clientKey}"), GetFailures($"email:{Normalize(email)}"));
            var delay = failures <= 1 ? 0 : Math.Min(_options.MaximumDelaySeconds, 1 << Math.Min(failures - 2, 3));
            return new LoginRisk(
                failures,
                failures >= _options.CaptchaAfterFailedAttempts,
                delay);
        }

        public Task DelayAsync(LoginRisk risk, CancellationToken cancellationToken) =>
            risk.DelaySeconds > 0
                ? Task.Delay(TimeSpan.FromSeconds(risk.DelaySeconds), cancellationToken)
                : Task.CompletedTask;

        public void RecordFailure(string clientKey, string email)
        {
            Increment($"ip:{clientKey}");
            Increment($"email:{Normalize(email)}");
        }

        public void RecordSuccess(string clientKey, string email)
        {
            _cache.Remove($"ip:{clientKey}");
            _cache.Remove($"email:{Normalize(email)}");
        }

        public CaptchaChallenge CreateCaptcha()
        {
            var left = Random.Shared.Next(2, 10);
            var right = Random.Shared.Next(1, 10);
            var expires = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
            var payload = string.Create(
                CultureInfo.InvariantCulture,
                $"{left + right}:{expires}");
            return new CaptchaChallenge($"¿Cuánto es {left} + {right}?", _protector.Protect(payload));
        }

        public bool ValidateCaptcha(string? token, string? answer)
        {
            if (string.IsNullOrWhiteSpace(token) ||
                !int.TryParse(answer, NumberStyles.Integer, CultureInfo.InvariantCulture, out var supplied))
            {
                return false;
            }

            try
            {
                var parts = _protector.Unprotect(token).Split(':');
                return parts.Length == 2 &&
                       int.TryParse(parts[0], out var expected) &&
                       long.TryParse(parts[1], out var expires) &&
                       DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= expires &&
                       supplied == expected;
            }
            catch
            {
                return false;
            }
        }

        public AdminEmailChallenge CreateAdminEmailChallenge(string userId)
        {
            var challengeId = Convert.ToHexString(RandomNumberGenerator.GetBytes(24));
            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString(CultureInfo.InvariantCulture);
            var codeHash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(code)));
            _cache.Set(
                $"admin-email:{challengeId}",
                $"{userId}:{codeHash}",
                TimeSpan.FromMinutes(10));
            return new AdminEmailChallenge(challengeId, code);
        }

        public string? ValidateAdminEmailChallenge(string challengeId, string code)
        {
            if (!_cache.TryGetValue<string>($"admin-email:{challengeId}", out var stored) ||
                string.IsNullOrWhiteSpace(stored))
            {
                return null;
            }

            var separator = stored.IndexOf(':');
            if (separator <= 0)
            {
                return null;
            }

            var userId = stored[..separator];
            var expectedHash = stored[(separator + 1)..];
            var suppliedHash = Convert.ToHexString(
                SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(code.Trim())));

            if (!CryptographicOperations.FixedTimeEquals(
                    Convert.FromHexString(expectedHash),
                    Convert.FromHexString(suppliedHash)))
            {
                return null;
            }

            _cache.Remove($"admin-email:{challengeId}");
            return userId;
        }

        private int GetFailures(string key) => _cache.TryGetValue<int>(key, out var value) ? value : 0;

        private void Increment(string key)
        {
            var value = GetFailures(key) + 1;
            _cache.Set(
                key,
                value,
                TimeSpan.FromMinutes(Math.Max(1, _options.AttemptWindowMinutes)));
        }

        private static string Normalize(string value) => value.Trim().ToLowerInvariant();
    }
}
