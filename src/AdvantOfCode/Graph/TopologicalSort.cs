namespace AdventOfCode.Graph;

public static class TopologicalSort
{
    /// <summary>
    /// Create tologically sorted list of T.
    /// The graph must be a DAG (direct acyclic graph), otherwise an exception will be thrown.
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="start"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> Sort<T>(IGraph<T> graph, T start)
    {
        HashSet<T> visited = [];
        HashSet<T> mark = [];
        Stack<T> stack = [];

        HashSet<T> nodes = Nodes(graph, start);
        
        while (nodes.Count != visited.Count)
        {
            var remaining = nodes.Except(visited).First();
            Visit(graph, remaining, visited, mark, stack);
        }

        return stack.ToList();
    }

    private static HashSet<T> Nodes<T>(IGraph<T> graph, T start)
    {
        AQueue<T> frontier = [start];
        HashSet<T> nodes = [start];

        while (!frontier.Empty)
        {
            T current = frontier.Get();

            foreach (T next in graph.Neighbors(current))
            {
                if(!nodes.Contains(next))
                {
                    frontier.Add(next);
                    nodes.Add(next);
                }
            }
        }

        return nodes;
    }

    private static void Visit<T>(IGraph<T> graph, T node, HashSet<T> visited, HashSet<T> mark, Stack<T> result)
    {
        if(visited.Contains(node)) return;
        // ReSharper disable once CanSimplifySetAddingWithSingleCall
        if(mark.Contains(node)) throw new Exception("Graph has a cycle");

        mark.Add(node);
            
        foreach (T neighbor in graph.Neighbors(node))
        {
            Visit(graph, neighbor, visited, mark, result);
        }

        mark.Remove(node);
        visited.Add(node);
        result.Push(node);
    }
}