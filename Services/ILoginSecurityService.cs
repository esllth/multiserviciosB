namespace MultiservicioB.Services
{
    public interface ILoginSecurityService
    {
        LoginRisk GetRisk(string clientKey, string email);
        Task DelayAsync(LoginRisk risk, CancellationToken cancellationToken);
        void RecordFailure(string clientKey, string email);
        void RecordSuccess(string clientKey, string email);
        CaptchaChallenge CreateCaptcha();
        bool ValidateCaptcha(string? token, string? answer);
        AdminEmailChallenge CreateAdminEmailChallenge(string userId);
        string? ValidateAdminEmailChallenge(string challengeId, string code);
    }

    public record LoginRisk(int FailedAttempts, bool RequiresCaptcha, int DelaySeconds);
    public record CaptchaChallenge(string Question, string Token);
    public record AdminEmailChallenge(string ChallengeId, string Code);
}
