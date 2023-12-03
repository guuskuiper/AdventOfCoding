using System.Drawing;
using AdventOfCode.Extensions;
using AdventOfCode.Graph;

namespace AdventOfCode.Year2023.Day03;

[DayInfo(2023, 03)]
public class Solution03 : Solution
{
    private record Gear
    {
        public readonly List<int> Numbers = [];
    }

    private readonly List<int> _numbers = [];
    private readonly Dictionary<Point, Gear> _gears = new();
    private RectValueGrid<char> _rectGrid = new(new char[0,0]);

    public string Run()
    {
        string[] input = this.ReadLines();

        _rectGrid = input.ToRecGrid(SquareGrid.DiagonalNeightbors);
        //_rectGrid.Draw();

        FindAdjacentNumbers();
        
        int adjacentNumbers = _numbers.Sum();
        int gearRatio = _gears.Values
            .Where(g => g.Numbers.Count == 2)
            .Select(g => g.Numbers.Aggregate(1, (accumulate, value) => accumulate * value))
            .Sum();
        
        return adjacentNumbers + "\n" + gearRatio;
    }

    private void FindAdjacentNumbers()
    {
        for (int y = 0; y < _rectGrid.Height; y++)
        {
            string number = "";
            for (int x = 0; x < _rectGrid.Width; x++)
            {
                Point p = new(x, y);
                char c = _rectGrid[p];
                if (char.IsDigit(c))
                {
                    number += c;
                }
                else
                {
                    AddValidNumber(number, x, y);
                    number = "";
                }
            }
            AddValidNumber(number, _rectGrid.Width - 1, y);
        }
    }

    private void AddValidNumber(string number, int x, int y)
    {
        if(number.Length == 0) return;
        
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
        bool containsSymbol = false;
        HashSet<Point> visitedGears = new();
        for (int x = xMin; x <= xMax; x++)
        {
            Point p = new Point(x, yLine);
            foreach (var neighbor in _rectGrid.Neighbors(p))
            {
                char c = _rectGrid[neighbor];
                containsSymbol |= IsSymbol(c);
                if (c == '*' && !visitedGears.Contains(neighbor))
                {
                    AddGear(value, neighbor);
                    visitedGears.Add(neighbor);
                }
            }
        }

        return containsSymbol;
    }

    private void AddGear(int value, Point p)
    {
        Gear g = _gears.GetOrCreate(p);
        g.Numbers.Add(value);
    }

    private bool IsSymbol(char c) => !char.IsDigit(c) && c != '.';
}    