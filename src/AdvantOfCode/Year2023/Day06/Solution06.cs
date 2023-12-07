using System.Diagnostics;
using AdventOfCode.Extensions;
using System.Runtime.InteropServices;

namespace AdventOfCode.Year2023.Day06;

[DayInfo(2023, 06)]
public class Solution06 : Solution
{
	private record Entry(string Name, long[] Values);

    public string Run()
    {
        string[] input = this.ReadLines();

        var entries = input.Select(Parse).ToList();

        List<(long First, long Second)> pairs = entries[0].Values.Zip(entries[1].Values).ToList();
        long optionsMultiplied = pairs.Select(RaceWinOptions).Aggregate(1L, (agg, val) => agg * val);

        long time = ParseNoSpace(input[0]);
        long distance = ParseNoSpace(input[1]);

        long wins = RaceWinOptions((time, distance));
        long wins2 = RaceWinExpression(time, distance);
        Debug.Assert(wins == wins2);

        return optionsMultiplied + "\n" + wins;
    }

    private long RaceWinOptions((long Time, long Distance) race)
    {
	    int wins = 0;
	    for (int i = 0; i <= race.Time; i++)
	    {
		    int buttonHold = i;
		    int speed = buttonHold;
		    long remaining = race.Time - buttonHold;
		    long distance = speed * remaining;
		    if (distance > race.Distance)
		    {
			    wins++;
		    }
	    }
        return wins;
    }

    private long RaceWinExpression(long t, long d)
    {
		//long maxDistance = t / 2;
		//distance (i = hold duration, t = time limit): -i^2 + t*i
		//max distance => ddistance/di = -i * 2 + t => - 2 * i + t = 0 => t = 2 * i -> i = t / 2
		// -i^2 + t*i = d => i (i - t) = d = > i ^ 2 - t * i + d = 0 => 
        // i = 1/2 ( t - sqrt(t^2 - 4*d))
        // i = 1/2 ( sqrt(t^2 - 4*d) + t)

        long start = (long)Math.Ceiling((t - Math.Sqrt(t * t - 4 * d)) / 2);
        long end = (long)Math.Floor((Math.Sqrt(t * t - 4 * d) + t) / 2);
		return end - start + 1;
    }

    private Entry Parse(string line)
    {
        LineReader reader = new LineReader(line);
        string name = reader.ReadLetters().ToString();
        reader.ReadChar(':');

        List<long> values = new();
        while (!reader.IsDone)
        {
			reader.SkipWhitespaces();
	        int number = reader.ReadInt();

            values.Add(number);
        }

        return new Entry(name, values.ToArray());
    }

    private long ParseNoSpace(string line)
    {
	    LineReader reader = new LineReader(line);
	    string name = reader.ReadLetters().ToString();
	    string numbers = "";
	    reader.ReadChar(':');

	    while (!reader.IsDone)
	    {
		    reader.SkipWhitespaces();
		    numbers += reader.ReadInt().ToString();
	    }

	    long value = long.Parse(numbers);

	    return value;
    }
}    