namespace StudyHub.Common.Models;

public class GoogleAuthConfig
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public TimeSpan IssuedAtClockTolerance { get; set; }
    public TimeSpan ExpirationTimeClockTolerance { get; set; }
}
