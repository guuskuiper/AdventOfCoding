namespace AdventOfCode.Graph;

public class AQueue<T>
{
    private readonly Queue<T> queue;

    public AQueue()
    {
        this.queue = new Queue<T>();
    }

    public bool Empty => queue.Count == 0;
    public void Add(T node) => queue.Enqueue(node);
    public T Get() => queue.Dequeue();
}