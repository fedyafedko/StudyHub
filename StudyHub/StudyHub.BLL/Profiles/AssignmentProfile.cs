using AutoMapper;
using StudyHub.Common.DTO.AssignmentDTO;
using StudyHub.Entities;

namespace StudyHub.BLL.Profiles;

public class AssignmentProfile : Profile
{
    public AssignmentProfile()
    {
        CreateMap<Assignment,  AssignmentDTO>();
        CreateMap<CreateAssignmentDTO, Assignment>();
        CreateMap<UpdateAssignmentDTO, Assignment>();
    }
}
