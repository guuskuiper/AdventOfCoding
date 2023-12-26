using System.Text;

namespace AdventOfCode.Year2019.Day17;

class ASCII
{
    public enum Direction
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3,
    }

    private const int WIDTH = 53;
    private const int HEIGHT = 39;
    private bool[,] grid;
    private ElfComputer computer;

    private int readX = 0;
    private int readY = 0;

    private int robotX;
    private int robotY;
    private int endX;
    private int endY;
    private Direction robotDirection;
    private string inputData;
    private int inputCount;

    bool mode;

    public long Response;

    public ASCII(IEnumerable<long> instructions)
    {
        computer = new ElfComputer(instructions, Input, Output);

        grid = new bool[WIDTH, HEIGHT];
    }

    public int Start()
    {
        computer.ProcessInstructions();

        int crossings = FindCrossings();
        FindEnd();
        StraightPath();
        return crossings;
    }

    public void Start2(string inputs)
    {
        mode = true;
        inputData = inputs;
        inputCount = 0;

        computer.SetData(0, 2);

        computer.ProcessInstructions();
    }

    public long Input()
    {
        var ch = inputData[inputCount];
        inputCount++;
        return Convert.ToInt32(ch);
    }

    public void Output(long output)
    {
        if(output > 0x7F)
        {
            Response = output;
            return;
        }
        var ch = Convert.ToChar(output);

        if(!mode)
        {
            switch(ch)
            {
                case '>':
                    robotX = readX;
                    robotY = readY;
                    robotDirection = Direction.Right;
                    grid[readX, readY] = true;
                    readX++;
                    break;
                case '<':
                    robotX = readX;
                    robotY = readY;
                    robotDirection = Direction.Left;
                    grid[readX, readY] = true;
                    readX++;
                    break;
                case 'v':
                    robotX = readX;
                    robotY = readY;
                    robotDirection = Direction.Down;
                    grid[readX, readY] = true;
                    readX++;
                    break;
                case '^':
                    robotX = readX;
                    robotY = readY;
                    robotDirection = Direction.Up;
                    grid[readX, readY] = true;
                    readX++;
                    break;
                case '#':
                    grid[readX, readY] = true;
                    readX++;
                    break;
                case '.':
                    grid[readX, readY] = false;
                    readX++;
                    break;
                case '\n':
                    readX = 0;
                    readY++;
                    break;
                default:
                    Console.WriteLine($"Unknown character {output}({ch})");
                    //throw new Exception($"Unknown character {output}-{ch}");
                    break;
            }
        }
    }

    private int FindCrossings()
    {
        // return Sum( x*y)
        int sum = 0;
        int crossings = 0;
        for(int y = 1; y < HEIGHT - 1; y++)
        {
            for(int x = 1; x < WIDTH - 1; x++)
            {
                if(!grid[x,y]) continue;
                if(!grid[x+1,y]) continue;
                if(!grid[x-1,y]) continue;
                if(!grid[x,y+1]) continue;
                if(!grid[x,y-1]) continue;
                sum += x*y;
                crossings++;
            }
        }
        return sum;
    }

    private void FindEnd()
    {
        for(int y = 1; y < HEIGHT - 1; y++)
        {
            for(int x = 1; x < WIDTH - 1; x++)
            {
                var nearCount = 0;
                if(!grid[x,y]) continue;
                if(grid[x+1,y]) nearCount++;
                if(grid[x-1,y]) nearCount++;
                if(grid[x,y+1]) nearCount++;
                if(grid[x,y-1]) nearCount++;

                if(nearCount == 1 && x != robotX && y != robotY)
                {
                    endX = x;
                    endY = y;
                    //Console.WriteLine($"End at {x},{y}");
                    //return;
                }
                    
            }
        }
    }

// AAAAAAAAAAAAAAAA BBBBBBBBBBB AAAAAAAAAAAAAAAA CCCCCCCCCCCCCCCCC AAAAAAAAAAAAAAAA BBBBBBBBBBB CCCCCCCCCCCCCCCCC BBBBBBBBBBB CCCCCCCCCCCCCCCCC BBBBBBBBBBB
// L,10,R,8,L,6,R,6,L,8,L,8,R,8,L,10,R,8,L,6,R,6,R,8,L,6,L,10,L,10,L,10,R,8,L,6,R,6,L,8,L,8,R,8,R,8,L,6,L,10,L,10,L,8,L,8,R,8,R,8,L,6,L,10,L,10,L,8,L,8,R,8

// A: 
// L,10,R,8,L,6,R,6

// B:
// L,8,L,8,R,8

// C:
// R8,L,6,L,10,L,10

// A,B,A,C,A,B,C,B,C,B

    private void StraightPath()
    {
        int curX = robotX;
        int curY = robotY;
        Direction curDir = robotDirection;


        int tempX = 0;
        int tempY = 0;

        int steps = 0;
        var sb = new StringBuilder("");
        while(! (curX == endX && curY == endY))
        {
            if(DirectionPosition(curX, curY, curDir, out tempX, out tempY))
            {
                steps++;
                curX = tempX;
                curY = tempY;
            }
            else
            {
                // find new direction
                if(steps > 0)
                {
                    sb.Append(steps);
                    sb.Append(',');
                }
                steps = 0;
                bool turn = false; // false = left, true = right
                if(curDir != Direction.Down && DirectionPosition(curX, curY, Direction.Up, out tempX, out tempY))
                {
                    turn = curDir == Direction.Left;
                    curDir = Direction.Up;
                }
                else if(curDir != Direction.Right && DirectionPosition(curX, curY, Direction.Left, out tempX, out tempY))
                {
                    turn = curDir == Direction.Down;
                    curDir = Direction.Left;
                }
                else if(curDir != Direction.Up && DirectionPosition(curX, curY, Direction.Down, out tempX, out tempY))
                {
                    turn = curDir == Direction.Right;
                    curDir = Direction.Down;
                }
                else if(curDir != Direction.Left && DirectionPosition(curX, curY, Direction.Right, out tempX, out tempY))
                {
                    turn = curDir == Direction.Up;
                    curDir = Direction.Right;
                }
                else
                {
                    throw new Exception("Crashed!!!");
                }
                char dirChat = turn ? 'R' : 'L';
                sb.Append(dirChat);
                sb.Append(',');
            }
        }

        if(steps > 0)
        {
            sb.Append(steps);
            sb.Append(',');
        }

        //Console.WriteLine("Final: " + sb);
    }

    private bool DirectionPosition(int x, int y, Direction directory, out int x2, out int y2)
    {
        switch(directory)
        {
            case Direction.Left:
                x2 = x - 1;
                y2 = y;
                if(x2 < 0) return false;
                break;
            case Direction.Right:
                x2 = x + 1;
                y2 = y;
                if(x2 >= WIDTH) return false;
                break;
            case Direction.Up:
                x2 = x;
                y2 = y - 1;
                if(y2 < 0) return false;
                break;
            case Direction.Down:
                x2 = x;
                y2 = y + 1;
                if(y2 >= HEIGHT) return false;
                break;
            default:
                x2 = -1;
                y2 = -1;
                return false;
        }

        if(!grid[x2, y2]) return false;

        return true;
    }
}