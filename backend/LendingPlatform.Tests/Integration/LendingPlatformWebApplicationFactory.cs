using LendingPlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LendingPlatform.Tests.Integration;

public class LendingPlatformWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private const string TestConnectionString =
        "Server=localhost;Database=LendingPlatform_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<LendingDbContext>();
            services.RemoveAll<DbContextOptions<LendingDbContext>>();

            services.AddDbContext<LendingDbContext>(options =>
            {
                options.UseSqlServer(TestConnectionString);
            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<LendingDbContext>();

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<LendingDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }
}