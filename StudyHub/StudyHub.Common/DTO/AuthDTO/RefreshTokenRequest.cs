namespace StudyHub.Common.DTO.AuthDTO;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
