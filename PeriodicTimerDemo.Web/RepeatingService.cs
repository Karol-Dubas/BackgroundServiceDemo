namespace PeriodicTimerDemo.Web;

public class RepeatingService : BackgroundService
{
    private readonly ILogger<RepeatingService> _logger;

    public RepeatingService(ILogger<RepeatingService> logger)
    {
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Losing 1ms every time
            // It gets worse if operations here take long time
            
            _logger.LogInformation(DateTimeOffset.UtcNow.ToString("O"));
            await Task.Delay(1000, stoppingToken);
        }
    }
}