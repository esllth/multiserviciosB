namespace MultiservicioB.Services
{
    public class AuthenticationSecurityOptions
    {
        public const string SectionName = "AuthenticationSecurity";

        public int CaptchaAfterFailedAttempts { get; set; } = 3;
        public int MaximumDelaySeconds { get; set; } = 5;
        public int AttemptWindowMinutes { get; set; } = 15;
        public bool RequireEmailCodeForAdministrators { get; set; } = true;
    }
}
