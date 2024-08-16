Console.WriteLine("Press any key to start...");
Console.ReadKey(true);

using var cts = new CancellationTokenSource();

// Starts as a background task.
// Method returns a Task immediately, fires off and isn't waiting to complete, moves to the next line.
var backgroundTask = StartBackgroundService(cts.Token);

Console.WriteLine("Press any key to stop background service...");
Console.ReadKey(true);

cts.Cancel();

// After cancellation, ensures that the program doesn't exit immediately.
// Makes sure that background task has a chance to properly shut down
// (finish whole method) before the application terminates.
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
            
            // When a cancellation token is passed to Task.Delay, then it
            // throws a TaskCanceledException if the token is canceled.
            await Task.Delay(500, ct);
        }
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("Background service canceled");
    }
}