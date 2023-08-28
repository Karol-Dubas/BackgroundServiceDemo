namespace PeriodicTimerDemo.Console;

public class BackgroundTask
{
    private  Task? _timerTask;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();

    public BackgroundTask(TimeSpan interval)
    {
        _timer = new PeriodicTimer(interval);
    }

    public void Start()
    {
        _timerTask = StartLoop();
    }

    public async Task Stop()
    {
        System.Console.WriteLine($"'{nameof(Stop)}' method start");

        if (_timerTask is null)
            return;
        
        _cts.Cancel();
        await _timerTask;
        _cts.Dispose();
        
        System.Console.WriteLine($"'{nameof(Stop)}' method end");
    }
    
    private async Task StartLoop()
    {
        System.Console.WriteLine($"'{nameof(StartLoop)}' method start");

        try
        {
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                System.Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            }
        }
        catch (OperationCanceledException e)
        {
            System.Console.WriteLine($"Exception: {e.Message}");
        }
        finally
        {
            System.Console.WriteLine($"'{nameof(StartLoop)}' method end");
        }
    }
}