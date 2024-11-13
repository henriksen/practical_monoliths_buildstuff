using Testcontainers.MsSql;

namespace Commerce.Tests.Integration;

[SetUpFixture]
public class SqlServerContainerFixture
{
    public static MsSqlContainer SqlServerContainer { get; private set; } = null!;
    public static string ConnectionString { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task GlobalSetupAsync()
    {
        try
        {
            SqlServerContainer = new MsSqlBuilder()
                .Build();

            await SqlServerContainer.StartAsync();

            ConnectionString = SqlServerContainer.GetConnectionString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during container startup: {ex.Message}");
            throw;
        }
    }

    [OneTimeTearDown]
    public Task GlobalTeardownAsync()
    {
        SqlServerContainer.DisposeAsync();

        return Task.CompletedTask;
    }
}