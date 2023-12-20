using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudyHub.BLL.Services.Interfaces;
using StudyHub.Common.Exceptions;
using StudyHub.Common.Models;
using StudyHub.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudyHub.BLL.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly TokenValidationParameters _tokenValidationParametrs;

    public TokenService(IOptions<JwtSettings> settings, TokenValidationParameters tokenValidationParametrs)
    {
        _settings = settings.Value;
        _tokenValidationParametrs = tokenValidationParametrs;
    }
    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var validationParametrs = _tokenValidationParametrs.Clone();
        validationParametrs.ValidateLifetime = false;
        try
        {
            var principal = jwtTokenHandler.ValidateToken(token, validationParametrs, out var validatedToken);

            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                throw new InvalidSecurityAlgorithmException("Current token does not have right security algorithm");

            return principal;
        }
        catch
        {
            throw new TokenValidatorException("Something went wrong with token validator");
        }
    }

    public bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return validatedToken is JwtSecurityToken jwtSecurityToken &&
            jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
    }

    public string GenerateJwtToken(User user, string[] roles)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_settings.Secret);
        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_settings.AccessTokenLifeTime),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return jwtToken;
    }
    public string GenerateRefreshTokenAsync(User user)
    {
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.Now.Add(_settings.RefreshTokenLifeTime),
            Used = true,
            Invalidated = false,
        };

        user.RefreshToken = refreshToken;

        return user.RefreshToken.Token;
    }
}
