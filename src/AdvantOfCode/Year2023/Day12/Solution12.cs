using System.Collections.Immutable;
using System.Diagnostics;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day12;

[DayInfo(2023, 12)]
public class Solution12 : Solution
{
	private record Condition(string Record, int[] Groups);

	private record PartOption(int Length, int StartMin, int StartMax)
	{
		public int EndMax => StartMax + Length - 1;
	};

    public string Run()
    {
        string[] input = this.ReadLines();

        var conditions = input.Select(ParseLine).ToArray();

        Condition test = conditions[2];
        int sum0_a = Solve(test);
        int sum0_b = Solve2(test);

        for (int i = 0; i < conditions.Length; i++)
        {
	        Condition cond = conditions[i];
	        int sum_a = Solve(cond);
	        int sum_b = Solve2(cond);
	        if (sum_a != sum_b)
	        {

	        }
        }

        long sum = conditions.Select(Solve).Sum();
        //long sum = -1;

        return sum + "\n";
    }

    private int Solve2(Condition condition)
    {
	    string[] parts = condition.Groups.Select(x => new string('#', x)).ToArray();

	    PartOption[] options = new PartOption[parts.Length];

	    for (int p = 0; p < parts.Length; p++)
	    {
			int currentLength = parts[p].Length;
		    int startMin = Previous(p);
		    int remaining = Remaining(p);
			int startMax = condition.Record.Length - remaining - currentLength;

			options[p] = new PartOption(currentLength, startMin, startMax);
	    }

		Simplify(condition, options);

	    for (var index = 0; index < options.Length; index++)
	    {
			PartOption current = options[index];
			if(current.StartMin == current.StartMax) continue; // cant change

		    if (index > 0)
		    {
				PartOption previous = options[index - 1];
				int limit = previous.StartMin + previous.Length + 1;
				current = current with
				{
					StartMin = Math.Max(current.StartMin, limit),
					StartMax = Math.Max(current.StartMax, limit),
				};
		    }

		    if (index < options.Length - 1)
		    {
			    PartOption next = options[index + 1];
			    int limit = next.StartMax - current.Length - 1;
			    current = current with
			    {
				    StartMax = Math.Min(current.StartMax, limit),
				    StartMin = Math.Min(current.StartMin, limit),
			    };
			}

		    options[index] = current;
	    }

	    Simplify(condition, options);

	    int sum = CreateOutputOptions(options, 0, 0);

	    return sum;

	    int Remaining(int partId)
	    {
		    int sum = condition.Groups.Length - partId - 1;
		    for (int p = partId + 1; p < condition.Groups.Length; p++)
		    {
			    sum += condition.Groups[p];
		    }
		    return sum;
	    }

	    int Previous(int partId)
	    {
		    int sum = partId;
		    for(int p = 0; p < partId; p++)
		    {
			    sum += condition.Groups[p];
		    }
		    return sum;
		}
    }

    private int CreateOutputOptions(PartOption[] options, int currentOption, int currentPosition)
    {
	    int nextOption = currentOption + 1;
	    PartOption option = options[currentOption];
	    int sum = 0;
	    int start = Math.Max(currentPosition, option.StartMin);
		if (nextOption < options.Length)
	    {
		    for (int i = start; i <= option.StartMax; i++)
		    {
			    sum += CreateOutputOptions(options, nextOption, i + option.Length + 1);
		    }
		}
	    else
	    {
		    sum = option.StartMax - start + 1;
		}

	    return sum;
    }

    private void Simplify(Condition condition, PartOption[] options)
    {
	    for (int p = 0; p < options.Length; p++)
	    {
		    PartOption part = options[p];
		    int startMin = part.StartMin;
		    int startMax = part.StartMax;
		    int indexOfDot = 0;
		    while (indexOfDot >= 0)
		    {
			    var slice = condition.Record.AsSpan(startMin, part.Length);
			    indexOfDot = slice.IndexOf('.');
			    if (indexOfDot >= 0)
			    {
				    startMin += indexOfDot + 1;
			    }
		    }

		    int lastIndexOfDot = 0;
		    while (lastIndexOfDot >= 0)
		    {
			    var slice = condition.Record.AsSpan(startMax, part.Length);
			    lastIndexOfDot = slice.IndexOf('.');
			    if (lastIndexOfDot >= 0)
			    {
				    startMax -= (part.Length - lastIndexOfDot);
			    }
		    }

		    options[p] = part with
		    {
			    StartMax = startMax,
			    StartMin = startMin,
		    };
	    }
	}

    private int Solve(Condition condition)
    {
		//ImmutableArray<char> chars = [];
		//var c2 = chars.Add('a');

		List<char> current = new List<char>();

        List<string> options = SolveStep(condition, current);

        return options.Count;
    }

    private List<string> SolveStep(Condition condition, List<char> chars)
    {
	    int pos = chars.Count;

	    if (pos >= condition.Record.Length)
	    {
		    if (!IsValid(chars, condition.Groups))
		    {
			    return [];
		    }
		    string output = new string(chars.ToArray());
		    return [output];
	    }

	    char c = condition.Record[pos];

	    if (c == '?')
	    {
		    // 1-2 options
		    List<string> output = [];

		    List<char> copy = chars.ToList();
            copy.Add('.');
            output.AddRange(SolveStep(condition, copy));
            
            chars.Add('#');
			output.AddRange(SolveStep(condition, chars));

			return output;
	    }
	    else
	    {
		    // 1 option
            chars.Add(c);
		    return SolveStep(condition, chars);
	    }
	}

    private bool IsValid(List<char> chars, int[] groups)
    {
	    bool valid = true;
	    int groupId = 0;
	    int group = 0;
	    for (int i = 0; i < chars.Count; i++)
	    {
            char c = chars[i];
            if (c == '.')
            {
	            CloseGroup();

            }
            else if(c == '#')
            {
	            group++;
            }
            else
            {
	            throw new Exception();
            }
	    }

	    CloseGroup();

		if (groupId != groups.Length)
	    {
		    valid = false;
	    }

		return valid;

		void CloseGroup()
		{
			if (group > 0)
			{
				if (groupId >= groups.Length || groups[groupId] != group)
				{
					valid = false;
				}
				groupId++;
				group = 0;
			}
		}
    }

    private Condition ParseLine(string line)
    {
        LineReader reader = new(line);

        string record = reader.ReadWhen(c => c is '.' or '#' or '?').ToString();
        reader.SkipWhitespaces();
        List<int> numbers = [];
        while (!reader.IsDone)
        {
	        int number = reader.ReadInt();
            numbers.Add(number);
            if(reader.IsDone) break;
            reader.ReadChar(',');
        }

        return new Condition(record, numbers.ToArray());
    }

}    