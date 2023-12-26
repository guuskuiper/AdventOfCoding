namespace AdventOfCode.Graph;

public static class DFS
{
    public static Dictionary<T, T> SearchToGoalFunc<T>(IGraph<T> graph, T start, Func<T, bool> isGoalReached)
        where T : notnull
    {
        HashSet<T> visited = [];
        Stack<T> stack = [];
        Dictionary<T, T> from = new()
        {
            [start] = start
        };
        
        stack.Push(start);

        while (stack.Count > 0)
        {
            T current = stack.Pop();
            
            if(isGoalReached(current)) break;

            if(visited.Add(current))
            {
                foreach (T next in graph.Neighbors(current))
                {
                    stack.Push(next);
                    from.TryAdd(next, current);
                }
            }
        }

        return from;
    }
}