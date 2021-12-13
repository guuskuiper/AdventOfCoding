using System.Text;

namespace AdventOfCode.Day13;

public class Solution13 : Solution
{
    private record Point(int X, int Y);

    private readonly List<Point> _points = new ();
    private readonly List<Point> _fold = new();

    private bool[,] _paper;
    private int _sizeX;
    private int _sizeY;
    
    
    public string Run()
    {
        var lines = InputReader.ReadFileLines();

        ParseLines(lines);
        CreatePaper();
        Fold(0, 1);
        long A = Count();
        Fold(1);
        string B = Print();
        
        return A + "\n" + B;
    }
    
    private void Fold(int offset, int count = -1)
    {
        var skipped = _fold.Skip(offset);

        IEnumerable<Point> counted = count > 0 ? skipped.Take(count) : skipped;
        foreach (var fold in counted)
        {
            Fold(fold);
        }
    }

    private void Fold(Point fold)
    {
        if (fold.Y < 0)
        {
            _sizeX = fold.X;
            for (int y = 0; y < _sizeY; y++)
            {
                for (int x = 0; x < _sizeX; x++)
                {
                    int foldedX = fold.X + (fold.X - x);
                    _paper[x, y] |= _paper[foldedX, y];
                }
            }
        }
        else
        {
            _sizeY = fold.Y;
            for (int x = 0; x < _sizeX; x++)
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    int foldedY = fold.Y + (fold.Y - y);
                    _paper[x, y] |= _paper[x, foldedY];
                }
            }
        }
    }

    private void CreatePaper()
    {
        _sizeX = 1 + 2 * _fold.First(p => p.Y < 0).X;
        _sizeY = 1 + 2 * _fold.First(p => p.X < 0).Y;
        _paper = new bool[_sizeX, _sizeY];

        foreach (var point in _points)
        {
            _paper[point.X, point.Y] = true;
        }
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
        var pos = line.Split(',');
        if (pos.Length == 2)
        {
            int x = Int32.Parse(pos[0]);
            int y = Int32.Parse(pos[1]);
            _points.Add(new Point(x, y));
        }
        else
        {
            var fold = line.Split();
            var foldXY = fold[2].Split('=');
            bool isX = foldXY[0] == "x";
            int x = isX ? Int32.Parse(foldXY[1]) : -1;
            int y = !isX ? Int32.Parse(foldXY[1]) : -1;
            _fold.Add(new Point(x, y));
            
        }
    }
    
    private long Count()
    {
        long count = 0;
        for (int y = 0; y < _sizeY; y++)
        {
            for (int x = 0; x < _sizeX; x++)
            {
                if (_paper[x, y])
                {
                    count++;
                }
            }
        }

        return count;
    }
    
    private string Print()
    {
        StringBuilder sb = new();
        for (int y = 0; y < _sizeY; y++)
        {
            for (int x = 0; x < _sizeX; x++)
            {
                sb.Append(_paper[x, y] ? '#' : ' ');
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
