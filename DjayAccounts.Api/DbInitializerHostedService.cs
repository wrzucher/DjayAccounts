using DjayAccounts.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DjayAccounts.Api;

/// <summary>
/// HostedService for Database initialization.
/// </summary>
public class DbInitializerHostedService : IHostedService
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbInitializerHostedService"/> class.
    /// </summary>
    public DbInitializerHostedService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var accountDbContextOption = scope.ServiceProvider.GetRequiredService< DbContextOptions < AccountDbContext >> ();
        using var accountDbContext = new AccountDbContext(accountDbContextOption);
        await accountDbContext.Database.EnsureCreatedAsync();
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
