namespace AdventOfCode.Year2022.Day04;

public class Solution04 : Solution
{
    private record Range(int Low, int High);
    private record Pair(Range First, Range Last);
    
    public string Run()
    {
        var lines = InputReader.ReadFileLines();
        var pairs = Parse(lines).ToArray();
        var count = FindFullyContained(pairs);
        var countAnyOverlap = FindAnyOverlap(pairs);
        return count + "\n" + countAnyOverlap;
    }
    
    private int FindFullyContained(IEnumerable<Pair> pairs)
    {
        int count = 0;
        foreach (Pair pair in pairs)
        {
            if (FullyContained(pair.First, pair.Last) 
                || FullyContained(pair.Last, pair.First))
            {
                count++;
            }
        }

        return count;
    }
    
    private int FindAnyOverlap(IEnumerable<Pair> pairs)
    {
        int count = 0;
        foreach (Pair pair in pairs)
        {
            if (AnyOverlapContained(pair.First, pair.Last) 
                || AnyOverlapContained(pair.Last, pair.First))
            {
                count++;
            }
        }

        return count;
    }

    private static bool FullyContained(Range a, Range b) => b.Low >= a.Low && b.High <= a.High;
    private static bool AnyOverlapContained(Range a, Range b) => (b.Low >= a.Low && b.Low <= a.High) ||
                                                                 (b.High <= a.High && b.High >= a.Low);

    private IEnumerable<Pair> Parse(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            var split = line.Split(',');
            var left = ParseSingle(split[0]);
            var right = ParseSingle(split[1]);
            var pair = new Pair(left, right);
            yield return pair;
        }
    }

    private Range ParseSingle(string pairPart)
    {
        var split = pairPart.Split('-');
        return new Range(int.Parse(split[0]), int.Parse(split[1]));
    }
}
