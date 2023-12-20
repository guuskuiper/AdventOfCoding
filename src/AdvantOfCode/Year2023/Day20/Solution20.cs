using System.Text;

namespace AdventOfCode.Year2023.Day20;

[DayInfo(2023, 20)]
public class Solution20 : Solution
{
	private class State
	{
		public bool Value { get; set; } = false;
		public Dictionary<string, bool> Inputs { get; set; } = [];
	}

	private record Module(string Name, char Type, string[] Outputs)
	{
		public State State = new();
	}

	public string Run()
	{
		string example = """
		                 broadcaster -> a, b, c
		                 %a -> b
		                 %b -> c
		                 %c -> inv
		                 &inv -> a
		                 """;
        //string[] input = example.Split(Environment.NewLine);
        string[] input = this.ReadLines();

        Module[] modules = input.Select(ParseLine).ToArray();

        Dictionary<string, Module> moduleMap = modules.ToDictionary(x => x.Name, x => x);


        List<Module> outputModules = [];

		// Initialize conjunction modules
		foreach (var module in modules)
        {
	        foreach (var output in module.Outputs)
	        {
		        if (!moduleMap.ContainsKey(output))
		        {
					continue;
		        }

		        var outputModule = moduleMap[output];
		        if (outputModule.Type == '&')
		        {
			        outputModule.State.Inputs[module.Name] = false;
		        }
	        }
        }

		string mermaid = ExportMermaid(modules);
        long sum = PulseSum(moduleMap, 1000);

        return sum + "\n";
    }

	private string ExportMermaid(Module[] modules)
	{
		StringBuilder sb = new();
		sb.AppendLine("flowchart TD");

		foreach (var module in modules)
		{
			foreach (var moduleOutput in module.Outputs)
			{
				sb.AppendLine($"{module.Name}[{module.Type.ToString().Trim()}{module.Name}] --> {moduleOutput}");
			}
		}

		return sb.ToString();
	}

	private record Pulse(string Source, string Destination, bool Value);

	private long PulseSum(Dictionary<string, Module> modules, int buttonPresses)
	{
		long lowPulses = 0;
		long highPulses = 0;

		Module button = new Module("button", ' ', ["broadcaster"]);

		Queue<Pulse> pulses = [];


		for (int i = 0; i < buttonPresses; i++)
		{
			SendPulse(new Pulse(button.Name, button.Outputs[0], false));

			while (pulses.Count > 0)
			{
				Pulse p = pulses.Dequeue();

				if (modules.TryGetValue(p.Destination, out Module? current))
				{
					var outputs = ProcessModule(current, p.Value, p.Source);
					foreach (Pulse output in outputs)
					{
						if (output.Destination == "rx")
						{
							if (!output.Value)
							{
								Console.WriteLine($"Part 2: {i}");
								return 0;
							}
						}
						SendPulse(output);
					}
				}
			}
		}

		void SendPulse(Pulse p)
		{
			//Console.WriteLine($"{p.Source} -{p.Value}-> {p.Destination}");
			AddPulse(p.Value);
			pulses.Enqueue(p);
		}

		void AddPulse(bool pulse)
		{
			if (pulse)
			{
				highPulses++;
			}
			else
			{
				lowPulses++;
			}
		}

		return lowPulses * highPulses;
	}

	private IEnumerable<Pulse> ProcessModule(Module current, bool inputPulse, string source)
	{
		bool outputPulse = inputPulse;
		if (current.Type == ' ')
		{
			// forward
			outputPulse = inputPulse;
		}
		else if (current.Type == '%')
		{
			// flip-flop
			if (inputPulse)
			{
				// nothing happens
				yield break;
			}
			else
			{
				current.State.Value ^= true; // flip state
				outputPulse = current.State.Value;
			}
		}
		else if (current.Type == '&')
		{
			// conjunction, all inputs are already initialized
			current.State.Inputs[source] = inputPulse;
			outputPulse = !current.State.Inputs.Values.All(x => x);
		}

		foreach (string output in current.Outputs)
		{
			yield return new Pulse(current.Name, output, outputPulse);
		}
	}

	private Module ParseLine(string line)
    {
	    LineReader reader = new(line);
	    char type = ' ';
	    if (!reader.IsLetter)
	    {
		    type = reader.ReadChar();
	    }
	    string name = reader.ReadLetters().ToString();
        reader.ReadChars(" -> ");

        List<string> outputs = [];
        while (!reader.IsDone)
        {
	        string output = reader.ReadLetters().ToString();
			outputs.Add(output);

	        if (reader.IsDone)
	        {
                break;
	        }
            reader.ReadChars(", ");
        }

        return new Module(name, type, outputs.ToArray());
    }
}    