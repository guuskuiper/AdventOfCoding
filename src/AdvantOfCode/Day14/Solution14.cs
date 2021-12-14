using System.Text;
using AdventOfCode.Extentions;

namespace AdventOfCode.Day14;

public class Solution14 : Solution
{
    private readonly string _polymereTemplateInput;
    private string _polymereTemplate;
    private Dictionary<string, char> _pairInsertions;
    private Dictionary<string, long> _elementPairs;

    public Solution14()
    {
        var lines = InputReader.ReadFileLinesArray();
        _polymereTemplateInput = lines[0];
        _polymereTemplate = lines[0];
        ParsePairs(lines[1..]);
    }
    public string Run()
    {
        Steps(10, Step);
        long A = GetScore(FillScoresA);
        
        _elementPairs = GetElements(_polymereTemplateInput);
        Steps(40, StepEfficient);
        long B = GetScore(FillScoresB);
        
        return A + "\n" + B;
    }

    private void Steps(int stepCount, Action step)
    {
        for (int i = 0; i < stepCount; i++)
        {
            step();
        }
    }

    private void Step()
    {
        StringBuilder sb = new();

        char prev = _polymereTemplate[0];
        sb.Append(prev);
        for (int i = 1; i < _polymereTemplate.Length; i++)
        {
            char current = _polymereTemplate[i];

            string pair = $"{prev}{current}";

            if (_pairInsertions.TryGetValue(pair, out char insertion))
            {
                sb.Append(insertion);
            }
            sb.Append(current);
            
            prev = current;
        }

        _polymereTemplate = sb.ToString();
    }

    private void StepEfficient()
    {
        Dictionary<string, long> elements = new();
        foreach (KeyValuePair<string,long> kvp in _elementPairs)
        {
            if (_pairInsertions.TryGetValue(kvp.Key, out char insertion))
            {
                string element1 = $"{kvp.Key[0]}{insertion}";
                string element2 = $"{insertion}{kvp.Key[1]}";
                
                elements.AddOrCreate(element1, kvp.Value);
                elements.AddOrCreate(element2, kvp.Value);
            }
            else
            {
                elements.AddOrCreate(kvp.Key, kvp.Value);
            }
        }

        _elementPairs = elements;
    }
    
    
    private long GetScore(Action<Dictionary<char, long>> fillScores)
    {
        Dictionary<char, long> scores = new();

        fillScores(scores);

        return HighMinusLowCount(scores);
    }

    private void FillScoresA(Dictionary<char, long> scores)
    {
        foreach (var c in _polymereTemplate)
        {
            scores.AddOrCreate(c, 1);
        }
    }
    
    private void FillScoresB(Dictionary<char, long> scores)
    {
        // all elements are counted double except the 1st and last element in the original input
        scores[_polymereTemplateInput[0]] = 1;
        scores[_polymereTemplateInput[^1]] = 1;
        
        foreach (var element in _elementPairs)
        {
            foreach (var c in element.Key)
            {
                scores.AddOrCreate(c, element.Value);
            }
        }

        foreach (char c in scores.Keys)
        {
            scores[c] /= 2;
        }
    }

    private long HighMinusLowCount(Dictionary<char, long> scores)
    {
        var sort = scores.ToList();
        sort.Sort( (kvp1, kvp2) => kvp1.Value.CompareTo(kvp2.Value));
        return sort[^1].Value - sort[0].Value;
    }

    private void ParsePairs(IEnumerable<string> pairs)
    {
        _pairInsertions = pairs
            .Select(p => p.Split(" -> "))
            .ToDictionary(s => s[0], s => s[1][0]);
    }
    
    private static Dictionary<string, long> GetElements(string polymere)
    {
        Dictionary<string, long> elements = new();

        for (var index = 0; index < polymere.Length - 1; index++)
        {
            string element = polymere.Substring(index, 2);

            elements.AddOrCreate(element, 1);
        }

        return elements;
    }
}
