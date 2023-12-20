using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyHub.BLL.Services.Interfaces;
using StudyHub.Common.DTO.AssignmentTask;
using StudyHub.Common.Exceptions;
using StudyHub.DAL.Repositories.Interfaces;
using StudyHub.Entities;

namespace StudyHub.BLL.Services;

public class AssignmentTaskService : IAssignmentTaskService
{
    private readonly IRepository<AssignmentTask> _assignmentTaskRepository;
    private readonly IRepository<Assignment> _assignmentRepository;
    private readonly IOptionsService _optionsService;
    private readonly IMapper _mapper;

    public AssignmentTaskService(
        IRepository<AssignmentTask> repositoryAssignmentTask, 
        IRepository<Assignment> repositoryAssignment,
        IOptionsService optionsService,
        IMapper mapper)
    {
        _assignmentTaskRepository = repositoryAssignmentTask;
        _assignmentRepository = repositoryAssignment;
        _optionsService = optionsService;
        _mapper = mapper;
    }

    public async Task<AssignmentTaskDTO> AddAssignmentTaskAsync(CreateAssignmentTaskDTO dto)
    {
        var entity = _mapper.Map<AssignmentTask>(dto);
        var assignment = await _assignmentRepository.FirstOrDefaultAsync(assignment => assignment.Id == dto.AssignmentId);

        if (assignment == null)
            throw new NotFoundException($"Assignment not found in the database with this ID: {dto.AssignmentId}");
        
        await _assignmentTaskRepository.InsertAsync(entity);
        var options = await _optionsService.AddAssignmentTaskOptionsAsync(entity.Id, dto.Options);

        var result = _mapper.Map<AssignmentTaskDTO>(entity);
        result.Options = options;

        return result;
    }

    public async Task<bool> DeleteAssignmentTaskAsync(Guid assignmentTaskId)
    {
        var entity = await _assignmentTaskRepository
            .FirstOrDefaultAsync(assignmentTask => assignmentTask.Id == assignmentTaskId);

        if (entity == null)
            throw new KeyNotFoundException($"Unable to find entity with such key {assignmentTaskId}");

        return await _assignmentTaskRepository.DeleteAsync(entity);
    }

    public async Task<List<AssignmentTaskDTO>> GetAssignmentTaskAsync(Guid assignmentId)
    {
        var result = await _assignmentTaskRepository
            .Include(assignmentTask => assignmentTask.Options)
            .Where(assignmentTask => assignmentTask.AssignmentId == assignmentId)
            .ToListAsync();

        if (result.Count == 0)
            throw new NotFoundException($"Unable to find entity with such key {assignmentId}");

        return _mapper.Map<List<AssignmentTaskDTO>>(result);
    }

    public async Task<AssignmentTaskDTO> UpdateAssignmentTaskAsync(Guid assignmentTaskId, UpdateAssignmentTaskDTO dto)
    {
        var entity = await _assignmentTaskRepository
            .Include(assignmentTask => assignmentTask.Options)
            .FirstOrDefaultAsync(assignmentTask => assignmentTask.Id == assignmentTaskId);

        if (entity == null)
            throw new NotFoundException($"Unable to find entity with such key {assignmentTaskId}");

        _mapper.Map(dto, entity);

        await _assignmentTaskRepository.UpdateAsync(entity);
        await _optionsService.UpdateAssignmentTaskOptionsAsync(entity.Id, dto.Options);

        return _mapper.Map<AssignmentTaskDTO>(entity);
    }
}
