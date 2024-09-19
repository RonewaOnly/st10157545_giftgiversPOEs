using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using st10157545_giftgiversPOEs.Controllers;

namespace st10157545_giftgiversPOEs.Models
{
    public class TokenRefreshService : IHostedService, IDisposable
    {
        private readonly ILogger<TokenRefreshService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;

        public TokenRefreshService(ILogger<TokenRefreshService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Token Refresh Service is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Token Refresh Service is working.");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DatabaseController>();
                    var now = DateTime.UtcNow;
                    // Fetch tokens that need to be refreshed
                    var tokensToRefresh = await context.ValidateAccesses
                        .Where(v => v.ExpireDate <= now && v.RenewlyDate == null)
                        .ToListAsync();
                    foreach (var token in tokensToRefresh)
                    {
                        var newAccessKey = Guid.NewGuid().ToString();
                        // Update the token's fields
                        token.Access_Key = newAccessKey;
                        token.RenewlyDate = now;
                        token.ExpireDate = now.AddMinutes(30); // Extend expiration to 30 minutes
                        context.ValidateAccesses.Update(token); // Mark the token for update
                    }
                    // Commit changes to the database
                    await context.SaveChangesAsync();
                }
            }
            catch (SqlException ex) when (ex.Number == -2) // SQL timeout
            {
                _logger.LogError("SQL Timeout occurred while refreshing tokens. Consider increasing the timeout setting.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while refreshing tokens: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Token Refresh Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
