namespace AdventOfCode.Year2023.Day14;

[DayInfo(2023, 14)]
public class Solution14 : Solution
{
	private const int MAX_CYCLE_COUNT = 200;
	private const int MAX_PERIOD = 100;

	private Size North = new(0, -1);
	private Size West = new(-1, 0);
	private Size South = new(0, 1);
	private Size East = new(1, 0);

    public string Run()
    {
		string[] input = this.ReadLines();

		long score;
		{
			char[,] gridNorth = input.ToGrid();
			Move(gridNorth, North);
			score = Score(gridNorth);
		}

		long scoreB = ScoreCycle(input.ToGrid());

        return score + "\n" + scoreB;
	}

	private long ScoreCycle(char[,] grid)
    {
	    int period = 0;

	    List<long> values = [];
	    for (int i = 0; i < MAX_CYCLE_COUNT; i++)
	    {
			Cycle(grid);
			values.Add(Score(grid));
	    }

	    for (int p = 3; p < MAX_PERIOD; p++)
	    {
		    if (IsValidPeriod(values, p))
		    {
			    period = p;
				break;
		    }
	    }
		ArgumentOutOfRangeException.ThrowIfEqual(0,period);

	    int offset = 1_000_000_000 % period;
		int offsetLast = values.Count % period;
		int diff = offset - offsetLast;
		long result = values[values.Count - period + diff - 1];

	    return result;
    }

    bool IsValidPeriod(List<long> values, int period)
    {
	    int last = values.Count - 1;
	    int lastTest = last - period;

	    bool result = true;
	    for (int i = 0; i < period; i++)
	    {
		    if (values[last - i] != values[lastTest - i])
		    {
			    result = false;
			    break;
		    }
	    }

	    return result;
    }

	private void Draw(char[,] grid, string title = "")
    {
	    if (!string.IsNullOrEmpty(title))
	    {
		    Console.WriteLine(title);
	    }
	    RectValueGrid<char> valueDebugGrid = new(grid);
	    valueDebugGrid.Draw();
	}

    private void Cycle(char[,] grid)
    {
		Move(grid, North);
		Move(grid, West);
		Move(grid, South);
		Move(grid, East);
    }

    private void Move(char[,] grid, Size size)
    {
	    int width = grid.Width();
	    int height = grid.Heigth();

		bool up = size.Width < 0 || size.Height < 0;
		if (up)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Point current = new Point(x, y);
					MovePoint(grid, size, current);
				}
			}
		}
		else
		{
			for (int x = width - 1; x >= 0; x--)
			{
				for (int y = height - 1; y >= 0; y--)
				{
					Point current = new Point(x, y);
					MovePoint(grid, size, current);
				}
			}
		}
    }

    private static void MovePoint(char[,] grid, Size size, Point current)
    {
	    char ch = grid[current.X, current.Y];
	    if (ch != 'O')
	    {
		    return;
	    }

	    while (true)
	    {
		    Point newPoint = current + size;
		    if (!InRange(newPoint))
		    {
			    break;
		    }

		    char target = grid[newPoint.X, newPoint.Y];
		    if (target != '.')
		    {
			    break;
		    }

		    Swap(ref grid[newPoint.X, newPoint.Y], ref grid[current.X, current.Y]);

		    current = newPoint;
	    }

	    bool InRange (Point p) => p.X >= 0 && p.X < grid.Width() &&
	                              p.Y >= 0 && p.Y < grid.Heigth();
    }

    private static void Swap(ref char a, ref char b)
    {
	    (b, a) = (a, b);
    }

	private long Score(char[,] grid)
    {
	    long score = 0;

	    for (int y = 0; y < grid.Heigth(); y++)
	    {
		    for (int x = 0; x < grid.Width(); x++)
		    {
			    Point current = new Point(x, y);
			    char ch = grid[current.X, current.Y];
			    if (ch == 'O')
			    {
				    score += grid.Heigth() - y;
			    }
		    }
	    }

	    return score;
    }
}    