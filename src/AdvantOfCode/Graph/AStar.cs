using System.Drawing;
using System.Numerics;

namespace AdventOfCode.Graph;

public class AStar
{
    public static int DistanceManhattan(Point a, Point b)
    {
        int dx = b.X - a.X;
        int dy = b.Y - a.Y;
        return Math.Abs(dx) + Math.Abs(dy);
    }

    public static (Dictionary<Point, Point>, Dictionary<Point, int>) SearchWithManhattan(IWeightedGraph<Point, int> graph,
        Point start, Point goal)
        => SearchFrom(graph, start, goal, DistanceManhattan);

    public static (Dictionary<T, T>, Dictionary<T, TCosts>) SearchFrom<T, TCosts>(IWeightedGraph<T, TCosts> graph,
        T start, T goal, Func<T, T, TCosts> heuristic)
        where T : notnull
        where TCosts : INumber<TCosts>
        => SearchFrom(graph, start, goal, current => goal.Equals(current), heuristic);
    
    public static (Dictionary<T, T>, Dictionary<T, TCosts>) SearchFrom<T, TCosts>(IWeightedGraph<T, TCosts> graph, 
        T start, T goal, Func<T, bool> goalReached, Func<T, T, TCosts> heuristic) 
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

            if (goalReached(current)) break;
            
            foreach (T next in graph.Neighbors(current))
            {
                TCosts newCosts = costs[current] + graph.Cost(current, next);
                if(!costs.ContainsKey(next) || newCosts < costs[next])
                {
                    costs[next] = newCosts;
                    var priority = newCosts + heuristic(next, goal);
                    from[next] = current;
                    frontier.Enqueue(next, priority);
                }
            }
        }

        return (from, costs);
    }
}