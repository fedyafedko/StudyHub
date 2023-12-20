using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyHub.Entities;

public class RefreshToken
{
    [Key]
    public string Token { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime ExpiryDate { get; set; }
    public Guid UserId { get; set; }
    public bool Used { get; set; }
    public bool Invalidated { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
