using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MultiservicioB.Services;

namespace MultiservicioB.Tests.Unit.Services;

public sealed class LoginSecurityServiceTests
{
    [Fact]
    public void RecordFailure_TracksAttemptsAndRaisesCaptchaRisk()
    {
        var service = CreateService(captchaAfterFailedAttempts: 2);

        service.RecordFailure("192.0.2.10", " usuario@ejemplo.com ");
        var firstRisk = service.GetRisk("192.0.2.10", "USUARIO@EJEMPLO.COM");
        service.RecordFailure("192.0.2.10", "usuario@ejemplo.com");
        var secondRisk = service.GetRisk("192.0.2.10", "usuario@ejemplo.com");

        Assert.Equal(1, firstRisk.FailedAttempts);
        Assert.False(firstRisk.RequiresCaptcha);
        Assert.Equal(2, secondRisk.FailedAttempts);
        Assert.True(secondRisk.RequiresCaptcha);
        Assert.Equal(1, secondRisk.DelaySeconds);
    }

    [Fact]
    public void RecordSuccess_ClearsAttemptsForClientAndEmail()
    {
        var service = CreateService();
        service.RecordFailure("192.0.2.10", "usuario@ejemplo.com");

        service.RecordSuccess("192.0.2.10", "USUARIO@EJEMPLO.COM");

        var risk = service.GetRisk("192.0.2.10", "usuario@ejemplo.com");
        Assert.Equal(0, risk.FailedAttempts);
        Assert.False(risk.RequiresCaptcha);
    }

    [Fact]
    public void ValidateCaptcha_AcceptsCorrectAnswerAndRejectsInvalidTokens()
    {
        var service = CreateService();
        var challenge = service.CreateCaptcha();
        var match = Regex.Match(challenge.Question, @"(?<left>\d+)\s*\+\s*(?<right>\d+)");
        Assert.True(match.Success);

        var answer = int.Parse(match.Groups["left"].Value, CultureInfo.InvariantCulture)
            + int.Parse(match.Groups["right"].Value, CultureInfo.InvariantCulture);

        Assert.True(service.ValidateCaptcha(challenge.Token, answer.ToString(CultureInfo.InvariantCulture)));
        Assert.False(service.ValidateCaptcha(challenge.Token, (answer + 1).ToString(CultureInfo.InvariantCulture)));
        Assert.False(service.ValidateCaptcha($"{challenge.Token}invalid", answer.ToString(CultureInfo.InvariantCulture)));
        Assert.False(service.ValidateCaptcha(challenge.Token, ""));
    }

    [Fact]
    public void ValidateAdminEmailChallenge_IsOneTimeAndDoesNotConsumeAnIncorrectAttempt()
    {
        var service = CreateService();
        var challenge = service.CreateAdminEmailChallenge("user-123");

        Assert.Null(service.ValidateAdminEmailChallenge(challenge.ChallengeId, "000000"));
        Assert.Equal("user-123", service.ValidateAdminEmailChallenge(challenge.ChallengeId, challenge.Code));
        Assert.Null(service.ValidateAdminEmailChallenge(challenge.ChallengeId, challenge.Code));
    }

    private static LoginSecurityService CreateService(int captchaAfterFailedAttempts = 3)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var protector = new EphemeralDataProtectionProvider();
        var options = Options.Create(new AuthenticationSecurityOptions
        {
            CaptchaAfterFailedAttempts = captchaAfterFailedAttempts,
            MaximumDelaySeconds = 5,
            AttemptWindowMinutes = 15
        });

        return new LoginSecurityService(cache, protector, options);
    }
}
