// https://youtu.be/E0Ld7ZgE4oY?list=PLqid4IboAPvpAg8XodZbeDvuhWoZ8gDv9

// Channels solve producer-consumer problem, which is an in-memory queue.
// We could fire and forget background tasks with Task.Run(...),
// but it might flood a ThreadPool or injected service can be disposed in a caller scope.
// Instead of sharing a global object with state we can share the channel,
// that is only responsible for sending messages - different memory approach.

// TODO: https://code-maze.com/aspnetcore-long-running-tasks-monolith-app/

using System.Collections.Concurrent;

var channel = new Channel<int>();
await Task.WhenAll(
    Task.Run(() => SendMessages(channel)),
    Task.Run(() => SendMessages(channel)),
    Task.Run(() => HandleMessages(channel)),
    Task.Run(() => HandleMessages(channel)),
    Task.Run(() => HandleMessages(channel))
    );

void SendMessages(Channel<int> channel)
{
    Console.WriteLine($"Message sender with id '{Environment.CurrentManagedThreadId}' started");

    for (int i = 0; i < 5; i++)
    {
        int number = Random.Shared.Next(1, 100);
        Console.WriteLine($"[{Environment.CurrentManagedThreadId}] Sent '{number}'");
        channel.Send(number);
        Thread.Sleep(Random.Shared.Next(1000)); // simulate work
    }
    
    channel.Stop();
}

async Task HandleMessages(Channel<int> channel)
{
    Console.WriteLine($"Message handler with id '{Environment.CurrentManagedThreadId}' started");
    
    while (!channel.IsCompleted)
    {
        int msg = await channel.Read();
        Console.WriteLine($"[{Environment.CurrentManagedThreadId}] Processing message: '{msg}' ...");
        Thread.Sleep(Random.Shared.Next(1000)); // simulate work
    }
}

class Channel<T>
{
    private ConcurrentQueue<T> _items = [];
    private bool _isCompleted = false;
    private SemaphoreSlim _sem = new(0);

    public void Send(T item)
    {
        _items.Enqueue(item);
        _sem.Release();
    }

    public async Task<T?> Read()
    {
        await _sem.WaitAsync(); // BUG: consumers wait here forever when all values were read
        return _items.TryDequeue(out var result) ? result : default;
    }

    public void Stop() => _isCompleted = true;

    public bool IsCompleted => _isCompleted && _items.IsEmpty;
}