namespace AdventOfCode.Day11;

public class Solution11 : Solution
{
    public class Cave
    {
        public const int SIZE = 10;
        public readonly Tile[,] Grid = new Tile[SIZE, SIZE];
    }
    
    public class Tile
    {
        public int Energy { get; set; }
        public bool Flashed { get; set; }
    }

    private Cave _cave;
    private long _flashes = 0;
    private long _steps = 0;

    public string Run()
    {
        var lines = InputReader.ReadFileLines();
        Parse(lines);
        
        Steps(100);
        long A = _flashes;

        RunUntil();
        long B = _steps;
        
        return A + "\n" + B;
    }

    private void Steps(int number)
    {
        for (_steps = 0; _steps < number; _steps++)
        {
            Step();
        }
    }

    private void RunUntil()
    {
        while (true)
        {
            bool synchronized = Step();
            _steps++;
            
            if (synchronized)
            {
                break;
            }
        }
    }

    private bool Step()
    {
        ApplyToGrid(Increase);
        
        ApplyToGrid(CheckFlash);

        bool allFlashed = true;
        ApplyToGrid(IsSynchronized);

        ApplyToGrid(ResetFlashed);

        return allFlashed;
        
        void IsSynchronized(Tile tile)
        {
            if (!tile.Flashed)
            {
                allFlashed = false;
            }
        }
    }

    private void Increase(Tile t)
    {
        t.Energy += 1;
    }

    private void CheckFlash(Tile tile, int i, int j)
    {
        if (tile.Energy > 9)
        {
            Flash(i, j);
        }
    }
    
    private void ResetFlashed(Tile tile)
    {
        if (tile.Flashed)
        {
            tile.Energy = 0;
            tile.Flashed = false;
        }
    }

    private void ApplyToGrid(Action<Tile> action)
    {
        void ActionWithoutPosition(Tile tile, int i, int j) => action(tile);
        ApplyToGrid(ActionWithoutPosition);
    }
    
    private void ApplyToGrid(Action<Tile, int, int> action)
    {
        for (int i = 0; i < Cave.SIZE; i++)
        {
            for (int j = 0; j < Cave.SIZE; j++)
            {
                action(_cave.Grid[i, j], i, j);
            }
        }
    }

    private void Flash(int i, int j)
    {
        if(_cave.Grid[i, j].Flashed) return;
        _cave.Grid[i, j].Flashed = true;
        _flashes++;

        for (int di = -1; di < 2; di++)
        {
            for (int dj = -1; dj < 2; dj++)
            {
                if(di == 0 && dj == 0) continue;

                int idi = i + di;
                int jdj = j + dj;
                
                if(!OnGrid(idi, jdj)) continue;

                var tile = _cave.Grid[idi, jdj];
                Increase(tile);
                CheckFlash(tile, idi, jdj);
            }
        }
    }

    private bool OnGrid(int i, int j)
    {
        return i is >= 0 and < Cave.SIZE && 
               j is >= 0 and < Cave.SIZE;
    }

    private void Parse(List<string> input)
    {
        _cave = new Cave();
        for (int i = 0; i < Cave.SIZE; i++)
        {
            var line = input[i];
            for (int j = 0; j < Cave.SIZE; j++)
            {
                _cave.Grid[i, j] = new Tile
                {
                    Energy = line[j] - '0',
                    Flashed = false
                };
            }
        }
    }
}
