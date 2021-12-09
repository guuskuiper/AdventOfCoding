using System.Linq;

namespace AdventOfCode.Day08;

public class Solution08 : Solution
{
    private int[] digitCount = new int[10];
    private int valueSum = 0;
    private Dictionary<string, int> patternDict;
    public string Run()
    {
        var input = InputReader.ReadFileLines();

        ParseLines(input);
        var A = digitCount[1] + digitCount[4] + digitCount[7] + digitCount[8];
        return A + "\n" + valueSum;
    }

    private void ParseLines(List<string> lines)
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
        patternDict = new Dictionary<string, int>();
        var words = pattern.Trim().Split();
        var one = words.Where(x => x.Length == 2).Take(1).Single();
        patternDict[one] = 1;
        
        var four = words.Where(x => x.Length == 4).Take(1).Single();
        patternDict[four] = 4;
        
        patternDict[words.Where(x => x.Length == 7).Take(1).Single()] = 8;
        patternDict[words.Where(x => x.Length == 3).Take(1).Single()] = 7;

        string fourNotOne = "";
        foreach (var c in four)
        {
            if (!one.Contains(c))
            {
                fourNotOne += c;
            }
        }

        Analyze5(words, one, fourNotOne);
        Analyze6(words, one, fourNotOne);
    }

    private void Analyze5(string[] words, string one, string fourNotOne)
    {
        //2, 3, 5 -> 3 both from 1
        //2, 5 -> both from 1 -> 4
        var l5 = words.Where(x => x.Length == 5).ToList();
        foreach (var s in l5)
        {
            if (FindAllPattern(s, one))
            {
                patternDict[s] = 3;
                l5.Remove(s);
                break;
            }
        }

        foreach (var s in l5)
        {
            if (FindAllPattern(s, fourNotOne))
            {
                patternDict[s] = 5;
                l5.Remove(s);
                break;
            }
        }

        patternDict[l5.Single()] = 2;
    }
    
    private void Analyze6(string[] words, string one, string fourNotOne)
    {
        var l6 = words.Where(x => x.Length == 6).ToList();

        //0, 6, 9 -> 0 not both of the new from 1 -> 4
        //6, 9 -> 6 not both from 1
        foreach (var s in l6)
        {
            if (!FindAllPattern(s, fourNotOne))
            {
                patternDict[s] = 0;
                l6.Remove(s);
                break;
            }
        }

        foreach (var s in l6)
        {
            if (!FindAllPattern(s, one))
            {
                patternDict[s] = 6;
                l6.Remove(s);
                break;
            }
        }

        patternDict[l6.Single()] = 9;
    }

    private bool FindAllPattern(string s, string p)
    {
        bool found = true;
        foreach (var c in p)
        {
            if (!s.Contains(c))
            {
                found = false;
                break;
            }
        }

        return found;
    }

    private void DecodeDigits(string text)
    {
        var digits = text.Trim().Split();

        int value = 0;

        foreach (var digit in digits)
        {
            var d = Dict2Digit(digit);
            if (d >= 0)
            {
                value *= 10;
                value += d;
                digitCount[d]++;
            }
        }

        valueSum += value;
    }

    private int Dict2Digit(string text)
    {
        foreach ((string key, int value) in patternDict)
        {
            if(text.Length != key.Length) continue;

            if (FindAllPattern(text, key))
            {
                return value;
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
