using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyHub.BLL.Services.Interfaces;
using StudyHub.Common.DTO.Subject;
using StudyHub.Common.Exceptions;
using StudyHub.DAL.Repositories.Interfaces;
using StudyHub.Entities;

namespace StudyHub.BLL.Services;

public class SubjectService : ISubjectService
{
    private readonly IRepository<Subject> _subjectRepository;
    private readonly IRepository<Teacher> _teacherRepository;
    private readonly IRepository<Student> _studentRepository;
    private readonly IMapper _mapper;

    public SubjectService(
        IRepository<Subject> subjectRepository,
        IRepository<Teacher> teacherRepository,
        IRepository<Student> studentRepository,
        IMapper mapper)
    {
        _subjectRepository = subjectRepository;
        _teacherRepository = teacherRepository;
        _studentRepository = studentRepository;
        _mapper = mapper;
    }

    public async Task<SubjectDTO> AddSubjectAsync(CreateSubjectDTO dto)
    {
        var entity = _mapper.Map<Subject>(dto);
        var teacher = await _teacherRepository.FirstOrDefaultAsync(teacher => teacher.UserId == dto.TeacherId);

        if (teacher == null)
            throw new NotFoundException($"Teacher not found in the database with this ID: {dto.TeacherId}");

        await _subjectRepository.InsertAsync(entity);

        var result = _mapper.Map<SubjectDTO>(entity);
        return result;
    }

    public async Task<bool> DeleteSubjectAsync(Guid userId, Guid subjectId)
    {
        var entity = await _subjectRepository
            .FirstOrDefaultAsync(subject => subject.Id == subjectId);

        if (entity == null)
            throw new NotFoundException($"Unable to find entity with such key: {subjectId}");

        if (entity.TeacherId != userId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        return await _subjectRepository.DeleteAsync(entity);
    }

    public async Task<SubjectDTO> GetSubjectAsync(Guid subjectId)
    {
        var entity = await _subjectRepository
            .FirstOrDefaultAsync(subject => subject.Id == subjectId);

        if (entity == null)
            throw new NotFoundException($"Unable to find entity with such key: {subjectId}");

        return _mapper.Map<SubjectDTO>(entity);
    }

    public List<SubjectDTO> GetSubjectsForStudent(Guid studentId)
    {
        var student =  _studentRepository
                .Include(subject => subject.Subjects)
                .FirstOrDefault(s => s.UserId == studentId);

        if (student == null)
            throw new NotFoundException($"Unable to find entity with such key: {studentId}");

        var subjects = _mapper
            .Map<List<SubjectDTO>>(student.Subjects);
                
        return subjects.ToList();
    }

    public List<SubjectDTO> GetSubjectsForTeacher(Guid teacherId)
    {
        var subjects =_mapper
            .Map<List<SubjectDTO>>(_subjectRepository
                .Where(x => x.TeacherId == teacherId));

        return subjects.ToList();
    }

    public async Task<SubjectDTO> UpdateSubjectAsync(Guid userId, Guid subjectId, UpdateSubjectDTO dto)
    {
        var entity = await _subjectRepository
            .FirstOrDefaultAsync(subject => subject.Id == subjectId);

        if (entity == null)
            throw new NotFoundException($"Unable to find entity with such key: {subjectId}");

        if (entity.TeacherId != userId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        _mapper.Map(dto, entity);

        await _subjectRepository.UpdateAsync(entity);

        return _mapper.Map<SubjectDTO>(entity);
    }
}
