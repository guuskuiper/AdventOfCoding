namespace AdventOfCode.Year2021.Day08;

[DayInfo(2021, 08)]
public class Solution08 : Solution
{
    private int[] digitCount = new int[10];
    private int valueSum = 0;

    private int[] encoding;
    
    public string Run()
    {
        string[] input = this.ReadLines();

        ParseLines(input);
        var A = digitCount[1] + digitCount[4] + digitCount[7] + digitCount[8];
        return A + "\n" + valueSum;
    }

    private void ParseLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            ParseLine(line);
        }
    }

    private void ParseLine(string line)
    {
        var split = line.Split('|');
        var pattern = split[0];
        var digits = split[1];
        
        AnalyzePattern(pattern);
        DecodeDigits(digits);
    }

    private void AnalyzePattern(string pattern)
    {
        encoding = new int[10];
        var words = pattern.Trim().Split();
        
        encoding[1] = Encode(GetSingleByLength(words, 2));
        encoding[4] = Encode(GetSingleByLength(words, 4));
        encoding[8] = Encode(GetSingleByLength(words, 7));
        encoding[7] = Encode(GetSingleByLength(words, 3));

        int fourNotOne = encoding[4] & ~encoding[1];

        Analyze5(words, encoding[1], fourNotOne);
        Analyze6(words, encoding[1], fourNotOne);
    }

    private string GetSingleByLength(string[] words, int length)
        => words.Where(x => x.Length == length).Take(1).Single();

    private int Encode(string pattern)
        => pattern.Aggregate(0, (i, c) => i | 1 << c - 'a');

    private IEnumerable<int> EncodeByLength(string[] words, int length)
        => words.Where(x => x.Length == length).Select(Encode);
    
    private void Analyze5(string[] words, int one, int fourNotOne)
    {
        var l5 = EncodeByLength(words, 5).ToList();
        
        //2, 3, 5 -> 3 both from 1
        //2, 5 -> both from 1 -> 4
        foreach (var s in l5)
        {
            if (FindContainsAll(s, one))
            {
                encoding[3] = s;
                l5.Remove(s);
                break;
            }
        }

        foreach (var s in l5)
        {
            if (FindContainsAll(s, fourNotOne))
            {
                encoding[5] = s;
                l5.Remove(s);
                break;
            }
        }

        encoding[2] = l5.Single();
    }
    
    private void Analyze6(string[] words, int one, int fourNotOne)
    {
        var l6 = EncodeByLength(words, 6).ToList();

        //0, 6, 9 -> 0 not both of the new from 1 -> 4
        //6, 9 -> 6 not both from 1
        foreach (var s in l6)
        {
            if (!FindContainsAll(s, fourNotOne))
            {
                encoding[0] = s;
                l6.Remove(s);
                break;
            }
        }

        foreach (var s in l6)
        {
            if (!FindContainsAll(s, one))
            {
                encoding[6] = s;
                l6.Remove(s);
                break;
            }
        }

        encoding[9] = l6.Single();
    }

    private bool FindContainsAll(int word, int pattern)
    {
        return (word & pattern) == pattern;
    }

    private void DecodeDigits(string text)
    {
        var digits = text.Trim().Split();

        int value = 0;
        foreach (var digit in digits)
        {
            var d = Decode(digit);
            if (d >= 0)
            {
                value *= 10;
                value += d;
                digitCount[d]++;
            }
        }

        valueSum += value;
    }

    private int Decode(string text)
    {
        int textEncoded = Encode(text);

        for (var digit = 0; digit < encoding.Length; digit++)
        {
            var e = encoding[digit];
            if (e == textEncoded)
            {
                return digit;
            }
        }

        return -1;
    }

    private int Text2SimpleDigit(string text)
    {
        return text.Length switch
        {
            2 => 1,
            3 => 7,
            4 => 4,
            7 => 8,
            _ => -1,
        };
    }
}
