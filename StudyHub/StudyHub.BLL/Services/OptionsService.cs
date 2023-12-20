using AutoMapper;
using StudyHub.BLL.Services.Interfaces;
using StudyHub.Common.DTO.AssignmentTaskOption;
using StudyHub.DAL.Repositories.Interfaces;
using StudyHub.Entities;

namespace StudyHub.BLL.Services;

public class OptionsService : IOptionsService
{
    private readonly IRepository<AssignmentTaskOptionBase> _optionRepository;
    private readonly IRepository<ChoiceOption> _choiceOptionRepository;
    private readonly IRepository<OpenEndedOption> _openEndedOptionRepository;
    private readonly IMapper _mapper;

    public OptionsService(
        IRepository<AssignmentTaskOptionBase> optionRepository,
        IRepository<ChoiceOption> choiceOptionsRepository,
        IRepository<OpenEndedOption> openEndedOptionRepository,
        IMapper mapper)
    {
        _optionRepository = optionRepository;
        _choiceOptionRepository = choiceOptionsRepository;
        _openEndedOptionRepository = openEndedOptionRepository;
        _mapper = mapper;
    }

    public async Task<List<AssignmentTaskOptionDTO>> AddAssignmentTaskOptionsAsync(Guid assignmentTaskId, List<CreateAssignmentTaskOptionDTO> taskOptions)
    {
        // ToDo: remove unnecessary call
        return await AddSeparationOptions(assignmentTaskId, taskOptions);
    }

    public async Task<List<AssignmentTaskOptionDTO>> UpdateAssignmentTaskOptionsAsync(Guid taskId, List<UpdateAssignmentTaskOptionDTO> dto)
    {
        // ToDo: remove unnecessary call
        return await UpdateSeparationOptions(taskId, dto);
    }

    private async Task<List<AssignmentTaskOptionDTO>> UpdateSeparationOptions(
        Guid assignmentTaskId,
        List<UpdateAssignmentTaskOptionDTO> taskOptions)
    {
        bool isOpenEnded = taskOptions.All(option => option.IsCorrect == null);

        if (isOpenEnded)
        {
            var entity = _openEndedOptionRepository
                .Where(x => x.AssignmentTaskId == assignmentTaskId)
                .ToList();

            for (var i = 0; i < entity.Count; i++)
            {
                _mapper.Map(taskOptions[i], entity[i]);
            }

            await _optionRepository.UpdateManyAsync(entity);

            return _mapper.Map<List<AssignmentTaskOptionDTO>>(entity);
        }
        else
        {
            var entity = _choiceOptionRepository
                .Where(x => x.AssignmentTaskId == assignmentTaskId)
                .ToList();

            for (var i = 0; i < entity.Count; i++)
            {
                _mapper.Map(taskOptions[i], entity[i]);
            }

            await _choiceOptionRepository.UpdateManyAsync(entity);

            return _mapper.Map<List<AssignmentTaskOptionDTO>>(entity);
        }
    }

    private async Task<List<AssignmentTaskOptionDTO>> AddSeparationOptions(
        Guid assignmentTaskId,
        List<CreateAssignmentTaskOptionDTO> taskOptions)
    {
        bool isOpenEnded = taskOptions.All(option => option.IsCorrect == null);

        var options = isOpenEnded
            ? _mapper.Map<List<OpenEndedOption>>(taskOptions).Cast<AssignmentTaskOptionBase>().ToList()
            : _mapper.Map<List<ChoiceOption>>(taskOptions).Cast<AssignmentTaskOptionBase>().ToList();

        options.ForEach(opt => opt.AssignmentTaskId = assignmentTaskId);

        var res = isOpenEnded
            ? await _openEndedOptionRepository.InsertManyAsync(options.OfType<OpenEndedOption>())
            : await _choiceOptionRepository.InsertManyAsync(options.OfType<ChoiceOption>());

        return _mapper.Map<List<AssignmentTaskOptionDTO>>(options);
    }
}
