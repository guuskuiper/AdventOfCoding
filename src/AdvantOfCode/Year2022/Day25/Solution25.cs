namespace AdventOfCode.Year2022.Day25;

[DayInfo(2022, 25)]
public class Solution25 : Solution
{
    private const int SNAFU_BASE = 5;
    
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        var values = Convert(lines).ToArray();
        long sum = values.Sum();
        string result = SNAFU(sum);
        
        return result + "\n";
    }

    private IEnumerable<long> Convert(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            yield return Convert(line);
        }
    }

    private string SNAFU(long number)
    {
        Stack<char> snafu = new();
        
        long remaining = number;
        bool carry = false;
        while (remaining > 0 || carry)
        {
            int mod = (int)(remaining % SNAFU_BASE);
            if (carry) mod++;
            if (mod <= 2)
            {
                snafu.Push(ToSNAFU(mod));
                carry = false;
            }
            else
            {
                snafu.Push(ToSNAFU(mod - SNAFU_BASE));
                carry = true;
            }
            remaining /= SNAFU_BASE;
        }

        return new string(snafu.ToArray());
    }

    private long Convert(string line)
    {
        long currentBase = 1;
        long sum = 0;
        for (int i = line.Length - 1; i >= 0; i--)
        {
            char c = line[i];
            long value = Convert(c);
            sum += value * currentBase;
            currentBase *= SNAFU_BASE;
        }
        return sum;
    }

    private int Convert(char c)
    {
        return c switch
        {
            '2' => 2,
            '1' => 1,
            '0' => 0,
            '-' => -1,
            '=' => -2,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }
    
    private char ToSNAFU(int c)
    {
        return c switch
        {
            2 => '2',
            1 => '1',
            0 => '0',
            -1 => '-',
            -2 => '=',
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }
}
