using Commerce.Features.Customer.Data;
using Commerce.Features.Identity.Data;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Tests.Integration;

public class CustomWebApplicationFactory(string databaseName) : WebApplicationFactory<Program>
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureServices(services =>
            {
                // Remove the existing DbContext registrations
                var descriptors = services.Where(
                    d =>
                        d.ServiceType == typeof(DbContextOptions<CustomerDbContext>)
                        || d.ServiceType == typeof(DbContextOptions<IdentityDbContext>)
                ).ToList();
                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                var connectionString = $"{SqlServerContainerFixture.ConnectionString};Database={databaseName};TrustServerCertificate=True;";

                // Add an in-memory database for testing
                services.AddDbContext<CustomerDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
                services.AddDbContext<IdentityDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
                var sp = services.BuildServiceProvider();

                // Ensure the database is created
                using var scope = sp.CreateScope();
                List<DbContext> dbs =
                [
                   scope.ServiceProvider.GetRequiredService<IdentityDbContext>(),
                   scope.ServiceProvider.GetRequiredService<CustomerDbContext>(),
                ];
                foreach (var db in dbs)
                {
                    db.Database.Migrate();
                }


            });
    }
}