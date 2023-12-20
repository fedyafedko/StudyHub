using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyHub.BLL.Services.Interfaces;
using StudyHub.Common.DTO.AssignmentDTO;
using StudyHub.Common.Exceptions;
using StudyHub.DAL.Repositories.Interfaces;
using StudyHub.Entities;

namespace StudyHub.BLL.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IRepository<Assignment> _assignmentRepository;
    private readonly IRepository<Subject> _subjectRepository;
    private readonly IMapper _mapper;

    public AssignmentService(
        IRepository<Assignment> assignmentRepository, 
        IRepository<Subject> subjectRepository, 
        IMapper mapper)
    {
        _assignmentRepository = assignmentRepository;
        _subjectRepository = subjectRepository;
        _mapper = mapper;
    }
    public async Task<AssignmentDTO> CreateAssignmentAsync(CreateAssignmentDTO dto)
    {
        var entity = _mapper.Map<Assignment>(dto);
        var subject = await _subjectRepository.FirstOrDefaultAsync(x => x.Id == dto.SubjectId);

        if (subject == null)
            throw new NotFoundException($"Subject not found in the database with this Id: {dto.SubjectId}");

        await _assignmentRepository.InsertAsync(entity);

        var result = _mapper.Map<AssignmentDTO>(entity);
        return result;
    }

    public async Task<bool> DeleteAssignmentAsync(Guid assignmentId)
    {
        var entity = await _assignmentRepository
            .FirstOrDefaultAsync(x => x.Id == assignmentId);

        if (entity == null)
            throw new NotFoundException($"Unable to find entity with such key: {assignmentId}");

        return await _assignmentRepository.DeleteAsync(entity);
    }

    public async Task<AssignmentDTO> GetAssignmentByIdAsync(Guid assignmentId)
    {
        var entity = await _assignmentRepository
            .FirstOrDefaultAsync(x => x.Id == assignmentId);

        if (entity == null)
            throw new NotFoundException($"Unable to find entity with such key: {assignmentId}");

        return _mapper.Map<AssignmentDTO>(entity);
    }

    public async Task<List<AssignmentDTO>> GetAssignmentsBySubjectIdAsync(Guid subjectId)
    {
        var entity = await _subjectRepository
            .Include(x => x.Assignments)
            .FirstOrDefaultAsync(x => x.Id == subjectId);

        if (entity == null)
            throw new NotFoundException($"Unable to find entity with such key: {subjectId}");

        return _mapper.Map<List<AssignmentDTO>>(entity.Assignments);
    }

    public async Task<AssignmentDTO> UpdateAssignmentAsync(Guid assignmentId, UpdateAssignmentDTO dto)
    {
        var entity = await _assignmentRepository.FirstOrDefaultAsync(x => x.Id == assignmentId);

        if (entity == null)
            throw new NotFoundException($"Unable to find entity with such key: {assignmentId}");

        _mapper.Map(dto, entity);

        await _assignmentRepository.UpdateAsync(entity);

        return _mapper.Map<AssignmentDTO>(entity);
    }
}
