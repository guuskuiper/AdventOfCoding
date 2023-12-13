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
	        //int sum0_a = Solve(test);
	        //int sum0_b = Solve3(test);

			for (int i = 0; i < conditions.Length; i++)
	        {
		        Condition cond = conditions[i];
		        int sum_a = Solve(cond);
		        int sum_b = Solve3(cond);
		        if (sum_a != sum_b)
		        {
			        List<string> solutions = SolveAndReturn(cond);
			        Console.WriteLine($"{cond.Record} {string.Join(',', cond.Groups)}");
			        Console.WriteLine(string.Join(Environment.NewLine, solutions));
		        }
	        }
        }

        //long sum = conditions.Select(Solve).Sum();
        long sum = conditions.Select(Solve3).Sum();
        return sum + "\n";

        var conditionsB = conditions.Select(Unfold).ToArray();
        long sumB = 0;
        for (int i = 0; i < conditionsB.Length; i++)
        {
			sumB += Solve3(conditionsB[i]);
			Console.WriteLine(i);
        }

		return sum + "\n" + sumB;
    }

    private Condition Unfold(Condition condition)
    {
	    string record = string.Join('?', Enumerable.Repeat(condition.Record, 5));
	    int[] numbers = Enumerable.Repeat(condition.Groups, 5).SelectMany(x => x).ToArray();
		return new Condition(record, numbers);
    }

    private int Solve3(Condition condition)
    {
	    string record = condition.Record;
	    int[] groups = condition.Groups;
	    Func<int, int, int>? cache = null;

	    int sumTotal = CachedOutputOptions(0, 0);
	    
	    return sumTotal;

	    int CachedOutputOptions(int currentOption, int currentPosition)
	    {
		    if (cache is null)
		    {
			    cache = MemoizerExtension.Memoize<int, int, int>((group, pos) =>
			    {
				    if (group >= groups.Length)
				    {
					    // done
					    if (pos <= record.Length && record.AsSpan().Slice(pos).Contains('#'))
					    {
						    return 0;
					    }
						return 1;
				    }

				    if (pos >= record.Length)
				    {
					    return 0; // invalid
				    }
					int length = groups[group];

					if (pos + length > record.Length)
					{
						return 0;
					}

				    char c = record[pos];
				    while (c is '.')
				    {
					    // skip '.'s
					    pos++;
					    
					    // impossible to complete
					    if (pos + length > record.Length) return 0;
					    
					    c = record[pos];
				    }
				    
				    int sum = 0;
				    
				    if(c is '#' or '?')
				    {
					    // check if group can be used here
					    if (IsValid(record, pos, length))
					    {
						    // advance to next possible position and increment group
						    sum += CachedOutputOptions(group + 1, pos + length + 1);
					    }
				    }
				    
				    if (c is '?')
				    {
					    // go to next position
					    sum += CachedOutputOptions(group, pos + 1);
				    }
				    
				    return sum;
			    });
		    }

		    return cache(currentOption, currentPosition);
	    }
    }

    private bool IsValid(ReadOnlySpan<char> record, int start, int length)
    {
	    bool validBefore = start == 0 || record[start - 1] != '#';
		bool validAfter = start + length == record.Length || record[start + length] != '#';
	    bool validInside = !record.Slice(start, length).Contains('.');

	    return validBefore && validAfter && validInside;
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