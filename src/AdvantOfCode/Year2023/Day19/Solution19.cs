namespace AdventOfCode.Year2023.Day19;

[DayInfo(2023, 19)]
public class Solution19 : Solution
{
	private record Workflow(string Label, Rule[] Rules);
	private record Rule(string ResultingLabel, Func<Part, bool> Condition, char Category, char Operator, int Limit, string Debug);
	private record Part(int X, int M, int A, int S);

	private readonly record struct PartRange(int Xmin, int Xmax, int Mmin, int Mmax, int Amin, int Amax, int Smin, int Smax, string Label)
	{
		public long Count => (long)(Xmax - Xmin + 1) * (Mmax - Mmin + 1) * (Amax - Amin + 1) * (Smax - Smin + 1);

		public (PartRange low, PartRange high) Split(char category, char op, int number)
		{
			int limit = op == '>' ? number : number - 1;
			PartRange low;
			PartRange high;

			if (category == 'x')
			{
				low = this with { Xmax = limit };
				high = this with { Xmin = limit + 1 };
			}
			else if (category == 'm')
			{
				low = this with { Mmax = limit };
				high = this with { Mmin = limit + 1 };
			}
			else if(category == 'a')
			{
				low = this with { Amax = limit };
				high = this with { Amin = limit + 1 };
			}
			else if(category == 's')
			{
				low = this with { Smax = limit };
				high = this with { Smin = limit + 1 };
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(category));
			}

			return (low, high);
		}
	}

	public string Run()
    {
        string[] input = this.ReadLines(StringSplitOptions.None);

        (Workflow[] workflows, Part[] parts) = ParseLines(input);

        Dictionary<string, Workflow> workflowMap = workflows.ToDictionary(x => x.Label, x => x);

        List<Part> acceptedParts = [];
        foreach (var part in parts)
        {
	        bool accepted = IsAccepted(workflowMap, part);
	        if (accepted)
	        {
				acceptedParts.Add(part);
	        }
        }

        long sum = acceptedParts.Select(p => p.X + p.M + p.A + p.S).Sum();

        PartRange start = new PartRange(1, 4000, 1, 4000, 1, 4000, 1, 4000, "in");

        List<PartRange> acceptedRanges = ProcessRange(workflowMap, start);

        long sumB = acceptedRanges.Select(p => p.Count).Sum();

        return sum + "\n" + sumB;
    }

	private List<PartRange> ProcessRange(Dictionary<string, Workflow> workflows, PartRange partRange)
	{
		if (partRange.Label == "R") return [];
		if (partRange.Label == "A") return [partRange];

		List<PartRange> acceptedRanges = [];

		PartRange remaining = partRange;

		Workflow workflow = workflows[remaining.Label];
		foreach (var rule in workflow.Rules)
		{
			if (rule.Category is ' ')
			{
				acceptedRanges.AddRange(ProcessRange(workflows, remaining with { Label = rule.ResultingLabel }));
				break;
			}

			(PartRange low, PartRange high) = remaining.Split(rule.Category, rule.Operator, rule.Limit);

			if (rule.Condition(new Part(low.Xmin, low.Mmin, low.Amin, low.Smin)))
			{
				acceptedRanges.AddRange(ProcessRange(workflows, low with { Label = rule.ResultingLabel }));
				remaining = high;
			}
			else if(rule.Condition(new Part(high.Xmin, high.Mmin, high.Amin, high.Smin)))
			{
				acceptedRanges.AddRange(ProcessRange(workflows, high with { Label = rule.ResultingLabel }));
				remaining = low;
			}
			else
			{
				throw new Exception("Invalid rule");
			}
		}

		return acceptedRanges;
	}

	private bool IsAccepted(Dictionary<string, Workflow> workflows, Part part)
	{
		const string IN = "in";

		string nextLabel = IN;
		while (nextLabel is not ("R" or "A"))
		{
			Workflow workflow = workflows[nextLabel];
			foreach (var rule in workflow.Rules)
			{
				if (rule.Condition(part))
				{
					nextLabel = rule.ResultingLabel;
					break;
				}
			}
		}

		return nextLabel == "A";
	}

    private (Workflow[] workflows, Part[] parts) ParseLines(string[] lines)
    {
	    List<Workflow> rules = [];
	    List<Part> parts = [];
	    bool isPart = false;
	    foreach (var line in lines)
	    {
		    if (string.IsNullOrEmpty(line))
		    {
			    isPart = true;
				continue;
		    }

			LineReader reader = new(line);
			if (isPart)
			{
				parts.Add(ParsePart(ref reader));
			}
			else
			{
				rules.Add(ParseRule(ref reader));
			}
	    }

		return (rules.ToArray(), parts.ToArray());
	}

    private Workflow ParseRule(ref LineReader reader)
    {
	    string label = reader.ReadLetters().ToString();
	    List<Rule> rules = [];
	    reader.ReadChar('{');

	    while (reader.IsLetter)
	    {
		    Rule rule;
			var letters = reader.ReadLetters();
			if (letters.Length == 1 && char.IsLower(letters[0]))
			{
				// x, m, a, s + {>,<} + number + : + label / R / A
				char c = letters[0];
				char op = reader.ReadChar();
				int number = reader.ReadInt();
				reader.ReadChar(':');
				string resultLabel = reader.ReadLetters().ToString();
				Func<Part, bool> condition = (c, op) switch
				{
					('x', '>') => part => part.X > number,
					('m', '>') => part => part.M > number,
					('a', '>') => part => part.A > number,
					('s', '>') => part => part.S > number,
					('m', '<') => part => part.M < number,
					('a', '<') => part => part.A < number,
					('x', '<') => part => part.X < number,
					('s', '<') => part => part.S < number,
					_ => throw new ArgumentOutOfRangeException()
				};
				string debug = $"{c} {op} {number} : {resultLabel}";
				rule = new Rule(resultLabel, condition, c, op, number, debug);
			}
			else
			{
				// R, A or label
				string resultingLabel = letters.ToString();
				rule = new Rule(resultingLabel, _ => true, ' ', ' ', 0, resultingLabel);
			}

			rules.Add(rule);

			char next = reader.ReadChar();
			if(next == '}') break;
			// ',' continue
	    }

	    return new Workflow(label, rules.ToArray());
    }

    private Part ParsePart(ref LineReader reader)
    {
        reader.ReadChars("{x=");
        int x = reader.ReadInt();
        reader.ReadChars(",m=");
	    int m = reader.ReadInt();
        reader.ReadChars(",a=");
	    int a = reader.ReadInt();
	    reader.ReadChars(",s=");
	    int s = reader.ReadInt();
        reader.ReadChars("}");

	    return new Part(x, m, a, s);
    }
}    