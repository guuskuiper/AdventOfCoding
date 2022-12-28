namespace AdventOfCode.Year2021.Day07;

[DayInfo(2021, 07)]
public class Solution07 : Solution
{
    private int _max;
    public string Run()
    {
        var input = InputReader.ReadFile();
        var numbers =  input.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
        _max = numbers.Max();

        var A = GetMinDistance(numbers, Distance);
        var B = GetMinDistance(numbers, DistanceIncreasing);
        
        return A + "\n" + B;
    }

    private long GetMinDistance(List<int> numbers, Func<IEnumerable<int>, long, long> distance)
    {
        var dMin = long.MaxValue;
        var m = -1;
        for (int i = 0; i < _max; i++)
        {
            var d = distance(numbers, i);
            if (d < dMin)
            {
                dMin = d;
                m = i;
            }
        }

        return dMin;
    }

    private long Mean(IEnumerable<int> numbers)
    {
        long sum = 0;
        long count = 0;
        foreach (var number in numbers)
        {
            sum += number;
            count++;
        }

        double mean = sum / (double)count;

        return (long)Math.Round(mean);
    }

    private long Distance(IEnumerable<int> numbers, long value)
    {
        long distance = 0;
        foreach (var number in numbers)
        {
            distance += Math.Abs(number - value);
        }

        return distance;
    }
    
    private long DistanceIncreasing(IEnumerable<int> numbers, long value)
    {
        long distance = 0;
        foreach (var number in numbers)
        {
            long n = Math.Abs(number - value);
            distance += SumUntil(n);
        }

        return distance;
    }

    private long SumUntil(long n)
    {
        return n * (n + 1) / 2;
    }
}
