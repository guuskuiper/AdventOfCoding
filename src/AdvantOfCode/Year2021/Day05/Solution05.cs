namespace AdventOfCode.Year2021.Day05;

public class Solution05 : Solution
{
    private int[][]? _oceanFloor;
    private List<Line> _oceanlines = new ();
    private bool onlyHorizontalVertical = true;

    public class Line
    {
        public Point From { get; set; }
        public Point To { get; set; }
    }
    
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    
    public string Run()
    {
        var lines = InputReader.ReadFileLines();
        ParseLines(lines);
        
        int overlapCountA = Solve(true);
        int overlapCountB = Solve(false);
        
        return overlapCountA + "\n" + overlapCountB;
    }

    private int Solve(bool horizontalVerticalOnly)
    {
        onlyHorizontalVertical = horizontalVerticalOnly;
        CreateOceanFloor();
        DrawLines();
        return CountOverlap();
    }

    private void ParseLines(List<string> lines)
    {
        foreach (var line in lines)
        {
            ParseLine(line);
        }
    }

    private void ParseLine(string line)
    {
        var arrow = line.Split("->");
        var from = ParsePoint(arrow[0]);
        var to = ParsePoint(arrow[1]);

        var oceanLine = new Line
        {
            From = from,
            To = to
        };
        
        _oceanlines.Add(oceanLine);
    }

    private Point ParsePoint(string point)
    {
        var split = point.Split(',');
        return new Point
        {
            X = int.Parse(split[0]),
            Y = int.Parse(split[1])
        };
    }

    private void CreateOceanFloor()
    {
        var points = _oceanlines.SelectMany(x => new[] { x.From, x.To }).ToList();
        int sizeX = 1 + points.Max(p => p.X);
        int sizeY = 1 + points.Max(p => p.Y);

        _oceanFloor = new int[sizeX][];
        for (var index = 0; index < _oceanFloor.Length; index++)
        {
            _oceanFloor[index] = new int[sizeY];
        }
    }

    private void DrawLines()
    {
        foreach (var oceanline in _oceanlines)
        {
            DrawLine(oceanline);
        }
    }

    private void DrawLine(Line line)
    {
        if (line.From.X == line.To.X)
        {
            DrawDiagonal(line);
        }
        else if (line.From.Y == line.To.Y)
        {
            DrawDiagonal(line);
        }
        else if(!onlyHorizontalVertical)
        {
            DrawDiagonal(line);
        }
    }

    private void DrawDiagonal(Line line)
    {
        int sx = Math.Sign(line.To.X - line.From.X);
        int sy = Math.Sign(line.To.Y - line.From.Y);

        int points = 1 + (sx != 0 ? Math.Abs(line.To.X - line.From.X) : Math.Abs(line.To.Y - line.From.Y));

        for (int i = 0; i < points; i++)
        {
            int x = line.From.X + i * sx;
            int y = line.From.Y + i * sy;

            _oceanFloor![x][y]++;
        }
    }

    private int CountOverlap()
    {
        int count = 0;
        foreach (var line in _oceanFloor!)
        {
            foreach (var tile in line)
            {
                if (tile > 1) count++;
            }
        }

        return count;
    }
}
