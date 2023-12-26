using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode.Year2022.Day10;

[DayInfo(2022, 10)]
public class Solution10 : Solution
{
    private const char FullBlock = (char)0x2588;
    private const char VisibleChar = FullBlock;
    private const char InvisibleChar = ' ';
    
    public string Run()
    {
        string[] lines = this.ReadLines();
        List<int?> instructions = Parse(lines);
        List<int> during = Cycles(instructions);
        int sum = SignalStrengthSum(during, 20, 40, 220);
        string crt = CRT(during);
        return sum + "\n" + crt;
    }

    private string CRT(List<int> during)
    {
        const int Width = 40;
        const int Height = 6;

        StringBuilder sb = new();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int pixel = x;
                int cycle = 1 + x + Width * y;
                int spriteMiddlePosition = during[cycle];
                bool isVisible = pixel >= spriteMiddlePosition - 1 &&
                                 pixel <= spriteMiddlePosition + 1;
                sb.Append(isVisible ? VisibleChar : InvisibleChar);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private int SignalStrengthSum(List<int> during, int start, int increment, int end)
    {
        int sum = 0;
        for (int i = start; i <= end; i += increment)
        {
            sum += i * during[i];
        }
        return sum;
    }

    private List<int> Cycles(List<int?> instructions)
    {
        int x = 1;
        List<int> during = new List<int>();
        during.Add(0);

        foreach (var instruction in instructions)
        {
            during.Add(x);
            if (instruction.HasValue)
            {
                during.Add(x);
                x += instruction.Value;
            }
        }

        return during;
    }

    private List<int?> Parse(string[] lines)
    {
        List<int?> instructions = new();
        foreach (var line in lines)
        {
            string[] split = line.Split(' ');
            int? add;
            if (split.Length == 2) add = int.Parse(split[1]);
            else add = null;
            instructions.Add(add);
        }

        return instructions;
    }
}
