using System.Text;

namespace AdventOfCode.Year2021.Day25;

[DayInfo(2021, 25)]
public class Solution25 : Solution
{
    private char[,] _map;
    
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        ParseLines(lines);
        int A = Steps();        
        return A + "\n";
    }

    private int Steps()
    {
        int step = 0;
        bool moved;
        do
        {
            moved = StepInplace();
            step++;
        } while (moved);

        return step;
    }

    private bool Step()
    {
        bool moved = false;
        moved |= MoveEast();
        moved |= MoveSouth();
        return moved;
    }

    private bool StepInplace()
    {
        bool moved = false;
        moved |= MoveInPlaceEast(_map);
        moved |= MoveInPlaceSouth(_map);
        return moved;
    }
    
    private string PrintLine(char[,] map, int x = 0)
    {
        StringBuilder sb = new();
        for (int y = 0; y < map.GetLength(1); y++)
        {
            sb.Append(map[x, y]);
        }

        return sb.ToString();;
    }

    private bool MoveEast()
    {
        return MoveGeneral(
            (x, y) => _map[x, y] == '>' && _map[x, NextY(y)] == '.',
            (x, y) => Swap(ref _map[x, y], ref _map[x, NextY(y)])
            );
    }
    
    private bool MoveSouth()
    {
        return MoveGeneral(
            (x, y) => _map[x, y] == 'v' && _map[NextX(x), y] == '.',
            (x, y) => Swap(ref _map[x, y], ref _map[NextX(x), y])
        );
    }
    
    private int NextX(int x) => (x + 1) % _map.GetLength(0);
    private int NextY(int y) => (y + 1) % _map.GetLength(1);
    
    private static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }

    private bool MoveGeneral(Func<int, int, bool> condition, Action<int, int> move)
    {
        Queue<(int x, int y)> moves = new ();
        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                if (condition(x, y))
                {
                    moves.Enqueue((x, y));
                }
            }
        }

        bool moved = moves.Count > 0;
        
        while (moves.Count > 0)
        {
            var location = moves.Dequeue();
            move(location.x, location.y);
        }

        return moved;
    }
    
    private static bool MoveInPlaceEast(char[,] map)
    {
        bool moved = false;
        for (int x = 0; x < map.GetLength(0); x++)
        {
            int lastY = map.GetLength(1) - 1;
            char y0 = map[x, 0];
            char ylast = map[x, lastY];
            
            bool prevSwapped = false;
            for (int y = map.GetLength(1) - 1; y > 0 ; y--)
            {
                if (map[x, y - 1] == '>' && map[x, y] == '.' && !prevSwapped)
                {
                    moved = true;
                    Swap(ref map[x, y], ref map[x, y - 1]);
                    prevSwapped = true;
                }
                else
                {
                    prevSwapped = false;
                }
            }

            if(ylast == '>' && y0 == '.')
            {
                moved = true;
                Swap(ref map[x, lastY], ref map[x, 0]);
            }
        }

        return moved;
    }
    
    private static bool MoveInPlaceSouth(char[,] map)
    {
        bool moved = false;
        for (int y = 0; y < map.GetLength(1); y++)
        {
            int lastX = map.GetLength(0) - 1;
            char x0 = map[0, y];
            char xlast = map[lastX, y];
            
            bool prevSwapped = false;
            for (int x = map.GetLength(0) - 1; x > 0 ; x--)
            {
                if (map[x - 1, y] == 'v' && map[x, y] == '.' && !prevSwapped)
                {
                    moved = true;
                    Swap(ref map[x, y], ref map[x - 1, y]);
                    prevSwapped = true;
                }
                else
                {
                    prevSwapped = false;
                }
            }

            if(xlast == 'v' && x0 == '.')
            {
                moved = true;
                Swap(ref map[lastX, y], ref map[0, y]);
            }
        }

        return moved;
    }
    
    private void ParseLines(string[] lines)
    {
        _map = new char[lines.Length, lines[0].Length];
        // _map2 = new char[lines.Length, lines[0].Length];

        for (int x = 0; x < lines.Length; x++)
        {
            for (int y = 0; y < lines[0].Length; y++)
            {
                _map[x, y] = lines[x][y];
            }
        }
    }
}
