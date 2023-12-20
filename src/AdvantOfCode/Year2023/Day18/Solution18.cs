using System.Globalization;
using Clipper2Lib;

namespace AdventOfCode.Year2023.Day18;

[DayInfo(2023, 18)]
public class Solution18 : Solution
{
	private Size North = new(0, -1);
	private Size West = new(-1, 0);
	private Size South = new(0, 1);
	private Size East = new(1, 0);

	private record Dig(char Direction, int Depth, string RGB);

    public string Run()
    {
        string[] input = this.ReadLines();

        Dig[] digPlan = input.Select(ParseLine).ToArray();

        long area = Area(digPlan);

		long areaB = AreaB(digPlan);

		return area + "\n" + areaB;
	}

    private long AreaB(Dig[] digPlan)
    {
	    Point pos = new Point(0, 0);
	    List<Point> points = [pos];

	    foreach (Dig dig in digPlan)
	    {
		    var span = dig.RGB.AsSpan();
		    int distance = int.Parse(span.Slice(0, 5), NumberStyles.HexNumber);
		    char directionChar = span[5];

		    Size direction = directionChar switch
		    {
			    '3' => North,
			    '1' => South,
			    '2' => West,
			    '0' => East,
			    _ => throw new ArgumentOutOfRangeException()
		    };
		    direction *= distance;
		    pos += direction;
		    points.Add(pos);
	    }

	    long area = OffsetArea(points);

	    return area;
	}

    private long OffsetArea(List<Point> points)
    {
	    const int SCALE = 10;
	    Path64 path = new();
	    Paths64 solution = new();

	    path = new Path64(points.Select(p => new Point64(p.X * SCALE, p.Y * SCALE)));

	    var offset = new ClipperOffset(miterLimit: 5.0);
		offset.AddPath(path, JoinType.Miter, EndType.Polygon);
		offset.Execute(5, solution);

		List<Point> offsetPoints = solution[0].Select(x => new Point((int)x.X, (int)x.Y)).ToList();

		double area = Clipper.Area(solution);

		double areaSelf = Area(offsetPoints);

		return (long) (area / (SCALE * SCALE));
    }


    private long Area(Dig[] digPlan)
    {
        Point pos = new Point(0, 0);
        List<Point> points =[pos];

        foreach (Dig dig in digPlan)
        {
	        Size direction = dig.Direction switch
	        {
		        'U' => North,
		        'D' => South,
		        'L' => West,
		        'R' => East,
		        _ => throw new ArgumentOutOfRangeException()
	        };
	        direction *= dig.Depth;
	        pos += direction;
            points.Add(pos);
        }

        long area = OffsetArea(points);

		return area;
    }

	/// <summary>
	/// Shoelace formula.
	/// </summary>
	/// <param name="points"></param>
	/// <returns></returns>
    private double Area(List<Point> points)
    {
	    double area = 0;

	    Point prev = points[points.Count - 1];
		for (int i = 0; i < points.Count; i++)
	    {
		    Point curr = points[i];

			// Trapezoid formula 
		    //double det = ((double)curr.X + prev.X) * ((double)curr.Y - prev.Y);

			// Triangle formula
		    double det = (double)prev.X * curr.Y - (double)prev.Y * curr.X;
		    area += det;

			prev = curr;
	    }

	    return area * 0.5;
    }

    private Dig ParseLine(string line)
    {
        LineReader reader = new(line);
        char d = reader.ReadChar();
        reader.SkipWhitespaces();
        int depth = reader.ReadInt();
        reader.SkipWhitespaces();
        reader.ReadChars("(#");
        string rgb = reader.ReadWhen(char.IsAsciiLetterOrDigit).ToString();
        reader.ReadChars(")");

        return new(d, depth, rgb);
    }
}    