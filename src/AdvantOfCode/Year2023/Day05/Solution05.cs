using AdventOfCode.Extensions;
using AdventOfCode.Year2019.Day12;

namespace AdventOfCode.Year2023.Day05;

[DayInfo(2023, 05)]
public class Solution05 : Solution
{
    private record Map(string From, string To, RangeMap[] Ranges);
    private record RangeMap(long Destination, long Source, long Length);

    private record Range(long Number, long Length);
    
    public string Run()
    {
        string[] input = this.ReadLines();
        long[] seeds = ParseSeedLine(input[0]);
        Range[] seedRanges = ParseSeedLengthLine(input[0]);
        Map[] maps = ParseMappings(input);

        List<long> locations = seeds
            .Select(x=> Solve2(x, maps))
            .ToList();

        var locs = seedRanges.Select(x => Solve3(x, maps)).ToList();
        long min = locs.Min(x => x.Min(y => y.Number));
        
        return locations.Min() + "\n"+ min;
    }
    
    private List<Range> Solve3(Range seed, Map[] maps)
    {
        List<Range> ranges = new() { seed };
        List<Range> mappedRanges;

        foreach (Map map in maps)
        {
            mappedRanges = new();

            foreach (Range range in ranges)
            {
                Range remaining = range;
                long remainingEnd = remaining.Number + remaining.Length;
                foreach (RangeMap rangeMap in map.Ranges)
                {
                    long rangeMapEnd = rangeMap.Source + rangeMap.Length;
                    // start inside mapping
                    if (remaining.Number >= rangeMap.Source && remaining.Number < rangeMap.Source + remaining.Length)
                    {
                        // fully inside
                        if (remainingEnd <= rangeMap.Source + rangeMap.Length)
                        {
                            Range mapped = remaining with { Number = remaining.Number - rangeMap.Source + rangeMap.Destination };
                            mappedRanges.Add(mapped);
                            remaining = remaining with { Length = 0 };
                            break;
                        }
                        else
                        {
                            // only start inside
                            long split = rangeMap.Source + rangeMap.Length;
                            Range mapped = new Range(range.Number - rangeMap.Source + rangeMap.Destination, split - remaining.Number);
                            remaining = new Range(split, range.Length - mapped.Length); 
                            mappedRanges.Add(mapped);
                            // split + continue with remaining
                        }
                    }
                    // end inside mapping
                    else if (remaining.Number < rangeMap.Source && remainingEnd > rangeMap.Source && remainingEnd < rangeMapEnd)
                    {
                        long split = rangeMap.Source;
                        Range before = remaining with { Length = split - remaining.Number };
                        Range mapped = new Range(rangeMap.Destination, remaining.Length - before.Length);
                        mappedRanges.Add(before);
                        mappedRanges.Add(mapped);
                        remaining = remaining with { Length = 0 };
                        break;
                        // only end inside
                    }
                    // over mapping
                    else if (remaining.Number < rangeMap.Source && remainingEnd > rangeMapEnd)
                    {
                        Range before = range with { Length = rangeMap.Source - remaining.Number };
                        Range inside = new Range(rangeMap.Destination, rangeMap.Length);
                        mappedRanges.Add(before);
                        mappedRanges.Add(inside);
                        remaining = new Range(rangeMap.Source + rangeMap.Length, remaining.Length - before.Length - inside.Length);
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

    // private Map Simplify(Map map)
    // {
    //     List<RangeMap> ranges = new();
    //     long source = 0;
    //     long length = 0;
    //     long dest = 0;
    //     
    //     for (int i = 1; i < map.Ranges.Length; i++)
    //     {
    //         RangeMap r = map.Ranges[i];
    //         if (r.Source == source + length && r.Destination == dest + length)
    //         {
    //             length += r.Length;
    //         }
    //         else
    //         {
    //             if (length > 0)
    //             {
    //                 ranges.Add(new RangeMap(dest, source, length));
    //             }
    //
    //             dest = r.Destination;
    //             source = r.Source;
    //             length = r.Length;
    //         }
    //     }
    //
    //     if (length > 0)
    //     {
    //         ranges.Add(new RangeMap(dest, source, length));
    //     }
    //
    //     return map with { Ranges = ranges.ToArray() };
    // }
    
    
    private long Solve2(long seed, Map[] maps)
    {
        long number = seed;

        foreach (Map map in maps)
        {
            long mappedNumber = number;
            foreach (RangeMap range in map.Ranges)
            {
                if (number >= range.Source && number < range.Source + range.Length)
                {
                    mappedNumber = number - range.Source + range.Destination;
                    break;
                }
            }
            number = mappedNumber;
        }

        return number;
    }

    private long Solve(long seed, Map[] maps)
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
                    mappedNumber = number - range.Source + range.Destination;
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