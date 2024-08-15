// https://youtu.be/E0Ld7ZgE4oY?list=PLqid4IboAPvpAg8XodZbeDvuhWoZ8gDv9

// Channels solve the producer-consumer problem, which is an in-memory queue.
// We could fire and forget background tasks with Task.Run(...),
// but it might flood a ThreadPool or injected service can be disposed in a caller scope.
// Instead of sharing a global object with state, we can share the channel,
// that is only responsible for sending messages - a different memory approach.

// TODO: https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/

var channel = new Channel<int>();
var cts = new CancellationTokenSource();

// Init handlers
var handleTasks = new[]
{
    Task.Run(() => HandleMessages(channel)),
    Task.Run(() => AsyncHandleMessages(channel)),
};

// Init senders
var senderTasks = new[]
{
    Task.Run(() => SendMessages(channel)),
    Task.Run(() => SendMessages(channel))
};

// Wait for senders and complete the channel
await Task.WhenAll(senderTasks);
channel.Stop();

// Wait for handlers and cancel the token
cts.CancelAfter(TimeSpan.FromSeconds(1));
await Task.WhenAll(handleTasks);

void SendMessages(Channel<int> channel)
{
    Console.WriteLine($"Message sender with id '{Environment.CurrentManagedThreadId}' started");

    for (int i = 0; i < 3; i++)
    {
        int number = Random.Shared.Next(1, 100);
        Console.WriteLine($"[{Environment.CurrentManagedThreadId}] Sending '{number}' ...");
        channel.Send(number);
        Thread.Sleep(Random.Shared.Next(500)); // simulate work
    }
}

async Task HandleMessages(Channel<int> channel)
{
    Console.WriteLine($"Message handler with id '{Environment.CurrentManagedThreadId}' started");
    
    while (!channel.IsCompleted)
    {
        int msg;
        try
        {
            msg = await channel.Read(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Channel read canceled");
            break;
        }
        
        Console.WriteLine($"[{Environment.CurrentManagedThreadId}] Processing message: '{msg}' ...");
        Thread.Sleep(Random.Shared.Next(500)); // simulate work
    }
}

async Task AsyncHandleMessages(Channel<int> channel)
{
    Console.WriteLine($"Async message handler with id '{Environment.CurrentManagedThreadId}' started");
    
    try
    {
        await foreach (int msg in channel.ReadAll().WithCancellation(cts.Token))
        {
            Console.WriteLine($"[{Environment.CurrentManagedThreadId}] Processing message: '{msg}' ...");
            Thread.Sleep(Random.Shared.Next(500)); // simulate work
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Channel read canceled");
    }
}
