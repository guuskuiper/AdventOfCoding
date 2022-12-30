namespace AdventOfCode.Year2019.Day24;

public class GameOfLife
{
    private const int SIZE = 5;
    private const char BUG = '#';
    private const char EMPTY = '.';
    private bool[,] grid;
    private bool[,,] rGrid;
    private HashSet<int> bioDiversity;

    public GameOfLife(IEnumerable<string> input)
    {
        var inputEnum = input.GetEnumerator();
        grid = new bool[SIZE, SIZE];
        bioDiversity = new HashSet<int> ();

        for (int i = 0; i < SIZE; i++)
        {
            inputEnum.MoveNext();
            var line = inputEnum.Current;
            for (int j = 0; j < SIZE; j++)
            {   
                grid[j, i] = line[j] == BUG;
            }
        }

        var bio = GridToBio(grid);
        bioDiversity.Add(bio);
    }

    public int CalcSame()
    {
        while(Step())
        {

        }
        return GridToBio(grid);
    }

//Each minute, The bugs live and die based on the number of bugs in the four adjacent tiles:

// A bug dies (becoming an empty space) unless there is exactly one bug adjacent to it.
// An empty space becomes infested with a bug if exactly one or two bugs are adjacent to it.

    private bool Step()
    {
        var newGrid = new bool[SIZE, SIZE];
        for (int y = 0; y < SIZE; y++)
        {
            for (int x = 0; x < SIZE; x++)
            {
                newGrid[x, y] = LiveAndDie(x, y);
            }
        }

        grid = newGrid;
        var bio = GridToBio(newGrid);
        if(!bioDiversity.Contains(bio))
        {
            bioDiversity.Add(bio);
            return true;
        }
        
        return false;
    }

    private bool LiveAndDie(int x, int y)
    {
        if(grid[x, y])
        {
            return Adjecent(x, y) == 1;
        }
        else
        {
            var adjecent = Adjecent(x, y);
            if(adjecent == 1 || adjecent == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private int Adjecent(int x, int y)
    {
        int count = 0;
        if(x > 0 && grid[x - 1, y]) count ++;
        if(x < SIZE - 1 && grid[x + 1, y]) count ++;
        if(y > 0 && grid[x, y - 1]) count ++;
        if(y < SIZE - 1 && grid[x, y + 1]) count ++;
        return count;
    } 

    private void Display()
    {
        for (int y = 0; y < SIZE; y++)
        {
            for (int x = 0; x < SIZE; x++)
            {   
                Console.Write(grid[x, y] ? BUG : EMPTY);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public static int GridToBio(bool[,] array)
    {
        int sum = 0;
        int count = 0;
        for (int y = 0; y < SIZE; y++)
        {
            for (int x = 0; x < SIZE; x++)
            {   
                var val = array[x, y];
                if(val)
                {
                    sum += 1 << count;
                }

                count++;
            }
        }
        return sum;
    }

    public int Steps(int count)
    {
        var maxLevels = 2*count+1;
        rGrid = new bool[SIZE, SIZE, maxLevels];
        var currentDepth = count / 2;
        for (int y = 0; y < SIZE; y++)
        {
            for (int x = 0; x < SIZE; x++)
            {   
                if(x == 2 && y == 2) continue;
                rGrid[x, y, currentDepth] = grid[x, y];
            }
        }

        foreach(var step in Enumerable.Range(0, count))
        {
            var newGrid = new bool[SIZE, SIZE, maxLevels];
            for(int d = 0; d < maxLevels; d++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    for (int x = 0; x < SIZE; x++)
                    {
                        if(x == 2 && y == 2) continue;
                        newGrid[x, y, d] = LiveAndDie(x, y, d);
                    }
                }
            }
            rGrid = newGrid;
            //DisplayR();
        }

        return CountBugs();
    }
        
    private bool LiveAndDie(int x, int y, int depth)
    {
        if(rGrid[x, y, depth])
        {
            return Adjecent(x, y, depth) == 1;
        }
        else
        {
            var adjecent = Adjecent(x, y, depth);
            if(adjecent == 1 || adjecent == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private int Adjecent(int x, int y, int depth)
    {
        int count = 0;
        if(x > 0 && rGrid[x - 1, y, depth]) count++;
        if(x < SIZE - 1 && rGrid[x + 1, y, depth]) count++;
        if(y > 0 && rGrid[x, y - 1, depth]) count++;
        if(y < SIZE - 1 && rGrid[x, y + 1, depth]) count++;

        // recursive grids...
        //previous depth
        if(depth > 0 && x == 0 && rGrid[1, 2, depth - 1]) count++;
        if(depth > 0 && x == SIZE - 1 && rGrid[3, 2, depth - 1]) count++;
        if(depth > 0 && y == 0 && rGrid[2, 1, depth - 1]) count++;
        if(depth > 0 && y == SIZE - 1 && rGrid[2, 3, depth - 1]) count++;

        //next depth
        if(depth < rGrid.GetLength(2) - 1)
        {
            if(x == 1 && y == 2)
            {
                for(int i = 0; i < SIZE; i++)
                {
                    if(rGrid[0, i, depth + 1]) count++;
                }
            }
            if(x == 3 && y == 2)
            {
                for(int i = 0; i < SIZE; i++)
                {
                    if(rGrid[SIZE-1, i, depth + 1]) count++;
                }
            }
            if(x == 2 && y == 1)
            {
                for(int i = 0; i < SIZE; i++)
                {
                    if(rGrid[i, 0, depth + 1]) count++;
                }
            }
            if(x == 2 && y == 3)
            {
                for(int i = 0; i < SIZE; i++)
                {
                    if(rGrid[i, SIZE-1, depth + 1]) count++;
                }
            }
        }

        return count;
    } 

    private int CountBugs()
    {
        var count = 0;
        for(int d = 0; d < rGrid.GetLength(2); d++)
        {
            count += CountLevel(d);
        }
        return count;
    }

    private int CountLevel(int depth)
    {
        var count = 0;
        for (int y = 0; y < SIZE; y++)
        {
            for (int x = 0; x < SIZE; x++)
            {
                if(x == 2 && y == 2) continue;
                if(rGrid[x, y, depth]) count++;
            }
        }
        return count;
    }

    private void DisplayR()
    {
        for(int d = 0; d < rGrid.GetLength(2); d++)
        {
            if(CountLevel(d)> 0)
            {
                Console.WriteLine("Depth: " + (d - (rGrid.GetLength(2) / 2)));
                for (int y = 0; y < SIZE; y++)
                {
                    for (int x = 0; x < SIZE; x++)
                    {   
                        if(x == 2 && y == 2) Console.Write('?');
                        else Console.Write(rGrid[x, y, d] ? BUG : EMPTY);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
    }
}