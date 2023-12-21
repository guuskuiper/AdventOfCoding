using System.Collections;

namespace AdventOfCode.Graph;

public class AQueue<T> : IEnumerable<T>
{
    private readonly Queue<T> queue;

    public AQueue() : this(Enumerable.Empty<T>())
    {
    }

    public AQueue(IEnumerable<T> enumerable)
    {
        this.queue = new Queue<T>(enumerable);
    }

    public bool Empty => queue.Count == 0;
    public void Add(T node) => queue.Enqueue(node);
    public T Get() => queue.Dequeue();
    public IEnumerator<T> GetEnumerator()
    {
        return queue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}