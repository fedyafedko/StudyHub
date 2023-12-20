namespace StudyHub.Common.DTO.AssignmentDTO;

public class AssignmentDTO
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int MaxMark { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime FinishDate { get; set; }
}
