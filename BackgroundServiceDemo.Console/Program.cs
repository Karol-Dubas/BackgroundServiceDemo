Console.WriteLine("Press any key to start...");
Console.ReadKey(true);

using var cts = new CancellationTokenSource();

// Starts as background task. Method returns Task immediately,
// fires off and isn't waiting to complete, moves to next line.
var backgroundTask = StartBackgroundService(cts.Token);

Console.WriteLine("Press any key to stop background service...");
Console.ReadKey(true);

cts.Cancel();

// After cancellation, ensures that the program doesn't exit immediately.
// Makes sure that background task has a chance to properly shut down
// (finish whole method) before application terminates.
await backgroundTask;

Console.WriteLine("Press any key to exit...");
Console.ReadKey(true);


async Task StartBackgroundService(CancellationToken ct)
{
    try
    {
        while (!ct.IsCancellationRequested)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            
            // When a cancellation token is passed to Task.Delay,then Task.Delay
            // throws a TaskCanceledException if the token is canceled.
            await Task.Delay(1000/*, ct*/);
        }

        Console.WriteLine("Cancellation token isn't passed to Task.Delay");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("Canceled");
    }
}