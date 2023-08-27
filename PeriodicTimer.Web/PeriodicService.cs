namespace PeriodicTimer.Web;

public class PeriodicService : BackgroundService
{
    private readonly System.Threading.PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));
    private readonly ILogger<PeriodicService> _logger;

    public PeriodicService(ILogger<PeriodicService> logger)
    {
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken) 
               && !stoppingToken.IsCancellationRequested)
        {
            // Tries to stay as close to the set delay as possible
            
            _logger.LogInformation(DateTimeOffset.UtcNow.ToString("O"));
        }
    }
}