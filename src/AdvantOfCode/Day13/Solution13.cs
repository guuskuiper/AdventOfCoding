using System.Text;

namespace AdventOfCode.Day13;

public class Solution13 : Solution
{
    private record Point(int X, int Y);

    private List<Point> _points = new ();
    private List<Point> _fold = new();

    private bool[,] _paper;
    
    public string Run()
    {
        var lines = InputReader.ReadFileLines();

        ParseLines(lines);
        CreatePaper();
        Fold(_fold[0]);
        long A = Count();
        Fold(_fold.Skip(1).ToList());
        string B = Print();
        
        return A + "\n" + B;
    }
    
    private void Fold(List<Point> folds)
    {
        foreach (var fold in folds)
        {
            Fold(fold);
        }
    }

    private void Fold(Point fold)
    {
        bool[,] foldedPaper;
        if (fold.Y < 0)
        {
            int sizeY = _paper.GetLength(1);
            foldedPaper = new bool[fold.X,sizeY];
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < fold.X; x++)
                {
                    int foldedX = fold.X + (fold.X - x);
                    foldedPaper[x, y] = _paper[x, y] || _paper[foldedX, y];
                }
            }
        }
        else
        {
            int sizeX = _paper.GetLength(0);
            foldedPaper = new bool[sizeX, fold.Y];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < fold.Y; y++)
                {
                    int foldedY = fold.Y + (fold.Y - y);
                    foldedPaper[x, y] = _paper[x, y] || _paper[x, foldedY];
                }
            }
        }

        _paper = foldedPaper;
    }

    private void CreatePaper()
    {
        int maxX = _points.Max(p => p.X);
        int maxY = _points.Max(p => p.Y);
        _paper = new bool[maxX + 1, maxY + 1 + 1];

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
            var foldxy = fold[2].Split('=');
            bool isX = foldxy[0] == "x";
            int x = isX ? Int32.Parse(foldxy[1]) : -1;
            int y = !isX ? Int32.Parse(foldxy[1]) : -1;
            _fold.Add(new Point(x, y));
            
        }
    }
    
    private long Count()
    {
        long count = 0;
        foreach (var element in _paper)
        {
            if (element)
            {
                count++;
            }
        }

        return count;
    }
    
    private string Print()
    {
        StringBuilder sb = new();
        for (int y = 0; y < _paper.GetLength(1); y++)
        {
            for (int x = 0; x < _paper.GetLength(0); x++)
            {
                sb.Append(_paper[x, y] ? '#' : ' ');
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
