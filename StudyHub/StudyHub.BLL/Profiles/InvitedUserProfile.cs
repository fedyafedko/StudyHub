using AutoMapper;
using StudyHub.Common.DTO;
using StudyHub.Entities;

namespace StudyHub.BLL.Profiles;

public class InvitedUserProfile : Profile
{
    public InvitedUserProfile() 
    {
        CreateMap<InvitedUserDTO, InvitedUser>();
    }
}
