using System.ComponentModel.DataAnnotations.Schema;

namespace StudyHub.Entities;

public class Assignment : EntityBase
{
    [ForeignKey(nameof(Subject))]
    public Guid SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int MaxMark { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime FinishDate { get; set; }
    public Subject Subject { get; set; } = null!;
    public List<AssignmentTask> Tasks { get; set; } = null!;
}
