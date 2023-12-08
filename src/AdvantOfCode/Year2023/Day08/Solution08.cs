using System.Collections.Frozen;
using AdventOfCode.Extensions;
using AdventOfCode.Graph;

namespace AdventOfCode.Year2023.Day08;

[DayInfo(2023, 08)]
public class Solution08 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();

        string instructions = input[0];
        
        Dictionary<string, List<string>> nodes = new();
        foreach (var s in input.Skip(1))
        {
            ParseLine(nodes, s);
        }

        FrozenDictionary<string, List<string>> frozenDictionary = nodes.ToFrozenDictionary();

        int steps = StepAAAToZZZ(frozenDictionary, instructions);

        long stepsB = StepXXAToXXZ(frozenDictionary, instructions);
        
        return steps + "\n" + stepsB;
    }
    
    private long StepXXAToXXZ(FrozenDictionary<string, List<string>> nodes, string instructions)
    {
        string[] currents = nodes.Keys.Where(x => x.EndsWith('A')).ToArray();
        long[] periods = currents.Select(x => FindPeriod(nodes, instructions,x)).ToArray();
        return periods.LCM();
    }

    private long FindPeriod(FrozenDictionary<string, List<string>> nodes, string instructions, string start)
    {
        long steps = 0;

        string current = start;

        string? target = null;

        while (current != target)
        {
            char c = instructions[(int) (steps % instructions.Length)];
            int index = c == 'L' ? 0 : 1;

            List<string> neighbours = nodes[current];
            current = neighbours[index];

            if (target is null && current.EndsWith('Z'))
            {
                target = current;
            }
            steps++;
        }

        return steps;
    }

    private int StepAAAToZZZ(FrozenDictionary<string, List<string>> nodes, string instructions)
    {
        const string Target = "ZZZ";
        int steps = 0;

        string current = "AAA";

        while (current != Target)
        {
            char c = instructions[steps % instructions.Length];
            int index = c == 'L' ? 0 : 1;

            List<string> neighbours = nodes[current];
            current = neighbours[index];

            steps++;
        }

        return steps;
    }

    private void ParseLine(Dictionary<string, List<string>> nodes, string line)
    {
        LineReader reader = new(line);
        string node = reader.ReadLetters().ToString();
        reader.ReadChars(" = (");
        string left = reader.ReadLetters().ToString();
        reader.ReadChars(", ");
        string right = reader.ReadLetters().ToString();
        reader.ReadChars(")");
        
        nodes.Add(node, [left, right]);
    }
}    