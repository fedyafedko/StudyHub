using Microsoft.AspNetCore.Identity;

namespace StudyHub.Entities;

public class User : IdentityUser<Guid>
{
    public RefreshToken RefreshToken { get; set; } = null!;
}