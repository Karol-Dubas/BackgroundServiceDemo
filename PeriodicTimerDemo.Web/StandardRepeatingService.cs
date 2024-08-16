namespace PeriodicTimerDemo.Web;

public class StandardRepeatingService : BackgroundService
{
    private readonly ILogger<StandardRepeatingService> _logger;

    public StandardRepeatingService(ILogger<StandardRepeatingService> logger)
    {
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken ct)
    {
        _logger.LogInformation($"Executing '{nameof(StartAsync)}'...");
        var startTask = base.StartAsync(ct);
        _logger.LogInformation($"Executed '{nameof(StartAsync)}'");
        return startTask;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation($"Starting '{nameof(ExecuteAsync)}'...");

        try
        {
            while (!ct.IsCancellationRequested)
            {
                // Losing ~1ms every time.
                // It gets worse if operation execution here takes a longer time.
                _logger.LogInformation(DateTimeOffset.UtcNow.ToString("O"));
                await Task.Delay(1000, ct);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation($"Exiting '{nameof(ExecuteAsync)}'...");
        }
        
        _logger.LogInformation($"'{nameof(StandardRepeatingService)}' was canceled");
    }

    public override Task StopAsync(CancellationToken ct)
    {
        _logger.LogInformation($"Executing '{nameof(StopAsync)}'...");
        var stopTask = base.StopAsync(ct);
        _logger.LogInformation($"Executed '{nameof(StopAsync)}'");
        return stopTask;
    }
}