using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day12;

[DayInfo(2023, 12)]
public class Solution12 : Solution
{
	private record Condition(string Record, int[] Groups);

    public string Run()
    {
        string[] input = this.ReadLines();

        Condition[] conditions = input.Select(ParseLine).ToArray();
        long sum = conditions.Select(SolveCached).Sum();

        Condition[] conditionsB = conditions.Select(Unfold).ToArray();
        long sumB = conditionsB.Select(SolveCached).Sum();

		return sum + "\n" + sumB;
    }

    private Condition Unfold(Condition condition)
    {
	    string record = string.Join('?', Enumerable.Repeat(condition.Record, 5));
	    int[] numbers = Enumerable.Repeat(condition.Groups, 5).SelectMany(x => x).ToArray();
		return new Condition(record, numbers);
    }

    private record CacheKey(int Group, int Position);
    private long SolveCached(Condition condition)
    {
	    int[] groups = condition.Groups;
	    int[] remaining = Enumerable.Range(0, groups.Length).Select(x => Remaining(x, groups)).ToArray();
	    Dictionary<CacheKey, long> cache = new();

	    long totalSum = CreateOptions(condition.Record, new CacheKey(0, 0));

	    return totalSum;

	    long CreateOptions(ReadOnlySpan<char> line, CacheKey key)
	    {
		    if (cache.TryGetValue(key, out long sum))
		    {
			    return sum;
		    }
		    
		    if (key.Group >= groups.Length)
		    {
			    // done
			    if (key.Position <= line.Length && line.Slice(key.Position).Contains('#'))
			    {
				    return 0;
			    }

			    return 1;
		    }

		    if (key.Position >= line.Length 
		        || key.Position + remaining[key.Group] > line.Length)
		    {
			    return 0;
		    }
		    
		    // Logic
		    char c = line[key.Position];

		    int length = groups[key.Group];
		    if (key.Position + length > line.Length)
		    {
			    return 0;
		    }

		    long insert = 0;
		    if(c is '#' or '?')
		    {
			    // check if group can be used here
			    if (IsValid(line, key.Position, length))
			    {
				    // advance to next possible position and increment group
				    insert = CreateOptions(line, new CacheKey(key.Group + 1, key.Position + length + 1));
			    }
		    }

		    long skip = 0;
		    if (c is '?' or '.')
		    {
			    // go to next position
			    skip = CreateOptions(line, key with {Position = key.Position + 1});
		    }

		    // Add to cache and return
		    sum = insert + skip;
		    cache[key] = sum;
		    return sum;
	    }
    }
    
    int Remaining(int partId, int[] groups)
    {
	    int sum = groups.Length - partId - 1;
	    for (int p = partId; p < groups.Length; p++)
	    {
		    sum += groups[p];
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

    private int Solve(Condition condition)
    {
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