using System.Drawing;

namespace AdventOfCode.Graph;

public static class BFS
{
    public static Dictionary<T, T> SearchFrom<T>(IGraph<T> graph, T start, T? goal)
        where T : notnull
        => SearchToGoalFunc(graph, start, goal is null ? _ => false : current => goal.Equals(current));
    
    public static Dictionary<T, T> SearchToGoalFunc<T>(IGraph<T> graph, T start, Func<T, bool> isGoalReached) where T : notnull
    {
        AQueue<T> frontier = new();
        frontier.Add(start);

        Dictionary<T, T> from = new()
        {
            [start] = start
        };

        while (!frontier.Empty)
        {
            T current = frontier.Get();

            if (isGoalReached(current)) break;
            
            foreach (T next in graph.Neighbors(current))
            {
                if(!from.ContainsKey(next))
                {
                    frontier.Add(next);
                    from[next] = current;
                }
            }
        }

        return from;
    }
}