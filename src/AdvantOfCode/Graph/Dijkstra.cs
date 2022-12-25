using System.Numerics;
namespace AdventOfCode.Graph;

public static class Dijkstra
{
    public static (Dictionary<T, T>, Dictionary<T, TCosts>) SearchFrom<T, TCosts>(IWeightedGraph<T, TCosts> graph, 
        T start, T? goal) 
        where T : notnull
        where TCosts : INumber<TCosts>
        =>  SearchGoal(graph, start, goal is null ? _ => false : current => goal.Equals(current));
    
    public static (Dictionary<T, T>, Dictionary<T, TCosts>) SearchGoal<T, TCosts>(IWeightedGraph<T, TCosts> graph, 
        T start, Func<T, bool> isGoalReached) 
        where T : notnull
        where TCosts : INumber<TCosts>
    {
        PriorityQueue<T, TCosts> frontier = new();
        frontier.Enqueue(start, TCosts.Zero);

        Dictionary<T, T> from = new() { [start] = start };
        Dictionary<T, TCosts> costs = new() { [start] = TCosts.Zero };

        while (frontier.Count != 0)
        {
            T current = frontier.Dequeue();
            
            if (isGoalReached(current)) break;
            
            foreach (T next in graph.Neighbors(current))
            {
                TCosts newCosts = costs[current] + graph.Cost(current, next);
                if(!costs.ContainsKey(next) || newCosts < costs[next])
                {
                    costs[next] = newCosts;
                    from[next] = current;
                    frontier.Enqueue(next, newCosts);
                }
            }
        }

        return (from, costs);
    }
}