using System.Collections.Concurrent;
using System.Diagnostics;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day05;

[DayInfo(2023, 05)]
public class Solution05 : Solution
{
    private record Map(string From, string To, RangeMap[] Ranges);

    private record RangeMap(long Destination, long Source, long Length)
    {
        public long Offset = Destination - Source;
        public long End = Source + Length;
    }

    private record Range(long Number, long Length)
    {
	    public (Range a, Range b) Split(long position)
	    {
            if(position < Number) throw new ArgumentOutOfRangeException(nameof(position) + " before Range: " + ToString());
            if(position > Number + Length) throw new ArgumentOutOfRangeException(nameof(position) + " after Range: " + ToString());

            Range before = this with { Length = position - Number };
            Range mapped = new Range(position, Length - before.Length);

            Debug.Assert(Length == before.Length + mapped.Length);

            return (before, mapped);
	    }

	    public Range Map(long offset) => this with { Number = Number + offset };
    }
    
    public string Run()
    {
        string[] input = this.ReadLines();
        long[] seeds = ParseSeedLine(input[0]);
        Range[] seedRanges = ParseSeedLengthLine(input[0]);
        Map[] maps = ParseMappings(input);

        List<long> locations = seeds
            .Select(x=> Solve(x, maps))
            .ToList();

        // Enable / disable bruteforce.
        if (false)
        {
            var minNumbers = new ConcurrentBag<long>();
            
            // Splitting ranges allows for more cores to process it in parallel.
            Parallel.ForEach(seedRanges, range =>
            {
                long minB = SolveBruteForce(range, maps);
                minNumbers.Add(minB);
            });
            Console.WriteLine(minNumbers.Min());
        }

        var locs = seedRanges.Select(x => Solve(x, maps)).ToList();
        long min = locs.Min(x => x.Min(y => y.Number));
        
        return locations.Min() + "\n"+ min;
    }

    private long SolveBruteForce(Range range, Map[] maps)
    {
        long end = range.Number + range.Length;
        long min = long.MaxValue;
        for (long i = range.Number; i < end; i++)
        {
            long result = Solve(i, maps);
            if (result < min) min = result;
        }
        return min;
    }
    
    private List<Range> Solve(Range seed, Map[] maps)
    {
        List<Range> ranges = new() { seed };
        List<Range> mappedRanges;

        foreach (Map map in maps)
        {
            mappedRanges = new();

            foreach (Range range2 in ranges)
            {
                Range remaining = range2;
                foreach (RangeMap rangeMap in map.Ranges)
                {
					long remainingEnd = remaining.Number + remaining.Length;
                    long rangeMapEnd = rangeMap.Source + rangeMap.Length;
                    // start inside mapping
                    if (remaining.Number >= rangeMap.Source && remaining.Number < rangeMap.Source + rangeMap.Length)
                    {
                        // fully inside
                        if (remainingEnd <= rangeMap.Source + rangeMap.Length)
                        {
                            Range mapped = remaining.Map(rangeMap.Offset);
                            mappedRanges.Add(mapped);
                            remaining = remaining with { Length = 0 };
                            break;
                        }
                        else
                        {
							// only start inside
							(Range inside, remaining) = remaining.Split(rangeMap.End);
							Range mapped = inside.Map(rangeMap.Offset);
                            mappedRanges.Add(mapped);
						}
                    }
                    // end inside mapping
                    else if (remaining.Number < rangeMap.Source && remainingEnd > rangeMap.Source && remainingEnd < rangeMapEnd)
                    {
                        (Range before, Range inside) = remaining.Split(rangeMap.Source);
						Range mapped = inside.Map(rangeMap.Offset);
                        mappedRanges.Add(before);
                        mappedRanges.Add(mapped);
                        remaining = remaining with { Length = 0 };
						break;
                    }
                    // over mapping
                    else if (remaining.Number < rangeMap.Source && remainingEnd > rangeMapEnd)
                    {
	                    (Range before, Range rest) = remaining.Split(rangeMap.Source);
	                    (Range inside, remaining) = rest.Split(rangeMap.End);
	                    Range mapped = inside.Map(rangeMap.Offset);
                        mappedRanges.Add(before);
                        mappedRanges.Add(mapped);
					}
                }

                if (remaining.Length > 0)
                {
                    mappedRanges.Add(remaining);
                }
            }

            // try to merge?
            ranges = mappedRanges;
        }

        return ranges;
    }

    private long Solve(long seed, Map[] maps)
    {
        long number = seed;

        foreach (Map map in maps)
        {
            long mappedNumber = number;
            foreach (RangeMap range in map.Ranges)
            {
                if (number >= range.Source && number < range.Source + range.Length)
                {
                    mappedNumber = number + range.Offset;
                    break;
                }
            }
            number = mappedNumber;
        }

        return number;
    }

    private long SolveOld(long seed, Map[] maps)
    {
        string name = "seed";
        long number = seed;

        while (name != "location")
        {
            Map map = maps.Single(x => x.From == name);
            name = map.To;
            long mappedNumber = number;
            foreach (RangeMap range in map.Ranges)
            {
                if (number >= range.Source && number < range.Source + range.Length)
                {
                    mappedNumber = number + range.Offset;
					break;
                }
            }
            number = mappedNumber;
        }

        return number;
    }

    private static Map[] ParseMappings(string[] input)
    {
        List<Map> maps = new();

        List<RangeMap> ranges = new();
        string from = "", to = "";
        for (int i = 1; i < input.Length; i++)
        {
            LineReader reader = new(input[i]);
            if (reader.IsLetter)
            {
                if (ranges.Count > 0)
                {
                    maps.Add(new Map(from, to, ranges.OrderBy(x => x.Source).ToArray()));
                    ranges = new();
                }
                from = reader.ReadLetters().ToString();
                reader.ReadChars("-to-");
                to = reader.ReadLetters().ToString();
                reader.ReadChars(" map:");
            }
            else
            {
                long f = reader.ReadLong();
                reader.SkipWhitespaces();
                long t = reader.ReadLong();
                reader.SkipWhitespaces();
                long length = reader.ReadLong();
                ranges.Add(new RangeMap(f, t, length));
            }
        }
        if (ranges.Count > 0)
        {
            maps.Add(new Map(from, to, ranges.OrderBy(x => x.Source).ToArray()));
        }

        return maps.ToArray();
    }

    private long[] ParseSeedLine(string line)
    {
        LineReader reader = new(line);
        reader.ReadChars("seeds: ");

        List<long> numbers = [];
        while (!reader.IsDone)
        {
            long number = reader.ReadLong();
            numbers.Add(number);
            reader.SkipWhitespaces();
        }

        return numbers.ToArray();
    }
    
    private Range[] ParseSeedLengthLine(string line)
    {
        LineReader reader = new(line);
        reader.ReadChars("seeds: ");

        List<Range> ranges = [];
        while (!reader.IsDone)
        {
            long number = reader.ReadLong();
            reader.SkipWhitespaces();
            long length = reader.ReadLong();
            reader.SkipWhitespaces();
            ranges.Add(new Range(number, length));
        }

        return ranges.ToArray();
    }
}    