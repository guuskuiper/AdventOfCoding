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
        Queue<(int x, int y, int ynew)> moves = new ();
        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                if (_map[x, y] == '>')
                {
                    int ytarget = (y + 1) % _map.GetLength(1);
                    if(_map[x, ytarget] == '.')
                    {
                        moves.Enqueue((x, y, ytarget));
                    }
                }
            }
        }

        bool moved = moves.Count > 0;
        while (moves.Count > 0)
        {
            var move = moves.Dequeue();
            Swap(ref _map[move.x, move.y], ref _map[move.x, move.ynew]);
        }
        
        return moved;
    }

    private void Swap(ref char a, ref char b)
    {
        (a, b) = (b, a);
    }
    
    private bool MoveSouth()
    {
        Queue<(int x, int xnew, int y)> moves = new ();
        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                if (_map[x, y] == 'v')
                {
                    int xtarget = (x + 1) % _map.GetLength(0);
                    if(_map[xtarget, y] == '.')
                    {
                        moves.Enqueue((x, xtarget, y));
                    }
                }
            }
        }

        bool moved = moves.Count > 0;
        while (moves.Count > 0)
        {
            var move = moves.Dequeue();
            Swap(ref _map[move.x, move.y], ref _map[move.xnew, move.y]);
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
