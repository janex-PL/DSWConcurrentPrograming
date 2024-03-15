namespace DSW.ConcurrentPrograming.Lab01;

public class Buffer(int capacity)
{
    private readonly Queue<Operation> _operations = new();
    private readonly SemaphoreSlim _producerSemaphore = new(capacity,capacity);
    private readonly SemaphoreSlim _consumerSemaphore = new(0,capacity);
    private readonly Mutex _mutex = new();
    
    public void Add(Operation operation)
    {
        _producerSemaphore.Wait();
        _mutex.WaitOne();
        _operations.Enqueue(operation);
        _mutex.ReleaseMutex();
        _consumerSemaphore.Release();
    }
    public Operation? Take()
    {
        _consumerSemaphore.Wait();
        _mutex.WaitOne();
        var operation = _operations.Count > 0 ? _operations.Dequeue() : null;
        _mutex.ReleaseMutex();
        _producerSemaphore.Release();
        return operation;
    }
}