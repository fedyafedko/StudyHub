using AutoMapper;
using StudyHub.Common.DTO.Subject;
using StudyHub.Entities;

namespace StudyHub.BLL.Profiles;

public class SubjectProfile : Profile
{
    public SubjectProfile()
    {
        CreateMap<Subject, SubjectDTO>();
        CreateMap<CreateSubjectDTO, Subject>();
        CreateMap<UpdateSubjectDTO, Subject>();
    }
}
