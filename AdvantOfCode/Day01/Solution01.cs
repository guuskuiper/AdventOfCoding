using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode.Day01;

public class Solution01 : Solution
{
    public string Run()
    {
        string input = File.ReadAllText("Day01/input01.txt");
        List<int> numbers = input.Split('\n').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
        List<int> window = GetSliding3Window(numbers).ToList();

        int increases = GetIncreases(numbers);
        int increasesWindow = GetIncreases(window);

        return increases + "," + increasesWindow ;
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
