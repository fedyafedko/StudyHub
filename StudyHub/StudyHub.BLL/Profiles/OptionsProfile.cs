using AutoMapper;
using StudyHub.Common.DTO.AssignmentTaskOption;
using StudyHub.Entities;

namespace StudyHub.BLL.Profiles;

public class OptionsProfile : Profile
{
    public OptionsProfile()
    {
        CreateMap<CreateAssignmentTaskOptionDTO, ChoiceOption>();
        CreateMap<CreateAssignmentTaskOptionDTO, OpenEndedOption>();
        CreateMap<ChoiceOption, AssignmentTaskOptionDTO>();
        CreateMap<OpenEndedOption, AssignmentTaskOptionDTO>();
        CreateMap<UpdateAssignmentTaskOptionDTO, ChoiceOption>();
        CreateMap<UpdateAssignmentTaskOptionDTO, OpenEndedOption>();
    }
}
