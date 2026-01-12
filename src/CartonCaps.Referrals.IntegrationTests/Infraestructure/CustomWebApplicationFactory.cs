using CartonCaps.Referrals.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CartonCaps.Referrals.IntegrationTests;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection _connection = null!;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ReferralDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Create in-memory Sqlite connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<ReferralDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create the database schema
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ReferralDbContext>();
            db.Database.EnsureCreated();

            SeedTestUser(db);
        });
    }

    private void SeedTestUser(ReferralDbContext context)
    {
        context.Referrals.RemoveRange(context.Referrals);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();

        context.Users.Add(new Api.Models.User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "andres@test.com",
            FirstName = "Andres",
            LastName = "Benitez",
            CreatedAt = DateTime.UtcNow.AddMonths(-2)
        });

        context.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }
}