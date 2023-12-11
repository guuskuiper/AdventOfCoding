using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using AdventOfCode.Extensions;
using AdventOfCode.Graph;

namespace AdventOfCode.Year2023.Day11;

[DayInfo(2023, 11)]
public class Solution11 : Solution
{
    public string Run()
    {
		string[] input = this.ReadLines();

		var grid = input.ToGrid();

		var emptyRows = EmptyRowsNumbers(grid);
		var emptyColumn = EmptyColumnNumbers(grid);

        var stars = Find(grid, '#').ToArray();
		var pairs = AllCombinations(stars).ToArray();

		long sum = pairs.Select(p => Distance(p, emptyColumn, emptyRows, 1)).Sum();
		long sum2 = pairs.Select(p => Distance(p, emptyColumn, emptyRows, 2)).Sum();
		long sum10 = pairs.Select(p => Distance(p, emptyColumn, emptyRows, 10)).Sum();
		long sum100 = pairs.Select(p => Distance(p, emptyColumn, emptyRows, 100)).Sum();
		long sum1m = pairs.Select(p => Distance(p, emptyColumn, emptyRows, 1_000_000)).Sum();

		if (true)
		{
			RectValueGrid<char> rectGrid = new RectValueGrid<char>(grid);
			rectGrid.Draw();
		}

		return sum2 + "\n" + sum1m; 
	}

    private long Distance(Pair pair, int[] emptyCol, int[] emptyRow, int expansionFactor = 1)
    {
	    long withoutExpand = AStar.DistanceManhattan(pair.A, pair.B);

	    long emptyCols = 0;
		{
		    int minX = Math.Min(pair.A.X, pair.B.X);
		    int maxX = Math.Max(pair.A.X, pair.B.X);
		    
		    for (int x = minX; x <= maxX; x++)
		    {
			    if (emptyCol.Contains(x))
			    {
				    emptyCols++;
			    }
		    }
	    }

	    long emptyRows = 0;
	    {
		    int minY = Math.Min(pair.A.Y, pair.B.Y);
		    int maxY = Math.Max(pair.A.Y, pair.B.Y);
		    for (int y = minY; y <= maxY; y++)
		    {
			    if (emptyRow.Contains(y))
			    {
				    emptyRows++;
			    }
		    }
	    }

	    long empties = emptyRows + emptyCols;

		return withoutExpand + (expansionFactor - 1) * empties;
	}

    private record Pair(Point A, Point B);

    private int[] EmptyRowsNumbers(char[,] grid)
    {
	    List<int> rowIndexes = new();
	    for (int y = 0; y < grid.Heigth(); y++)
	    {
		    bool isEmpty = true;
		    for (int x = 0; x < grid.Width(); x++)
		    {
			    char c = grid[x, y];
			    if (c == '#')
			    {
					isEmpty = false;
					break;
			    }
		    }

		    if (isEmpty)
		    {
			    rowIndexes.Add(y);
		    }
	    }
		return rowIndexes.ToArray();
    }

    private int[] EmptyColumnNumbers(char[,] grid)
    {
	    List<int> rowIndexes = new();
		for (int x = 0; x < grid.Width(); x++)
	    {
		    bool isEmpty = true;
			for (int y = 0; y < grid.Heigth(); y++)
		    {
			    char c = grid[x, y];
			    if (c == '#')
			    {
				    isEmpty = false;
				    break;
			    }
		    }

		    if (isEmpty)
		    {
			    rowIndexes.Add(x);
		    }
	    }
	    return rowIndexes.ToArray();
    }

	private IEnumerable<Pair> AllCombinations(Point[] points)
    {
	    for (int x = 0; x < points.Length; x++)
	    {
		    for (int y = x + 1; y < points.Length; y++)
		    {
			    yield return new Pair(points[x], points[y]);
		    }
	    }
    }

    private IEnumerable<Point> Find(char[,] grid, char c)
    {
	    for (int x = 0; x < grid.Width(); x++)
	    {
		    for (int y = 0; y < grid.Heigth(); y++)
		    {
			    if (grid[x, y] == c)
			    {
				    yield return new Point(x, y);
			    }
		    }
	    }
    }
}    