using Microsoft.IdentityModel.Tokens;
using StudyHub.Entities;
using System.Security.Claims;

namespace StudyHub.BLL.Services.Interfaces;
public interface ITokenService
{
    string GenerateJwtToken(User user, string[] roles);
    string GenerateRefreshTokenAsync(User user);
    bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken);
    ClaimsPrincipal GetPrincipalFromToken(string token);
}
