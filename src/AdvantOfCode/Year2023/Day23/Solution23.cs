namespace AdventOfCode.Year2023.Day23;

[DayInfo(2023, 23)]
public class Solution23 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        char[,] grid = input.ToGrid();
        IslandGrid island = new(grid);
        Point start = new(1, 0);
        Point end = new(grid.Width() - 2, grid.Heigth()-1);
        int length = FindLongest(island, start, end);

        return length + "\n";
    }
    
    private int FindLongest(IslandGrid grid, Point start, Point end)
    {
        IGraph<Point> dag = CreateDag(grid, start);
        List<Point> sorted = TopologicalSort.Sort(dag, start);

        Dictionary<Point, int> costs = [];
        costs[start] = 0;

        foreach (Point point in sorted)
        {
            int cost = costs[point];
            foreach (Point neighbor in dag.Neighbors(point))
            {
                if (costs.TryGetValue(neighbor, out int costNeighbor))
                {
                    if (costNeighbor < cost + 1)
                    {
                        costs[neighbor] = cost + 1;
                    }
                }
                else
                {
                    costs[neighbor] = cost + 1;
                }
            }
        }

        return costs[end];
    }

    private IGraph<Point> CreateDag(IslandGrid grid, Point start)
    {
        Dag dag = new();
        AQueue<Point> frontier = [start];
        HashSet<Point> nodes = [start];

        while (!frontier.Empty)
        {
            Point current = frontier.Get();

            foreach (Point next in grid.Neighbors(current))
            {
                if(!grid.IsInDirectionPassable(current, next)) continue;
                
                if(!nodes.Contains(next))
                {
                    frontier.Add(next);
                    nodes.Add(next);
                    dag.AddEdge(current, next);
                }
                else
                {
                    char c = grid[current];
                    if (c != '.')
                    {
                        dag.AddEdge(current, next);
                    }
                }
            }
        }

        return dag;
    }

    private class Dag : IGraph<Point>
    {
        private readonly Dictionary<Point, List<Point>> _neighbours = []; 

        public void AddEdge(Point from, Point to)
        {
            List<Point> list = _neighbours.GetOrCreate(from);
            list.Add(to);
        }
        
        public IEnumerable<Point> Neighbors(Point node)
        {
            List<Point>? neightbors;
            if(_neighbours.TryGetValue(node, out neightbors))
            {
            }
            return neightbors ?? [];
        }
    }

    private class IslandGrid : IRectGrid<Point>, IValueGrid<char>
    {
        private readonly char[,] _grid;
        public IslandGrid(char[,] grid)
        {
            _grid = grid;
            Width = grid.Width();
            Height = grid.Heigth();
        }
        
        public int Width { get; }
        public int Height { get; }

        public IEnumerable<Point> Neighbors(Point node)
        {
            foreach (var neighbor in SquareGrid.SquareNeightbors)
            {
                Point result = node + neighbor;

                if (InRange(result))
                {
                    if(IsForest(result)) continue;
                    // if(!IsValidHill(node, result)) continue;
                    // if(IsOppositeHill(node, result)) continue; // wrong direction
                    yield return result;
                }
            }
        }

        public bool IsInDirectionPassable(Point from, Point to)
        {
            bool passable = true;
            if (!IsValidHill(from, to))
            {
                passable = false;
            }
            if (IsOppositeHill(from, to))
            {
                passable = false;
            }

            return passable;
        }

        private bool IsForest(Point p) => this[p] == '#';

        private bool IsValidHill(Point hill, Point destination)
        {
            char c = this[hill];
            Size direction = c switch
            {
                '>' => new Size(1, 0),
                '<' => new Size(-1, 0),
                '^' => new Size(0, -1),
                'v' => new Size(0, 1),
                _ => Size.Empty
            };
            Point hillResult = hill + direction;
            return direction == Size.Empty || destination == hillResult;
        }
        
        private bool IsOppositeHill(Point current, Point destination)
        {
            char c = this[destination];
            Size direction = c switch
            {
                '>' => new Size(1, 0),
                '<' => new Size(-1, 0),
                '^' => new Size(0, -1),
                'v' => new Size(0, 1),
                _ => Size.Empty
            };
            Point hillResult = destination + direction;
            return current == hillResult;
        }

        public char this[Point p] => _grid[p.X, p.Y];

        private bool InRange(Point p) => p.X >= 0 && p.X < Width &&
                                         p.Y >= 0 && p.Y < Height;
    }
}    