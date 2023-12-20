using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudyHub.Entities;

namespace StudyHub.DAL.EF;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Teacher> Teachers { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<AssignmentTask> AssignmentTasks { get; set; } = null!;
    public DbSet<ChoiceOption> ChoiceAnswers { get; set; } = null!;
    public DbSet<OpenEndedOption> OpenEndedAnswers { get; set; } = null!;
    public DbSet<StudentSelectedOption> StudentSelectedOptions { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<InvitedUser> InvitedUsers { get; set; } = null!;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
