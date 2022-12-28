namespace AdventOfCode.Year2021.Day01;

[DayInfo(2021, 01)]
public class Solution01 : Solution
{
    public string Run()
    {
        List<string> input = InputReader.ReadFileLines();

        List<int> numbers = input.Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
        List<int> window = GetSliding3Window(numbers).ToList();

        int increases = GetIncreases(numbers);
        int increasesWindow = GetIncreases(window);

        return increases + "\n" + increasesWindow;
    }

    private int GetIncreases(IEnumerable<int> numbers)
    {
        int increases = 0;
        int prev = numbers.Take(1).Single();

        foreach (var number in numbers.Skip(1))
        {
            if(number > prev) increases++;
            prev = number;
        }

        return increases;
    }

    private IEnumerable<int> GetSliding3Window(IEnumerable<int> input)
    {
        int pp = input.Take(1).Single();
        int p = input.Skip(1).Take(1).Single();
        input = input.Skip(2);

        foreach (int i in input)
        {
            yield return pp + p + i;
            pp = p;
            p = i;
        }
    }
}
