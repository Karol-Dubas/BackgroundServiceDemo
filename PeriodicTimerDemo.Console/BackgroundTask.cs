public class BackgroundTask
{
    private readonly Action _action;
    private Task? _loopTask;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cts = new();

    public BackgroundTask(Action action, TimeSpan period)
    {
        _action = action;
        _timer = new PeriodicTimer(period);
    }

    public void Start()
    {
        _loopTask = StartLoop();
    }

    public async Task Stop()
    {
        Console.WriteLine($"'{nameof(Stop)}' method start");

        if (_loopTask is null)
            return;
        
        _cts.Cancel();
        await _loopTask;
        _cts.Dispose();
        _timer.Dispose();
        
        Console.WriteLine($"'{nameof(Stop)}' method end");
    }
    
    private async Task StartLoop()
    {
        await Task.Yield();
        
        Console.WriteLine($"'{nameof(StartLoop)}' method start");
        
        try
        {
            // Tries to stay as close as possible to the set period
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                _action();
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation canceled");
        }
        finally
        {
            Console.WriteLine($"'{nameof(StartLoop)}' method end");
        }
    }
}