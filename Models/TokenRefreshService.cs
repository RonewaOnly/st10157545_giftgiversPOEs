using Microsoft.EntityFrameworkCore;
using ST10157545_GIFTGIVERS.Controllers;

namespace ST10157545_GIFTGIVERS.Models
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

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DatabaseController>();
                    var now = DateTime.UtcNow;
                    var tokensToRefresh = await context.ValidateAccesses
                        .Where(v => v.ExpireDate <= now && v.RenewlyDate == null)
                        .ToListAsync();

                    foreach (var token in tokensToRefresh)
                    {
                        var newAccessKey = Guid.NewGuid().ToString();

                        token.Access_Key = newAccessKey;
                        token.RenewlyDate = now;
                        token.ExpireDate = now.AddMinutes(30); // Extend expiration to 30 minutes

                        context.ValidateAccesses.Update(token);
                    }

                    await context.SaveChangesAsync();
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

