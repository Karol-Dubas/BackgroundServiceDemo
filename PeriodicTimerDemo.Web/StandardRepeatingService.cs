namespace PeriodicTimerDemo.Web;

public class StandardRepeatingService : BackgroundService
{
    private readonly ILogger<StandardRepeatingService> _logger;

    public StandardRepeatingService(ILogger<StandardRepeatingService> logger)
    {
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting " + nameof(StartAsync));
        var startTask = base.StartAsync(cancellationToken);
        _logger.LogInformation("Started " + nameof(StartAsync));
        return startTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(nameof(ExecuteAsync));

        // TODO: exception bubble up, must be caught!
        
        while (!stoppingToken.IsCancellationRequested)
        {
            // Losing 1ms every time
            // It gets worse if operations here take a long time

            _logger.LogInformation(DateTimeOffset.UtcNow.ToString("O"));
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(StopAsync));
        return base.StopAsync(cancellationToken);
    }
}