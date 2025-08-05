using DjayAccounts.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DjayAccounts.Api;

public class DbInitializerHostedService : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    public DbInitializerHostedService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var accountDbContextOption = scope.ServiceProvider.GetRequiredService< DbContextOptions < AccountDbContext >> ();
        using var accountDbContext = new AccountDbContext(accountDbContextOption);
        await accountDbContext.Database.EnsureCreatedAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
