using StudyHub.Common.DTO;

namespace StudyHub.BLL.Services.Interfaces;
public interface IUserInvitingService
{
    Task InviteStudentsAsync(InviteStudentsRequest inviteStudentsRequest);
    Task InviteAsync(string email, string role);
}
