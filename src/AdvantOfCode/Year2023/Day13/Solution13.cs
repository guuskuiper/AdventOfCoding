using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day13;

[DayInfo(2023, 13)]
public class Solution13 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines(StringSplitOptions.None);

        List<char[,]> grids = [];

		int start = 0;
        for (int i = 0; i < input.Length; i++)
        {
            string line = input[i];
            if (string.IsNullOrEmpty(line))
            {
	            string[] block = input.Skip(start).Take(i - start).ToArray();
	            var grid = ParseBlock(block);
                grids.Add(grid);
                start = i + 1;
            }
        }

        int sum = grids.Select(FindReflections).Sum();

        int smudgeSum = grids.Select(FindSmudge).Sum();

		return sum + "\n"+ smudgeSum;
    }

    private char[,] ParseBlock(string[] block)
    {
	    return block.ToGrid();
    }

    private int FindSmudge(char[,] grid)
    {
	    int sum = 0;
	    for (int x = 1; x < grid.Width(); x++)
	    {
		    // x = 1: between index 0 and 1
		    bool isColReflection = IsColSmudge(grid, x);
		    if (isColReflection)
		    {
			    sum += x;
		    }
	    }

	    for (int y = 1; y < grid.Heigth(); y++)
	    {
		    // y = 1: between index 0 and 1
		    bool isRowReflection = IsRowSmudge(grid, y);
		    if (isRowReflection)
		    {
			    sum += 100 * y;
		    }
	    }

	    return sum;
    }

    private bool IsColSmudge(char[,] grid, int column)
    {
	    int lower = column;
	    int higher = grid.Width() - lower;

	    int min = Math.Min(lower, higher);

	    int defects = 0;

	    for (int x = 0; x < min; x++)
	    {
		    for (int y = 0; y < grid.Heigth(); y++)
		    {
			    char l = grid[column - 1 - x, y];
			    char r = grid[column + x, y];
			    if (l != r)
			    {
				    defects++;
				    if (defects > 1)
				    {
					    return false;
				    }
			    }
		    }
	    }

	    return defects == 1;
    }

    private bool IsRowSmudge(char[,] grid, int row)
    {
	    int lower = row;
	    int higher = grid.Heigth() - lower;

	    int min = Math.Min(lower, higher);

	    int defects = 0;

		for (int y = 0; y < min; y++)
	    {
		    for (int x = 0; x < grid.Width(); x++)
		    {
			    char l = grid[x, row - 1 - y];
			    char r = grid[x, row + y];
			    if (l != r)
			    {
					defects++;
					if (defects > 1)
					{
						return false;
					}
				}
		    }
	    }

	    return defects == 1;
    }

	private int FindReflections(char[,] grid)
    {
        int sum = 0;
        for (int x = 1; x < grid.Width(); x++)
        {
            // x = 1: between index 0 and 1
	        bool isColReflection = IsColReflection(grid, x);
	        if (isColReflection)
	        {
		        sum += x;
	        }
        }

        for (int y = 1; y < grid.Heigth(); y++)
        {
	        // y = 1: between index 0 and 1
	        bool isRowReflection = IsRowReflection(grid, y);
	        if (isRowReflection)
	        {
				sum += 100*y;
	        }
        }

		return sum;
    }

    private bool IsColReflection(char[,] grid, int column)
    {
	    int lower = column;
	    int higher = grid.Width() - lower;

	    int min = Math.Min(lower, higher);

	    for (int x = 0; x < min; x++)
	    {
		    for (int y = 0; y < grid.Heigth(); y++)
		    {
                char l = grid[column - 1 - x, y];
                char r = grid[column + x, y];
                if (l != r)
                {
	                return false;
                }
		    }
	    }

	    return true;
    }

    private bool IsRowReflection(char[,] grid, int row)
    {
	    int lower = row;
	    int higher = grid.Heigth() - lower;

	    int min = Math.Min(lower, higher);

	    for (int y = 0; y < min; y++)
	    {
		    for (int x = 0; x < grid.Width(); x++)
		    {
			    char l = grid[x, row - 1 - y];
			    char r = grid[x, row + y];
			    if (l != r)
			    {
				    return false;
			    }
		    }
	    }

	    return true;
    }
}    