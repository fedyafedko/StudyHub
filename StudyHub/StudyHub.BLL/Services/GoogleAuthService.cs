using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth;
using StudyHub.BLL.Services.Interfaces;
using StudyHub.Common.DTO.AuthDTO;
using StudyHub.Common.Exceptions;
using StudyHub.Entities;
using Microsoft.AspNetCore.Identity;
using StudyHub.Common.Models;
using Microsoft.Extensions.Options;

namespace StudyHub.BLL.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly GoogleAuthConfig _googleConfig;

    public GoogleAuthService(
        UserManager<User> userManager,
        ITokenService tokenService,
        IOptions<GoogleAuthConfig> googleConfig)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _googleConfig = googleConfig.Value;
    }
    public async Task<AuthSuccessDTO> GoogleLogin(string authorizationCode)
    {
        var paylod = await GetUserInfoAsync(authorizationCode);

        var user = await _userManager.FindByEmailAsync(paylod.Email);

        if (user == null)
            throw new NotFoundException($"Unable to find user by specified email. Email: {user!.Email}");

        return new AuthSuccessDTO(_tokenService.GenerateJwtToken(user, (await _userManager.GetRolesAsync(user)).ToArray()),
            _tokenService.GenerateRefreshTokenAsync(user));
    }

    private async Task<GoogleJsonWebSignature.Payload> GetUserInfoAsync(string authorizationCode)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _googleConfig.ClientId,
                ClientSecret = _googleConfig.ClientSecret
            }
        });

        var tokenResponse = await flow.ExchangeCodeForTokenAsync(
            string.Empty,
            authorizationCode,
            _googleConfig.RedirectUri,
            CancellationToken.None);

        var setting = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new List<string> { _googleConfig.ClientId },
            IssuedAtClockTolerance = _googleConfig.IssuedAtClockTolerance,
            ExpirationTimeClockTolerance = _googleConfig.ExpirationTimeClockTolerance,
        };

        var paylod = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken, setting);
        return paylod;
    }
}
