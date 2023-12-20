using StudyHub.Common.DTO.AssignmentTaskOption;

namespace StudyHub.Common.DTO.AssignmentTask;

public class UpdateAssignmentTaskDTO
{
    public string Label { get; set; } = string.Empty;
    public int Mark { get; set; }
    public List<UpdateAssignmentTaskOptionDTO> Options { get; set; } = null!;
}
