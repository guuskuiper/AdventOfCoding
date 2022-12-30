namespace AdventOfCode.Year2019.Day19;

public class TractorBeam
{
    private const int WIDTH = 50;
    private const int HEIGHT = 50;
    private ElfComputer computer;
    private bool[,] grid;

    private List<long> inputCopy;

    public TractorBeam(IEnumerable<long> input)
    {
        inputCopy = new List<long> (input);

        grid = new bool[WIDTH, HEIGHT];

        for(int x = 0; x < WIDTH; x++)
        {
            for(int y = 0; y < HEIGHT; y++)
            {
                grid[x,y] = CalcPosition(x, y);
            }
        }
    }

    public int Follow(int size)
    {
        int curX = 4;
        int curY = 3;

        int steps = 0;
        while(true)
        {
            var beam = CalcPosition(curX, curY);
            if(beam)
            {
                if(curY > size)
                {
                    var checkX = curX + size - 1;
                    var checkY = curY - size + 1;
                    var fit = CalcPosition(checkX, checkY);
                    if(fit)
                    {
                        int awnser = curX * 10000 + checkY;
                        if(curX < WIDTH && curY < HEIGHT)
                        {
                            grid[curX, checkY] = false;
                        }

                        return awnser;
                    }
                }
                curY++;
            }
            else
            {
                curX++;
            }

            steps++;
        }
    }

    private bool CalcPosition(int x, int y)
    {
        bool res = false;
        bool first = true;

        var pc = new ElfComputer(inputCopy, () => 
        { 
            if(first) 
            {
                first = false; 
                return x; 
            }
            else 
            {
                return y; 
            }
        }, (x) => res = x > 0);
        pc.ProcessInstructions();
        return res;
    }
    
    public int Count()
    {
        int count = 0;
        for(int y = 0; y < HEIGHT; y++)
        {
            for(int x = 0; x < WIDTH; x++)
            {
                if(grid[x,y]) count++;
            }
        }

        return count;
    }

    private void Display()
    {
        for(int y = 0; y < HEIGHT; y++)
        {
            for(int x = 0; x < WIDTH; x++)
            {
                Console.Write(grid[x,y] ? '#' : '.');
            }
            Console.WriteLine();
        }
    }
}