using System.Drawing;

namespace AdventOfCode.Graph;

public static class BFS
{
    public static Dictionary<T, T> SearchFrom<T>(IGraph<T> graph, T start, T? goal) where T : notnull
    {
        var frontier = new AQueue<T>();
        frontier.Add(start);

        Dictionary<T, T> from = new()
        {
            [start] = start
        };

        while (!frontier.Empty)
        {
            var current = frontier.Get();

            if (goal != null && current.Equals(goal))
            {
                break;
            }
            
            foreach (var next in graph.Neighbors(current))
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
    
    public static Dictionary<T, T> SearchToGoalFunc<T>(IGraph<T> graph, T start, Func<T, bool> isGoalReached) where T : notnull
    {
        var frontier = new AQueue<T>();
        frontier.Add(start);

        Dictionary<T, T> from = new()
        {
            [start] = start
        };

        while (!frontier.Empty)
        {
            var current = frontier.Get();

            if (isGoalReached(current))
            {
                break;
            }
            
            foreach (var next in graph.Neighbors(current))
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