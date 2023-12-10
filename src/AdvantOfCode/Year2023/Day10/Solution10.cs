using System.Drawing;
using AdventOfCode.Graph;
using AdventOfCode.Extensions;
using AdventOfCode.Year2021.Day17;

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
                    output = '.'; //p;
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
            { '|', [new(0, 1), new(0, -1)] },
            { '-', [new(1, 0), new(-1, 0)] },
            { 'L', [new(1, 0), new(0, -1)] },
            { 'J', [new(-1, 0), new(0, -1)] },
            { '7', [new(-1, 0), new(0, 1)] },
            { 'F', [new(1, 0), new(0, 1)] },
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