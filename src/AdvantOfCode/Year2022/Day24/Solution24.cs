using System.Drawing;
using AdventOfCode.Extensions;
using AdventOfCode.Graph;

namespace AdventOfCode.Year2022.Day24;

public class Solution24 : Solution
{
    private record TimePoint(Point Point, int Time);
    private record Blizzard(Point Point, char Direction);
    private class BlizzardGraph : IGraph<TimePoint>, IRectGrid<Point>
    {
        private static readonly Size Empty = Size.Empty;
        private static readonly Size North = new(0, -1);
        private static readonly Size South = new(0, 1);
        private static readonly Size West = new(-1, 0);
        private static readonly Size East = new(1, 0);
        
        private static readonly Size[] Directions = { Empty, North, South, West, East};
        
        private readonly HashSet<Point>[] _occupied;
        private readonly Size _bounds;

        public BlizzardGraph(Blizzard[][] blizzards, Size bounds)
        {
            _bounds = bounds;
            End = Point.Empty + bounds + South;
            _occupied = new HashSet<Point>[blizzards.Length];
            for (int i = 0; i < blizzards.Length; i++)
            {
                Blizzard[] bs = blizzards[i];
                HashSet<Point> pts = new();
                foreach (Blizzard blizzard in bs)
                {
                    pts.Add(blizzard.Point);
                }
                _occupied[i] = pts;
            }
        }
        
        public static readonly Point Start = new (1, 0);
        public readonly Point End;
        
        public IEnumerable<TimePoint> Neighbors(TimePoint node)
        {
            int nextTime = node.Time + 1;
            int timeIndex = nextTime % _occupied.Length;

            foreach (Size direction in Directions)
            {
                Point target = node.Point + direction;
                
                if(!InRange(target)) continue;
                
                if(!_occupied[timeIndex].Contains(target))
                {
                    yield return new TimePoint(target, nextTime);
                }
            }
        }

        private bool InRange(Point p)
        {
            if (p == Start || p == End) return true;
            
            return p.X >= 1 && p.X <= _bounds.Width && 
                   p.Y >= 1 && p.Y <= _bounds.Height;
        }

        public int Width => _bounds.Width + 2;
        public int Height => _bounds.Height + 2;
        public IEnumerable<Point> Neighbors(Point node)
        {
            TimePoint p = new TimePoint(node, 0);
            foreach (TimePoint timePoint in Neighbors(p))
            {
                yield return timePoint.Point;
            }
        }
    }

    public string Run()
    {
        string example = """
            #.######
            #>>.<^<#
            #.<..<<#
            #>v.><>#
            #<^v^^>#
            ######.#
            """;
        // string[] lines = example.Split("\r\n");
        var lines = InputReader.ReadFileLinesArray();
        
        var blizzards = Parse(lines).ToArray();
        Size bounds = new(lines[0].Length - 2, lines.Length - 2);
        int lcm = MathGeneric.LCM(bounds.Width, bounds.Height);
        
        Blizzard[][] blizzardsMoves = new Blizzard[lcm][];
        for (int s = 0; s < lcm; s++)
        {
            blizzardsMoves[s] = Move(blizzards, s, bounds);
        }

        Blizzard[] repeatInitial = Move(blizzards, lcm, bounds);
        for (int i = 0; i < lcm; i++)
        {
            if (repeatInitial[i] != blizzards[i]) throw new Exception("Not repeated");
            if (blizzardsMoves[0][i] != blizzards[i]) throw new Exception("Not repeated");
        }
        
        BlizzardGraph graph = new BlizzardGraph(blizzardsMoves, bounds);
        
        TimePoint start = new TimePoint(BlizzardGraph.Start, 0);
        bool ExitFound(TimePoint timePoint) => timePoint.Point == graph.End;

        var results = BFS.SearchToGoalFunc(graph, start, ExitFound);
        TimePoint exit = BestEndpoint(results, ExitFound);

        var path = TimePoints2Path(results, exit);
        GraphDraw.DrawGrid(graph, path, BlizzardGraph.Start, graph.End);

        bool EntranceFound(TimePoint timePoint) => timePoint.Point == BlizzardGraph.Start;
        var resultsBack = BFS.SearchToGoalFunc(graph, exit, EntranceFound);
        
        TimePoint entrance = BestEndpoint(resultsBack, EntranceFound);
        var finalResults = BFS.SearchToGoalFunc(graph, entrance, ExitFound);
        TimePoint final = BestEndpoint(finalResults, ExitFound);
        
        return exit.Time + "\n" + final.Time;
    }

    private Dictionary<Point, Point> TimePoints2Path(Dictionary<TimePoint, TimePoint> timePoints, TimePoint goal)
    {
        Dictionary<Point, Point> pts = new();

        TimePoint current = goal;
        while (true)
        {
            if (timePoints.TryGetValue(current, out TimePoint? parent) && parent.Time == current.Time - 1)
            {
                pts[current.Point] = parent.Point;
                current = parent;
            }
            else
            {
                break;
            }
        }
        
        return pts;
    }

    private TimePoint BestEndpoint(Dictionary<TimePoint, TimePoint> route, Func<TimePoint, bool> goal)
    {
        return route.Keys
            .Where(goal)
            .MinBy(x => x.Time)!;
    }

    private Blizzard[] Move(Blizzard[] initial, int steps, Size bounds)
    {
        int dx = steps % bounds.Width;
        int dxNeg = bounds.Width - dx;
        int dy = steps % bounds.Height;
        int dyNeg = bounds.Height - dy;
        Blizzard[] blizzards = new Blizzard[initial.Length];
        for (int i = 0; i < initial.Length; i++)
        {
            Blizzard b = initial[i];
            Point target = b.Direction switch
            {
                '>' => b.Point with { X = ((b.Point.X - 1 + dx) % bounds.Width) + 1 },
                '<' => b.Point with { X = ((b.Point.X - 1 + dxNeg) % bounds.Width) + 1 },
                'v' => b.Point with { Y = ((b.Point.Y - 1 + dy) % bounds.Height) + 1 },
                '^' => b.Point with { Y = ((b.Point.Y - 1 + dyNeg) % bounds.Height) + 1 },
                _ => throw new ArgumentOutOfRangeException()
            };
            blizzards[i] = b with { Point = target };
        }

        return blizzards;
    }
    
    private IEnumerable<Blizzard> Parse(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                char c = line[x];
                if (c is '>' or '<' or '^' or 'v')
                {
                    yield return new Blizzard(new Point(x, y), c);
                }
            }
        }
    }
}
 