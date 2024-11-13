using Commerce.Features.Customer.Data;

namespace Commerce.Tests.Integration;

public abstract class IntegrationTest
{
    protected HttpClient _testClient;
    protected CustomWebApplicationFactory _factory;
    private string _databaseName;

    [SetUp]
    public void SetUp()
    {
        _databaseName = $"TestDb_{Guid.NewGuid()}";
        _factory = new CustomWebApplicationFactory(_databaseName);
        _testClient = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        DropDatabase(_factory, _databaseName);

        _testClient?.Dispose();
        _factory?.Dispose();

    }

    private static void DropDatabase(CustomWebApplicationFactory factory, string databaseName)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
        db.Database.EnsureDeleted();
    }
}