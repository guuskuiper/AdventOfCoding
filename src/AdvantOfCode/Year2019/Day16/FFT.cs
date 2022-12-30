namespace AdventOfCode.Year2019.Day16;

public class FFT
{
    private readonly int[] BASE = {0, 1, 0, -1};

    private int[] Input; 
    private int Phases;

    public FFT(IEnumerable<int> input, int phases)
    {
        Input = input.ToArray();
        Phases = phases;
    }

    public string FFTExact()
    {
        var res = Input;
        for(int i = 0; i < Phases; i++)
        {
            res = Phase(res);
        }

        return string.Join(string.Empty, res.Take(8));
    }

    public string FFTSimple(int offset)
    {
        var res = Input;
        for(int i = 0; i < Phases; i++)
        {
            res = PhaseSimple(res); 
        }

        return string.Join(string.Empty, res.Skip(offset).Take(8));
    }

    private int[] PhaseSimple(int[] input)
    {
        var result = new int[input.Length];
        int prevSum = 0;
        for (int i = input.Length - 1; i >= input.Length / 2 ; i--)
        {
            prevSum += input[i];
            result[i] = prevSum % 10;
        }
        return result;
    }

    private int[] Phase(int[] input)
    {
        var result = new int[input.Length];
        for (int r = 0; r < input.Length; r++)
        {
            var pattern = Pattern(input.Length, r + 1);
            var accumulate = 0;
            for (int i = 0; i < input.Length; i++)
            {
                accumulate += input[i] * pattern[i];
            }
            result[r] = Math.Abs(accumulate) % 10;
        }

        return result;
    }

    private int[] Pattern(int length, int repeat)
    {
        return BASE.SelectMany(x => Enumerable.Repeat(x, repeat)).Repeat(1 + (int)Math.Ceiling((double)length / (repeat * BASE.Length))).Skip(1).Take(length).ToArray();
    }
}