namespace AdventOfCode.Year2022.Day04;

[DayInfo(2022, 04)]
public class Solution04 : Solution
{
    private record Range(int Low, int High)
    {
        public static Range Parse(string line)
        {
            var split = line.Split('-');
            return new Range(int.Parse(split[0]), int.Parse(split[1]));
        }
    }

    private record Pair(Range First, Range Last)
    {
        public static Pair Parse(string line)
        {
            var split = line.Split(',');
            var left = Range.Parse(split[0]);
            var right = Range.Parse(split[1]);
            return new Pair(left, right);
        }
    }
    
    public string Run()
    {
        string[] lines = this.ReadLines();
        var pairs = lines.Select(Pair.Parse).ToArray();
        var countAnyOverlap = pairs.Count(x => AnyOverlapContained(x.First, x.Last) || AnyOverlapContained(x.Last, x.First));
        var countFullyContained = pairs.Count(x => FullyContained(x.First, x.Last) || FullyContained(x.Last, x.First));
        return countFullyContained + "\n" + countAnyOverlap;
    }

    private static bool FullyContained(Range a, Range b) => b.Low >= a.Low && b.High <= a.High;
    private static bool AnyOverlapContained(Range a, Range b) => (b.Low >= a.Low && b.Low <= a.High) ||
                                                                 (b.High <= a.High && b.High >= a.Low);
}
