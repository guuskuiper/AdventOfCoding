using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day03;

[DayInfo(2023, 03)]
public class Solution03 : Solution
{
    private record Point(int X, int Y);

    private record Gear(int Left, int Right);

    private readonly List<int> _numbers = new();
    private readonly Dictionary<Point, Gear> _gears = new();
    private char[,] _grid = new char[0,0];
    private int Width => _grid.GetLength(0);
    private int Heigth => _grid.GetLength(1);

    public string Run()
    {
        string[] input = this.ReadLines();

        _grid = ToGrid(input);

        FindAdjecentNumbers();
        
        int adjecentNumbers = _numbers.Sum();
        int gearRatio = _gears.Values.Select(g => g.Left * g.Right).Sum();
        
        return adjecentNumbers + "\n" + gearRatio; // >548403
    }

    private void FindAdjecentNumbers()
    {
        for (int y = 0; y < Heigth; y++)
        {
            string number = "";
            for (int x = 0; x < Width; x++)
            {
                char c = _grid[x, y];
                if (char.IsDigit(c))
                {
                    number += c;
                }
                else
                {
                    if (number.Length > 0)
                    {
                        AddValidNumber(number, x, y);
                    }

                    number = "";
                }
            }

            if (number.Length > 0)
            {
                int x = Width - 1;
                AddValidNumber(number, x, y);
            }
        }
    }

    private void AddValidNumber(string number, int x, int y)
    {
        int value = int.Parse(number);
        int startX = x - number.Length;
        bool valid = ContainsSymbols(startX, x - 1, y, value);
        if (valid)
        {
            _numbers.Add(value);
        }
    }

    private bool ContainsSymbols(int xMin, int xMax, int yLine, int value)
    {
        int x1 = Math.Max(0, xMin - 1);
        int x2 = Math.Min(Width - 1, xMax + 1);

        int y1 = Math.Max(0, yLine - 1);
        int y2 = Math.Min(Heigth - 1, yLine + 1);

        bool containsSymbol = false;
        for (int y = y1; y <= y2; y++)
        {
            for (int x = x1; x <= x2; x++)
            {
                char c = _grid[x, y];
                containsSymbol |= IsSymbol(c);
                if (c == '*')
                {
                    AddGear(value, x, y);
                }
            }
        }

        return containsSymbol;
    }

    private void AddGear(int value, int x, int y)
    {
        Point p = new Point(x, y);
        if (_gears.TryGetValue(p, out Gear? gear))
        {
            if (gear.Right > 0) throw new Exception("Triple gear?");
        }
        Gear updated = gear is null ? new Gear(value, 0) : gear with { Right = value };
        _gears[p] = updated;
    }

    private bool IsSymbol(char c) => !char.IsDigit(c) && c != '.';

    private char[,] ToGrid(string[] lines)
    {
        int width = lines[0].Length;
        int height = lines.Length;

        char[,] grid = new char[width, height];
        for (int y = 0; y < height; y++)
        {
            var line = lines[y];
            for (int x = 0; x < width; x++)
            {
                grid[x,y] = line[x];
            }
        }

        return grid;
    }
}    