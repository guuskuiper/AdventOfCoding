using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day16;

[DayInfo(2019, 16)]
public class Solution16 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        int[] digits = input[0].Select(char.GetNumericValue).Select(Convert.ToInt32).ToArray();

        var fft = new FFT(digits, 100);
        string partA = fft.FFTExact();

        var digits2 = digits.Repeat(10_000).ToArray();
        var fft2 = new FFT(digits2, 100);
        var offset = int.Parse(input[0].Substring(0, 7));
        string partB = fft2.FFTSimple(offset);
        return partA + "\n" + partB;
    }
}    