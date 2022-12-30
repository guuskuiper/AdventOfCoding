using System.Text;

namespace AdventOfCode.Year2019.Day11;

class Robot
{
    private const char FullBlock = (char)0x2588;
    private const char VisibleChar = FullBlock;
    private const char InvisibleChar = ' ';
    
    private const int WIDTH = 1000;
    private const int HEIGHT = 1000;

    public enum Direction
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3,
    }

    private ElfComputer computer;
    private Direction currentDirection = Direction.Up;
    private bool isColor = true;
    private int color;
    private int curX;
    private int curY;
    
    private int maxX;
    private int maxY;
    private int minX;
    private int minY;

    private int[,] panels;
    private bool[,] painted;

    public Robot(IEnumerable<long> instructions, int initialValue)
    {
        computer = new ElfComputer(instructions, Input, Output);

        panels = new int[WIDTH, HEIGHT];
        painted = new bool[WIDTH, HEIGHT];

        // start in the middle?
        curX = WIDTH / 2 + 1;
        curY = HEIGHT / 2 + 1;
        maxX = curX;
        minX = curX;
        maxY = curY;
        minY = curY;

        panels[curX, curY] = initialValue; // intialize one white panel
    }

    public int StartRobot()
    {
        computer.ProcessInstructions();

        int paintedCount = 0;
        for(int x = 0; x < WIDTH; x++)
        {
            for(int y = 0; y < HEIGHT; y++)
            {
                if(painted[x,y])
                {
                    paintedCount++;
                }
            }
        }

        return paintedCount;
    }

    public string Draw()
    {
        StringBuilder sb = new();

        for (int x = maxX; x >= minX; x--)
        {
            for (int y = minY; y <= maxY; y++)
            {
                sb.Append(panels[x, y] == 1 ? VisibleChar : InvisibleChar);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public long Input()
    {
        return panels[curX, curY];
        // current panel: 0 black, 1 is white
        //return 0; // or 1;
    }

    public void Output(long output)
    {
        if(isColor)
        {
            // first paint color: 0 black, 1 is white
            color = (int)output;
        }
        else
        {
            // then direction: 0 left 90, 1 right 90
            if(output == 0) TurnLeft();
            else TurnRight();
        }
        isColor = !isColor;
    }

    private void TurnLeft()
    {
        Direction newDirection;
        switch(currentDirection)
        {
            default:
            case Direction.Up: 
                newDirection = Direction.Left;
                break;
            case Direction.Left: 
                newDirection = Direction.Down;
                break;
            case Direction.Down: 
                newDirection = Direction.Right;
                break;
            case Direction.Right: 
                newDirection = Direction.Up;
                break;
        }

        currentDirection = newDirection;
        Move();
    }

    private void TurnRight()
    {
        Direction newDirection;
        switch(currentDirection)
        {
            default:
            case Direction.Up: 
                newDirection = Direction.Right;
                break;
            case Direction.Left: 
                newDirection = Direction.Up;
                break;
            case Direction.Down: 
                newDirection = Direction.Left;
                break;
            case Direction.Right: 
                newDirection = Direction.Down;
                break;
        }

        currentDirection = newDirection;
        Move();
    }

    private void Move()
    {
        Paint();

        int dx = 0;
        int dy = 0;
        switch(currentDirection)
        {
            case Direction.Up: 
                dx = 1;
                break;
            case Direction.Left: 
                dy = -1;
                break;
            case Direction.Down: 
                dx = -1;
                break;
            case Direction.Right: 
                dy = 1;
                break;
        }

        curX += dx;
        curY += dy;

        if (curX > maxX) maxX = curX;
        if (curX < minX) minX = curX;
        if (curY > maxY) maxY = curY;
        if (curY < minY) minY = curY;
    }

    private void Paint()
    {
        painted[curX, curY] = true;
        panels[curX, curY] = color;
    }
}