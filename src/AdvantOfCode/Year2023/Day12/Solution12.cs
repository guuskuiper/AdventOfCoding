using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
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

        if (false)
        {
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
			        List<string> solutions = SolveAndReturn(cond);
			        Console.WriteLine($"{cond.Record} {string.Join(',', cond.Groups)}");
			        Console.WriteLine(string.Join(Environment.NewLine, solutions));
		        }
	        }
        }

        //long sum = conditions.Select(Solve).Sum();
        long sum = conditions.Select(Solve2).Sum();


        var conditionsB = conditions.Select(Unfold).ToArray();
        long sumB = 0;
        for (int i = 0; i < conditionsB.Length; i++)
        {
	        int b = 0; //Solve2(conditionsB[i]);
			sum += b;
			//Console.WriteLine(i);
        }

        //long sumB = conditionsB.Select(Solve2).Sum();
		//long sum = -1;

		return sum + "\n" + sumB;
    }

    private Condition Unfold(Condition condition)
    {
	    string record = string.Join('?', Enumerable.Repeat(condition.Record, 5));
	    int[] numbers = Enumerable.Repeat(condition.Groups, 5).SelectMany(x => x).ToArray();
		return new Condition(record, numbers);
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

	    for (var index = 0; index < options.Length; index++)
	    {
		    PartOption current = options[index];
			// Dont start a pattern next to a '#'

			int startMin = current.StartMin;
			if (current.StartMin > 0)
			{
				while (condition.Record[startMin - 1] == '#')
				{
					startMin++;
				}
			}

			int startMax = current.StartMax;
			if ((current.StartMax + current.Length) < condition.Record.Length)
			{
				while (condition.Record[startMax + current.Length] == '#')
				{
					startMax--;
				}
			}

			options[index] = current with
			{
				StartMin = startMin,
				StartMax = startMax,
			};
	    }

	    ReadOnlySpan<char> record = condition.Record;
	    int sum = CreateOutputOptions(ref record, options, 0, 0);

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

    private int CreateOutputOptions(ref ReadOnlySpan<char> record, PartOption[] options, int currentOption, int currentPosition)
    {
	    int nextOption = currentOption + 1;
	    PartOption option = options[currentOption];
	    int sum = 0;

	    // are al options valid? for example: "?.? 1" -> startMin 0, startMax 2 -> 1 not valid
		// TODO: prevent generating to many '#'s

		int start = Math.Max(currentPosition, option.StartMin);
		if (record.Slice(currentPosition, start - currentPosition).Contains('#'))
		{
			return 0;// invalid, '#' in between 2 groups
		}

		if (nextOption < options.Length)
	    {
		    for (int i = start; i <= option.StartMax; i++)
		    {
			    if (IsValid(record, i, option.Length))
			    {
				    sum += CreateOutputOptions(ref record, options, nextOption, i + option.Length + 1);
			    }
			    else
			    {
				    
			    }

			    if (record[i] == '#')
			    {
				    break;
					// must start here / include this
			    }
		    }
		}
	    else
	    {
		    for (int i = start; i <= option.StartMax; i++)
		    {
			    if (record.Slice(i + option.Length).Contains('#'))
			    {
					if (record[i] == '#') break;
				    continue;
			    }
			    if (IsValid(record, i, option.Length))
			    {
				    sum++;
			    }
			    else
			    {
				    
			    }

			    if (record[i] == '#')
			    {
				    break;
				    // must start here / include this
			    }
			}
		}

	    return sum;
    }

    private bool IsValid(ReadOnlySpan<char> record, int start, int length)
    {
	    bool validBefore = start == 0 || record[start - 1] != '#';
		bool validAfter = start + length == record.Length || record[start + length] != '#';
	    bool validInside = !record.Slice(start, length).Contains('.');

	    return validBefore && validAfter && validInside;
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

    private List<string> SolveAndReturn(Condition condition)
    {
	    List<char> current = new List<char>();

	    return SolveStep(condition, current);
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