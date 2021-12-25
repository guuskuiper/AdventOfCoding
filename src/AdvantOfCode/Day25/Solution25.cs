namespace AdventOfCode.Day25;

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
            moved = Step();
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
    
    private void Swap(ref char a, ref char b)
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
    
    private void ParseLines(string[] lines)
    {
        _map = new char[lines.Length, lines[0].Length];

        for (int x = 0; x < lines.Length; x++)
        {
            for (int y = 0; y < lines[0].Length; y++)
            {
                _map[x, y] = lines[x][y];
            }
        }
    }
}
