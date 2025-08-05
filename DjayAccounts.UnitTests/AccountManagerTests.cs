using AutoMapper;
using DjayAccounts.Core;
using DjayAccounts.DbPersistence;
using DjayAccounts.EntityFramework.Contexts;
using DjayAccounts.DbPersistence.ObjectModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DjayAccounts.UnitTests;

[TestClass]
public class AccountManagerTests
{
    private DbContextOptions<AccountDbContext> options = null!;
    private SqliteConnection connection = null!;
    private IMapper mapper = null!;
    private AccountDbPersistence persistence = null!;
    private AccountManager manager = null!;

    [TestInitialize]
    public void Setup()
    {
        this.connection = new SqliteConnection("Filename=:memory:");
        this.connection.Open();

        this.options = new DbContextOptionsBuilder<AccountDbContext>()
            .UseSqlite(this.connection)
            .Options;

        using (var context = new AccountDbContext(this.options))
        {
            context.Database.EnsureCreated();
        }

        var loggerFactory = new LoggerFactory();

        var config = new MapperConfiguration(
            _ => { _.AddProfile<AccountProfile>(); },
            loggerFactory);
        this.mapper = config.CreateMapper();

        this.persistence = new AccountDbPersistence(this.options, this.mapper);
        this.manager = new AccountManager(this.persistence);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.connection.Close();
    }

    [TestMethod]
    public async Task CreateCustomerAsync_ShouldReturnOk_WhenNewCustomer()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var result = await this.manager.CreateCustomerAsync(customerId, "John", "Doe");

        // Assert
        Assert.AreEqual(ServiceErrorCode.Ok, result);
    }

    [TestMethod]
    public async Task CreateCustomerAsync_ShouldReturnAlreadyExists_WhenCustomerExists()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        await this.manager.CreateCustomerAsync(customerId, "John", "Doe");

        // Act
        var result = await this.manager.CreateCustomerAsync(customerId, "John", "Doe");

        // Assert
        Assert.AreEqual(ServiceErrorCode.CustomerAlreadyExists, result);
    }

    [TestMethod]
    public async Task CreateCurrentAccountAsync_ShouldReturnOk_WhenValid()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        await this.manager.CreateCustomerAsync(customerId, "Alice", "Smith");
        var accountId = Guid.NewGuid();

        // Act
        var result = await this.manager.CreateCurrentAccountAsync(accountId, customerId, "USD", 100m, 50m);

        // Assert
        Assert.AreEqual(ServiceErrorCode.Ok, result);
    }

    [TestMethod]
    public async Task CreateCurrentAccountAsync_ShouldFail_WhenOverdraftNegative()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        await this.manager.CreateCustomerAsync(customerId, "Bob", "Marley");
        var accountId = Guid.NewGuid();

        // Act
        var result = await this.manager.CreateCurrentAccountAsync(accountId, customerId, "USD", 100m, -10m);

        // Assert
        Assert.AreEqual(ServiceErrorCode.ValidationFailed, result);
    }

    [TestMethod]
    public async Task FreezeAccountAsync_ShouldReturnOk_WhenActive()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        await this.manager.CreateCustomerAsync(customerId, "Nick", "Frost");
        var accountId = Guid.NewGuid();
        await this.manager.CreateCurrentAccountAsync(accountId, customerId, "USD", 100m, 0m);

        // Act
        var result = await this.manager.FreezeAccountAsync(accountId);

        // Assert
        Assert.AreEqual(ServiceErrorCode.Ok, result);
    }

    [TestMethod]
    public async Task UnfreezeAccountAsync_ShouldReturnOk_WhenFrozen()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        await this.manager.CreateCustomerAsync(customerId, "George", "Lucas");
        var accountId = Guid.NewGuid();
        await this.manager.CreateCurrentAccountAsync(accountId, customerId, "USD", 100m, 0m);
        await this.manager.FreezeAccountAsync(accountId);

        // Act
        var result = await this.manager.UnfreezeAccountAsync(accountId);

        // Assert
        Assert.AreEqual(ServiceErrorCode.Ok, result);
    }
}
