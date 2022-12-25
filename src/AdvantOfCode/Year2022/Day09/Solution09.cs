using AdventOfCode.Extensions;

namespace AdventOfCode.Year2022.Day09;

public class Solution09 : Solution
{
    private record Move(char Direction, int Steps);

    private record Point(int X, int Y)
    {
        public bool IsNear(Point other)
        {
            Point diff = this - other;
            return Math.Abs(diff.X) <= 1 &&
                   Math.Abs(diff.Y) <= 1;
        }
        public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
    }
    private record Rect(Point Min, Point Max);

    private class World
    {
        private readonly Rect _bounds;
        private readonly bool[,] _visited;

        public World(Rect bounds)
        {
            _bounds = bounds;

            Point size = bounds.Max - bounds.Min;
            _visited = new bool[size.X + 1, size.Y + 1];
        }

        public void Visit(Point p)
        {
            Point offset = p - _bounds.Min;
            _visited[offset.X, offset.Y] = true;
        }

        public int CountVisited()
        {
            return _visited.ToEnumerable().Count(x => x);
        }
    }
    
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        var moves = Parse(lines);
        Rect bounds = BoundPoint(moves);
        int visited = Simulate(moves, bounds);
        int visited2 = Simulate2(moves, bounds, 10);
        return visited + "\n" + visited2;
    }
    
    private int Simulate2(IEnumerable<Move> moves, Rect bounds, int count)
    {
        World world = new World(bounds);
        Point[] knots = Enumerable.Repeat(new Point(0, 0), count).ToArray();
        world.Visit(knots[knots.Length - 1]);

        foreach (Move move in moves)
        {
            for (int i = 0; i < move.Steps; i++)
            {
                knots[0] = MoveStep(knots[0], move.Direction);
                for (int k = 1; k < knots.Length; k++)
                {
                    Follow(knots[k-1], ref knots[k]);
                }
                world.Visit(knots[knots.Length - 1]);
            }
        }

        return world.CountVisited();
    }

    private int Simulate(IEnumerable<Move> moves, Rect bounds)
    {
        World world = new World(bounds);
        Point H = new Point(0, 0);
        Point T = new Point(0,0);
        world.Visit(T);

        foreach (Move move in moves)
        {
            for (int i = 0; i < move.Steps; i++)
            {
                H = MoveStep(H, move.Direction);
                Follow(H, ref T);
                world.Visit(T);
            }
        }

        return world.CountVisited();
    }

    private void Follow(Point next, ref Point current)
    {
        if (!next.IsNear(current))
        {
            current = Follow(next, current);
        }
    }

    private Point Follow(Point next, Point current)
    {
        Point delta = next - current;
        Point vector = new Point(Math.Sign(delta.X), Math.Sign(delta.Y));
        return current + vector;
    }

    private Rect BoundPoint(IEnumerable<Move> moves)
    {
        Point current = new Point(0, 0);
        Point max = new Point(0, 0);
        Point min = new Point(0, 0);

        foreach (Move move in moves)
        {
            current = MovePoint(current, move);
            if (current.X > max.X)
            {
                max = max with { X = current.X };
            }
            else if (current.Y > max.Y)
            {
                max = max with { Y = current.Y };
            }
            else if (current.X < min.X)
            {
                min = min with { X = current.X };
            }
            else if (current.Y < min.Y)
            {
                min = min with { Y = current.Y };
            }
        }

        return new Rect(min, max);
    }

    private Point MoveStep(Point p, char direction)
    {
        return direction switch
        {
            'R' => p with { X = p.X + 1 },
            'L' => p with { X = p.X - 1 },
            'U' => p with { Y = p.Y + 1 },
            'D' => p with { Y = p.Y - 1 },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Point MovePoint(Point p, Move m)
    {
        return m.Direction switch
        {
            'R' => p with { X = p.X + m.Steps },
            'L' => p with { X = p.X - m.Steps },
            'U' => p with { Y = p.Y + m.Steps },
            'D' => p with { Y = p.Y - m.Steps },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private List<Move> Parse(IEnumerable<string> lines)
    {
        List<Move> moves = new();
        foreach (var line in lines)
        {
            string[] split = line.Split(' ');
            int step = int.Parse(split[1]);
            Move move = new Move(split[0][0], step);
            moves.Add(move);
        }

        return moves;
    }
}
