using AutoMapper;
using StudyHub.BLL.Services.Interfaces;
using StudyHub.Common.DTO;
using StudyHub.Common.Exceptions;
using StudyHub.DAL.Repositories.Interfaces;
using StudyHub.Entities;
using StudyHub.FluentEmail.Interfaces;
using System.Security.Cryptography;

namespace StudyHub.BLL.Services;

public class UserInvitedService : IUserInvitingService
{
    private readonly IRepository<InvitedUser> _invitedUserRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IReadOnlyList<string> Roles = new List<string>
    {
        "Admin",
        "Student",
        "Teacher"
    }.AsReadOnly();

    public UserInvitedService(IRepository<InvitedUser> invitedUserRepository, IMapper mapper, IEmailService emailService)
    {
        _emailService = emailService;
        _invitedUserRepository = invitedUserRepository;
        _mapper = mapper;
    }

    public async Task InviteAsync(string email, string role)
    {
        if (!Roles.Contains(role))
            throw new NotFoundException($"Role {role} doesn't exist");

        var registration = new InvitedUserDTO
        {
            Email = email,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Role = role
        };

        var sendMessage = await _emailService.Send(registration);

        if (!sendMessage)
            throw new Exception("Something went wrong, invintation was not sent");

        var result = _mapper.Map<InvitedUser>(registration);

        await _invitedUserRepository.InsertAsync(result);
    }

    public async Task InviteStudentsAsync(InviteStudentsRequest inviteStudentsRequest)
    {
        foreach (var email in inviteStudentsRequest.Email)
        {
            var registration = new InvitedUserDTO
            {
                Email = email,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Role = "Student"
            };

            var sendMessage = await _emailService.Send(registration);

            if (!sendMessage)
                throw new Exception("Something went wrong, invintation was not sent");

            var result = _mapper.Map<InvitedUser>(registration);

            await _invitedUserRepository.InsertAsync(result);
        }
    }
}
