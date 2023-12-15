using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day15;

[DayInfo(2023, 15)]
public class Solution15 : Solution
{
	private record Step(string Label, char Operation, int Number);

	private record Lens(string Label, int FocalLength);

	private record Box
	{
		public List<Lens> LensSlots { get; } = new();
	}

    public string Run()
    {
        string[] input = this.ReadLines();
        string[] steps = input[0].Split(',');

        long sum = steps.Select(Hash).Sum();

        Step[] parsedSteps = steps.Select(ParseStep).ToArray();
        Box[] boxed = new Box[256];
        for (int i = 0; i < boxed.Length; i++)
        {
	        boxed[i] = new Box();
        }

        foreach (var parsedStep in parsedSteps)
        {
	        Execute(boxed, parsedStep);
        }

        int totalPower = 0;
        for (var index = 0; index < boxed.Length; index++)
        {
	        var box = boxed[index];
	        int power = 0;
	        for (int lens = 0; lens < box.LensSlots.Count; lens++)
	        {
		        int lensPower = (index + 1) * (lens + 1) * box.LensSlots[lens].FocalLength;
				power += lensPower;
	        }
			totalPower += power;
        }

        return sum + "\n" + totalPower;
    }

    private void Execute(Box[] boxes, Step step)
    {
	    int boxNumber = Hash(step.Label);
		Box box = boxes[boxNumber];

		int index = box.LensSlots.FindIndex(x => x.Label == step.Label);

		if (step.Operation == '-')
	    {
		    if (index >= 0)
		    {
			    box.LensSlots.RemoveAt(index);
		    }
	    }
	    else
	    {
		    if (index >= 0)
		    {
				box.LensSlots[index] = new Lens(step.Label, step.Number);
		    }
		    else
		    {
				box.LensSlots.Add(new Lens(step.Label, step.Number));   
		    }
	    }
    }

    private int Hash(string step)
    {
        int hash = 0;
	    foreach (var c in step)
	    {
		    int value = c;
		    hash += value;
		    hash *= 17;
		    hash %= 256;
	    }
        return hash;
    }

    private Step ParseStep(string step)
    {
	    LineReader reader = new(step);
	    string label = reader.ReadLetters().ToString();
	    char operation = reader.ReadChar();
	    int number = 0;
		if (operation == '=')
	    {
		    number = reader.ReadInt();
	    }
	    return new Step(label, operation, number);
    }
}    