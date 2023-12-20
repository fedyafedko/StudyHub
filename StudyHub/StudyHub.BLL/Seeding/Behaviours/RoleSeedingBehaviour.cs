using Microsoft.AspNetCore.Identity;
using StudyHub.BLL.Seeding.Interfaces;

namespace StudyHub.BLL.Seeding.Behaviours;

public class RoleSeedingBehaviour : ISeedingBehaviour
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RoleSeedingBehaviour(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    } 

    public async Task SeedAsync()
    {
        var roles = new List<string>
        {
            "Admin",
            "Teacher",
            "Student"
        };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
