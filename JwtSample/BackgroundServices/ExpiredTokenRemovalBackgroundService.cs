using JwtSample.Repositories;

namespace JwtSample.BackgroundServices;

public class ExpiredTokenRemovalBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
	private readonly ILogger<ExpiredTokenRemovalBackgroundService> _logger;

    public ExpiredTokenRemovalBackgroundService(IServiceProvider services, ILogger<ExpiredTokenRemovalBackgroundService> logger)
    {
        _services = services;
		_logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
		_logger.LogInformation(" Token cleanup background service started.");
		
        var timer = new PeriodicTimer(TimeSpan.FromHours(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
			_logger.LogInformation(" Running cleanup cycle at: {Time}", DateTime.UtcNow);
			
            try
            {
                using var scope = _services.CreateScope();
                var refreshTokenRepo = scope.ServiceProvider.GetRequiredService<RefreshTokenRepository>();

                var toDateTime = DateTime.UtcNow.AddHours(-1);
                var deletedCount = await refreshTokenRepo.BulkDelete(toDateTime);

                _logger.LogInformation(" Removed {Count} expired tokens older than {Time}.", deletedCount, toDateTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error during token cleanup cycle.");
            }
        }
		_logger.LogInformation(" Token cleanup background service stopping.");
    }
}