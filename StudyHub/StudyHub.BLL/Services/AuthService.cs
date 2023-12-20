using Microsoft.AspNetCore.Identity;
using StudyHub.BLL.Services.Interfaces;
using StudyHub.Common.DTO.AuthDTO;
using StudyHub.Common.Exceptions;
using StudyHub.DAL.Repositories.Interfaces;
using StudyHub.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace StudyHub.BLL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRepository<InvitedUser> _invitedUserRepository;
    private readonly IRepository<Teacher> _teacherRepository;
    private readonly IRepository<Student> _studentRepository;

    public AuthService(
        UserManager<User> userManager,
        ITokenService tokenService,
        IRepository<InvitedUser> invitedUserRepository,
        IRepository<Teacher> teacherRepository,
        IRepository<Student> studentRepository)
    {
        _invitedUserRepository = invitedUserRepository;
        _userManager = userManager;
        _tokenService = tokenService;
        _teacherRepository = teacherRepository;
        _studentRepository = studentRepository;
    }

    public async Task<AuthSuccessDTO> LoginAsync(LoginUserDTO user)
    {
        var existingUser = await _userManager.FindByEmailAsync(user.Email);

        if (existingUser == null)
            throw new NotFoundException($"Unable to find user by specified email. Email: {user.Email}");

        var isPasswordValid = await _userManager.CheckPasswordAsync(existingUser, user.Password);

        if (!isPasswordValid)
            throw new InvalidCredentialsException($"User input incorrect password. Password: {user.Password}");

        return new AuthSuccessDTO(_tokenService.GenerateJwtToken(existingUser, (await _userManager.GetRolesAsync(existingUser)).ToArray()),
            _tokenService.GenerateRefreshTokenAsync(existingUser));
    }

    public async Task<AuthSuccessDTO> RegisterAsync(RegisterUserDTO user)
    {
        var invitedUser = _invitedUserRepository.FirstOrDefault(e => e.Email == user.Email);
        if (invitedUser == null)
            throw new NotFoundException($"You doesn't invited by this email:{user.Email}");

        if(invitedUser.Token != user.Token)
            throw new IncorrectParametersException("Token does not exist");

        var existingUser = await _userManager.FindByEmailAsync(user.Email);

        if (existingUser != null)
            throw new NotFoundException($"User with specified email already exists. Email: {user.Email}");

        var newUser = new User()
        {
            Email = user.Email,
            UserName = user.Email,
        };

        var result = await _userManager.CreateAsync(newUser, user.Password);

        if (!result.Succeeded)
            throw new UserManagerException($"User manager operation failed:\n", result.Errors);
        if (invitedUser.Role == "Teacher")
        {
            var newTeacher = new Teacher()
            {
                User = newUser,
                UserId = newUser.Id,
            };  
            await _teacherRepository.InsertAsync(newTeacher);
        }

        if (invitedUser.Role == "Student")
        {
            var newStudent = new Student()
            {
                User = newUser,
                UserId = newUser.Id,
            };
            await _studentRepository.InsertAsync(newStudent);
        }

        var addRoleResult = await _userManager.AddToRoleAsync(newUser, invitedUser.Role);

        if (!addRoleResult.Succeeded)
            throw new UserManagerException($"User manager operation failed:\n", result.Errors);

        return new AuthSuccessDTO(_tokenService.GenerateJwtToken(newUser , (await _userManager.GetRolesAsync(newUser)).ToArray()), 
            _tokenService.GenerateRefreshTokenAsync(newUser));
    }

    public async Task<AuthSuccessDTO> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var validatedToken = _tokenService.GetPrincipalFromToken(accessToken);

        var expiryDateUnix = long.Parse(validatedToken!.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

        var expiryDateTimeUtc = new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, DateTimeKind.Utc)
            .AddSeconds(expiryDateUnix);

        if (expiryDateTimeUtc > DateTime.UtcNow)
            throw new IncorrectParametersException("Access token is not expired yet");

        var jti = validatedToken!.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value;

        var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);

        if (user is null)
            throw new NotFoundException("User with this id does not exist");

        if (DateTimeOffset.UtcNow > user.RefreshToken.ExpiryDate)
            throw new ExpiredException("Refresh token is expired");

        if (user.RefreshToken.Token != refreshToken)
            throw new IncorrectParametersException("Refresh token is invalid");

        return new AuthSuccessDTO(_tokenService.GenerateJwtToken(user!, (await _userManager.GetRolesAsync(user)).ToArray()), 
            _tokenService.GenerateRefreshTokenAsync(user!));
    }
}
