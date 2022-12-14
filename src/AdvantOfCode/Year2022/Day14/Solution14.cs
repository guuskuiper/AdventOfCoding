using System.Drawing;
using System.Text;
using AdventOfCode.Extentions;

namespace AdventOfCode.Year2022.Day14;

public class Solution14 : Solution
{
    private class Cave
    {
        public static readonly Point SandStart = new Point(500, 0);

        private const char Air = '.';
        private const char Rock = '#';
        private const char Sand = 'o';
        private const char SandSpawn = '+';
        
        
        private readonly int _xmin;
        private readonly int _xmax;
        private readonly int _ymin;
        private readonly int _ymax;
        private readonly char[,] _cave; 

        public Cave(int xmin, int xmax, int ymin, int ymax)
        {
            _xmin = xmin;
            _xmax = xmax;
            _ymin = Math.Min(0, ymin);
            _ymax = ymax;
            _cave = new char[_xmax - _xmin + 1, _ymax - _ymin + 1];
        }

        public void Initialize()
        {
            for (int x = 0; x < _cave.GetLength(0); x++)
            {
                for (int y = 0; y < _cave.GetLength(1); y++)
                {
                    _cave[x, y] = Air;
                }
            }
        }

        public void SetRock(Point p)
        {
            Set(p, Rock);
        }
        
        public void SetSandRest(Point p)
        {
            Set(p, Sand);
        }

        private void Set(Point p, char c)
        {
            _cave[p.X - _xmin, p.Y - _ymin] = c;
        }

        public bool IsAir(Point p)
        {
            return _cave[p.X - _xmin, p.Y - _ymin] == Air;
        }

        public void Print()
        {
            StringBuilder sb = new();
            //for(int y = _cave.GetLength(1) - 1; y >= 0; y--)
            for(int y = 0; y < _cave.GetLength(1); y++)
            {
                for (int x = 0; x < _cave.GetLength(0); x++)
                {
                    sb.Append(_cave[x, y]);
                }

                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }

        public void AddSandSpawn()
        {
            Set(SandStart, SandSpawn);
        }
    }

    private static Size _downOffset = new Size(0, 1);
    private static Size _leftDownOffset = new Size(-1, 1);
    private static Size _rightDownOffset = new Size(1, 1);
    
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        var rocks = Parse(lines);

        int xMin = rocks.SelectMany(x => x).Min(p => p.X);
        int xMax = rocks.SelectMany(x => x).Max(p => p.X);
        int yMin = rocks.SelectMany(x => x).Min(p => p.Y);
        int yMax = rocks.SelectMany(x => x).Max(p => p.Y);
        
        int sandCount = PartA(rocks, xMin, xMax, yMin, yMax);
        int sandCountB = PartB(rocks, xMin, xMax, yMin, yMax);
        
        return sandCount + "\n" + sandCountB;
    }

    private int PartA(List<List<Point>> rocks, int xMin, int xMax, int yMin, int yMax)
    {
        int yGoal = yMax + 1;
        Cave cave = new Cave(xMin - 1, xMax + 1, yMin, yGoal);
        cave.Initialize();

        foreach (var rock in rocks)
        {
            DrawLines(cave, rock);
        }

        cave.AddSandSpawn();
        
        int sandCount = 0;
        while (ProduceSand(cave, yGoal))
        {
            sandCount++;
        }
        
        //cave.Print();

        return sandCount;
    }

    private int PartB(List<List<Point>> rocks, int xMin, int xMax, int yMin, int yMax)
    {
        int yFloor = yMax + 2;
        int xMinB = Cave.SandStart.X - yFloor;
        int xMaxB = Cave.SandStart.X + yFloor;
        Cave caveB = new Cave(xMinB, xMaxB, 0, yFloor);
        caveB.Initialize();

        List<Point> bottom = new List<Point>{ new (xMinB, yFloor), new (xMaxB, yFloor) };

        DrawLines(caveB, bottom);
        foreach (var rock in rocks)
        {
            DrawLines(caveB, rock);
        }
        
        int sandCount = 1;
        while (ProduceSandB(caveB))
        {
            sandCount++;
        }
        
        //caveB.Print();
        return sandCount;
    }
    
    private bool ProduceSandB(Cave cave)
    {
        Point current = Cave.SandStart;
        bool moved = true;
        while (moved)
        {
            Point down = current + _downOffset;
            Point downLeft = current + _leftDownOffset;
            Point downRight = current + _rightDownOffset;
            
            if (cave.IsAir(down))
            {
                current = down;
            }
            else if (cave.IsAir(downLeft))
            {
                current = downLeft;
            }
            else if (cave.IsAir(downRight))
            {
                current = downRight;
            }
            else
            {
                moved = false;
            }
        }

        cave.SetSandRest(current);
        return current != Cave.SandStart;
    }

    private bool ProduceSand(Cave cave, int yGoal)
    {
        bool inside = true;
        Point current = Cave.SandStart;
        bool moved = true;
        while (moved)
        {
            Point down = current + _downOffset;
            Point downLeft = current + _leftDownOffset;
            Point downRight = current + _rightDownOffset;
            
            if (cave.IsAir(down))
            {
                current = down;
            }
            else if (cave.IsAir(downLeft))
            {
                current = downLeft;
            }
            else if (cave.IsAir(downRight))
            {
                current = downRight;
            }
            else
            {
                moved = false;
            }
           
            if (current.Y == yGoal)
            {
                inside = false;
                break;
            }
        }

        cave.SetSandRest(current);
        return inside;
    }

    private void DrawLines(Cave cave , List<Point> points)
    {
        Point prev = points[0];
        cave.SetRock(prev);
        foreach (var point in points.Skip(1))
        {
            Size dir = new Size(Math.Sign(point.X - prev.X), Math.Sign(point.Y - prev.Y));
            
            while (prev != point)
            {
                Point current = prev + dir;
                cave.SetRock(current);
                prev = current;
            }
        }
    }

    private List<List<Point>> Parse(IEnumerable<string> lines)
    {
        List<List<Point>> rocks = new();
        foreach (var line in lines)
        {
            List<Point> rock = new List<Point>();
            string[] split = line.Split(" -> ");
            foreach (var s in split)
            {
                string[] ints = s.Split(',');
                int x = int.Parse(ints[0]);
                int y = int.Parse(ints[1]);
                Point p = new Point(x, y);
                rock.Add(p);
            }
            rocks.Add(rock);
        }

        return rocks;
    }
}
