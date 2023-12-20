using StudyHub.BLL.Seeding.Behaviours;
using StudyHub.BLL.Seeding.Interfaces;
using System.Net.Mail;
using System.Net;

namespace StudyHub.Extensions;

public static class SeedingExtensions
{
    public static IServiceCollection AddSeeding(this IServiceCollection services)
    {
        services.AddScoped<ISeedingBehaviour, RoleSeedingBehaviour>();

        return services;
    }

    public static async Task ApplySeedingAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;

        var behaviours = services.GetRequiredService<IEnumerable<ISeedingBehaviour>>();

        foreach (var behaviour in behaviours)
        {
            await behaviour.SeedAsync();
        }
    }
}
