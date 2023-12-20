using FluentEmail.Core;
using Microsoft.Extensions.Options;
using StudyHub.Common.DTO;
using StudyHub.Common.Models;
using StudyHub.FluentEmail.Interfaces;

namespace StudyHub.FluentEmail;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;
    private readonly MessageSettings _messageSettings;

    public EmailService(IFluentEmail fluentEmail, IOptions<MessageSettings> messageSettings)
    {
        _fluentEmail = fluentEmail;
        _messageSettings = messageSettings.Value;
    }

    public async Task<bool> Send(InvitedUserDTO invitedUserDTO)
    {
        string url = $"https://localhost:1234/accept-invitation?role={invitedUserDTO.Role}&token={invitedUserDTO.Token}";

        var sendEmail = await _fluentEmail
              .To(invitedUserDTO.Email)
              .Subject("Invitation")
              .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}.FluentEmail//{_messageSettings.MessagePath}", new{ invitedUserDTO.Email, invitedUserDTO.Role, Url = url })
              .SendAsync();

        _fluentEmail.Data.ToAddresses.Clear();

        return sendEmail.Successful;
    }
}
