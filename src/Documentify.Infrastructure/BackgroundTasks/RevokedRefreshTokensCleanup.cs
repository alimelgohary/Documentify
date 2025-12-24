using Documentify.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Documentify.Infrastructure.BackgroundTasks
{
    public class RevokedRefreshTokensCleanup(
        ILogger<RevokedRefreshTokensCleanup> _logger,
        IServiceProvider _provider)
        : BackgroundService
    {
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);
        private Timer? _timer = null;
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Revoked Refresh Tokens Cleanup service is running with interval {_interval}");
            _timer = new Timer(
                CleanupTokens,
                null,
                TimeSpan.FromMinutes(1), // Delay first run by 1 minute
                _interval  // Run every _interval
            );
            return Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Revoked Refresh Tokens Cleanup service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        async void CleanupTokens(object? state)
        {
            try
            {
                _logger.LogInformation("Running Cleanup");

                using var scope = _provider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var toBeRemoved = _context.RevokedRefreshTokens
                                        .Where(x => x.NaturalExpireDate < DateTime.UtcNow);
                _context.RevokedRefreshTokens.RemoveRange(toBeRemoved);
                var res = await _context.SaveChangesAsync();

                _logger.LogInformation($"Cleanup refresh tokens done with {res} rows affected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the cleanup method.");
            }
        }
        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
