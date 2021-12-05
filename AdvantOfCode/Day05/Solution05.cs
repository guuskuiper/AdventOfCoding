namespace AdventOfCode.Day05;

public class Solution05 : Solution
{
    private int[][]? _oceanFloor;
    private List<OceanLine> _oceanlines = new ();
    private bool onlyHorizontalVertical = true;

    public class OceanLine
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
        
        int overlapCount = Solve(true);
        int overlapCountB = Solve(false);
        
        return overlapCount + "\n" + overlapCountB;
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
        var arrowFrom = arrow[0].Split(',');
        var arrowTo = arrow[1].Split(',');

        var oceanLine = new OceanLine()
        {
            From = new Point
            {
                X = int.Parse(arrowFrom[0]),
                Y = int.Parse(arrowFrom[1])
            },
            To = new Point
            {
                X = int.Parse(arrowTo[0]),
                Y = int.Parse(arrowTo[1])
            }
        };
        
        _oceanlines.Add(oceanLine);
    }

    private void CreateOceanFloor()
    {
        int maxFromX = _oceanlines.Select(x => x.From.X).Max();
        int maxToX = _oceanlines.Select(x => x.To.X).Max();
        int sizeX = Math.Max(maxFromX, maxToX) + 1;
        
        int maxFromY = _oceanlines.Select(x => x.From.Y).Max();
        int maxToY = _oceanlines.Select(x => x.To.Y).Max();
        int sizeY = Math.Max(maxFromY, maxToY) + 1;

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

    private void DrawLine(OceanLine line)
    {
        if (line.From.X == line.To.X)
        {
            int x = line.From.X;
            int yMin = Math.Min(line.From.Y, line.To.Y);
            int yMax = Math.Max(line.From.Y, line.To.Y);

            for (int y = yMin; y <= yMax; y++)
            {
                _oceanFloor[x][y]++;
            }
        }
        else if (line.From.Y == line.To.Y)
        {
            int y = line.From.Y;
            int xMin = Math.Min(line.From.X, line.To.X);
            int xMax = Math.Max(line.From.X, line.To.X);

            for (int x = xMin; x <= xMax; x++)
            {
                _oceanFloor[x][y]++;
            }
        }
        else if(!onlyHorizontalVertical)
        {
            //45 degree
            int sx = Math.Sign(line.To.X - line.From.X);
            int sy = Math.Sign(line.To.Y - line.From.Y);

            int points = Math.Abs(line.To.X - line.From.X) + 1;

            for (int i = 0; i < points; i++)
            {
                int x = line.From.X + i * sx;
                int y = line.From.Y + i * sy;
                
                _oceanFloor[x][y]++;
            }
        }
    }

    private int CountOverlap()
    {
        int count = 0;
        foreach (var line in _oceanFloor)
        {
            foreach (var tile in line)
            {
                if (tile > 1) count++;
            }
        }

        return count;
    }
}
