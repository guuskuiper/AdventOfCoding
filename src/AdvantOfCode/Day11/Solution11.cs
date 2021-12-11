namespace AdventOfCode.Day11;

public class Solution11 : Solution
{
    public class Cave
    {
        public const int SIZE = 10;
        public Tile[,] Grid = new Tile[SIZE, SIZE];
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
        
        return A + "\n" + _steps;
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
        for (int i = 0; i < Cave.SIZE; i++)
        {
            for (int j = 0; j < Cave.SIZE; j++)
            {
                _cave.Grid[i, j].Energy += 1;
            }
        }
        
        for (int i = 0; i < Cave.SIZE; i++)
        {
            for (int j = 0; j < Cave.SIZE; j++)
            {
                if (_cave.Grid[i, j].Energy > 9)
                {
                    Flash(i, j);
                }
            }
        }

        bool allFlashed = true;
        for (int i = 0; i < Cave.SIZE; i++)
        {
            for (int j = 0; j < Cave.SIZE; j++)
            {
                if (!_cave.Grid[i, j].Flashed)
                {
                    allFlashed = false;
                }
            }
        }
        
        for (int i = 0; i < Cave.SIZE; i++)
        {
            for (int j = 0; j < Cave.SIZE; j++)
            {
                if (_cave.Grid[i, j].Flashed)
                {
                    _cave.Grid[i, j].Energy = 0;
                    _cave.Grid[i, j].Flashed = false;
                }
            }
        }

        return allFlashed;
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
                
                Increase(idi, jdj);
                
                if (OnGrid(idi, jdj) && _cave.Grid[idi, jdj].Energy > 9)
                {
                    Flash(idi, jdj);
                }
            }
        }
    }

    private void Increase(int i, int j)
    {
        if(!OnGrid(i, j)) return;
        
        _cave.Grid[i, j].Energy += 1;
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
