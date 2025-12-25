using Documentify.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Documentify.IntegrationTests
{
    public class NoDbWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // DbContext replacement (SQLite in-memory)
                var descriptor = services.Single(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                services.Remove(descriptor);

                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                services.AddSingleton(connection);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(connection));
            });
        }

    }

}
