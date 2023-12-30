using System.Drawing;
using AdventOfCode.Graph;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day10;

[DayInfo(2023, 10)]
public class Solution10 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();

        char[,] grid = input.ToGrid();
        PipeGrid pipeGrid = new PipeGrid(grid);

        Point s = FindChar(grid, 'S');

        grid[s.X, s.Y] = '|'; // manual
        Dictionary<Point, Point> distances = BFS.SearchToGoalFunc(pipeGrid, s, _=> false);

        Point endPoint = distances.Keys.LastOrDefault();
        Point current = endPoint;
        int steps = 0;
        while (current != s)
        {
            current = distances[current];
            steps++;
        }

        int enclosed = 0;
        char[,] displayGrid = new char[pipeGrid.Width, pipeGrid.Height];
        for (int y = 0; y < pipeGrid.Height; y++)
        {
            int pipecrossings = 0;
            for (int x = 0; x < pipeGrid.Width; x++)
            {
                char output;
                Point c = new Point(x, y);
                if (distances.TryGetValue(c, out _))
                {
                    // part of main loop
                    char p = grid[x, y];
                    output = p switch
                    {
                        '|' => '║',
                        '-' => '═',
                        'L' => '╚',
                        'J' => '╝',                        
                        '7' => '╗',                        
                        'F' => '╔',
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    bool mainEntry = p switch
                    {
                        '|' => true,
                        'L' => true,
                        'J' => true,
                        _ => false,
                    };
                    if (mainEntry)
                    {
                        pipecrossings++;
                    }
                }
                else
                {
                    bool inside = (pipecrossings % 2) == 1;
                    output = inside ? 'I' : 'O';
                    if (inside)
                    {
                        enclosed++;
                    }
                }

                displayGrid[x, y] = output;
            }
        }

        if (false)
        {
            pipeGrid.DrawGrid(distances, s, endPoint);
            RectValueGrid<char> rectValueGrid = new(displayGrid);
            rectValueGrid.Draw();
        }

        return steps + "\n" + enclosed; 
    }

    private Point FindChar(char[,] grid, char s)
    {
        for (int x = 0; x < grid.Width(); x++)
        {
            for (int y = 0; y < grid.Heigth(); y++)
            {
                if (grid[x, y] == 'S')
                {
                    return new Point(x, y);
                }
            }
        }

        throw new ArgumentOutOfRangeException(nameof(s), "Char not in grid");
    }

    private class PipeGrid : IRectGrid<Point>
    {
        private readonly char[,] _grid;
        private readonly Dictionary<char, Size[]> pipeConnections = new()
        {
            { '|', [Sizes.Down, Sizes.Up] },
            { '-', [Sizes.Right, Sizes.Left] },
            { 'L', [Sizes.Right, Sizes.Up] },
            { 'J', [Sizes.Left, Sizes.Up] },
            { '7', [Sizes.Left, Sizes.Down] },
            { 'F', [Sizes.Right, Sizes.Down] },
            { '.', [] },
            { 'S', [] }, // unknown
        };

        public PipeGrid(char[,] grid)
        {
            _grid = grid;
            Width = grid.Width();
            Height = grid.Heigth();
        }
        
        public int Width { get; }
        public int Height { get; }
    
        public IEnumerable<Point> Neighbors(Point node)
        {
            char pipe = _grid[node.X, node.Y];

            foreach (var neighbor in pipeConnections[pipe])
            {
                Point result = node + neighbor;
                if (InRange(result))
                {
                    yield return result;
                }
            }
        }

        private bool InRange(Point p) => p.X >= 0 && p.X < Width &&
                                         p.Y >= 0 && p.Y < Height;
    }
}    