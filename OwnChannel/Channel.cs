using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class Channel<T>
{
    private ConcurrentQueue<T> _items = [];
    private bool _isStopped = false;
    private SemaphoreSlim _sem = new(0);

    public void Send(T item)
    {
        if (_isStopped)
            throw new InvalidOperationException("Channel has been stopped");
        
        _items.Enqueue(item);
        _sem.Release();
    }

    public async Task<T> Read(CancellationToken ct)
    {
        await _sem.WaitAsync(ct);
        return _items.TryDequeue(out var result) ? result : throw new UnreachableException();
    }
    
    public async IAsyncEnumerable<T> ReadAll([EnumeratorCancellation] CancellationToken ct = default)
    {
        while (!IsCompleted)
        {
            await _sem.WaitAsync(ct);
            yield return _items.TryDequeue(out var result) ? result : throw new UnreachableException();
        }
    }

    public void Stop() => _isStopped = true;

    public bool IsCompleted => _isStopped && _items.IsEmpty;
}