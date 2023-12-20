using System.ComponentModel.DataAnnotations.Schema;

namespace StudyHub.Entities;

public class Subject : EntityBase
{
    [ForeignKey(nameof(Teacher))]
    public Guid TeacherId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Teacher Teacher { get; set; } = null!;
    public List<Student> Students { get; set; } = null!;
    public List<Assignment> Assignments { get; set; } = null!;
}
