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
		
		// find output "rx"
		Module toRX = FindProducer(modules, "rx");
		Module[] sources = FindProducers(modules, toRX.Name).ToArray();
		

		string mermaid = ExportMermaid(modules);
        long sum = PulseSum(moduleMap, 1000, sources.Select(x => x.Name).ToHashSet(), out long part2);

        return sum + "\n" + part2;
    }


	private record Pulse(string Source, string Destination, bool Value);

	private long PulseSum(Dictionary<string, Module> modules, int buttonPresses, HashSet<string> part2Destinations, out long largeNumber)
	{
		long lowPulses = 0;
		long highPulses = 0;

		Dictionary<string, int> firstPulse = new();
		Dictionary<string, int> pulsePeriod = new();

		bool part1Done = false;

		Module button = new Module("button", ' ', ["broadcaster"]);

		Queue<Pulse> pulses = [];

		for (int i = 0; i < 25_000; i++)
		{
			if (i >= buttonPresses) part1Done = true;
			
			SendPulse(new Pulse(button.Name, button.Outputs[0], false));

			while (pulses.Count > 0)
			{
				Pulse p = pulses.Dequeue();

				if (modules.TryGetValue(p.Destination, out Module? current))
				{
					var outputs = ProcessModule(current, p.Value, p.Source);
					foreach (Pulse output in outputs)
					{
						if(!output.Value && part2Destinations.Contains(output.Destination))
						{
							string outputName = output.Destination;
							if(!firstPulse.TryGetValue(outputName, out int first))
							{
								firstPulse[outputName] = i;
							}
							else
							{
								if (!pulsePeriod.ContainsKey(outputName))
								{
									pulsePeriod[outputName] = i - first;
									if (pulsePeriod.Count >= part2Destinations.Count)
									{
										largeNumber = pulsePeriod.Values.Aggregate(1L, (acc, val) => acc * val); 
										return lowPulses * highPulses;
									}
								}
							}
							//Console.WriteLine($"Part 2 {output.Destination}: {i}");
						}
						SendPulse(output);
					}
				}
			}
		}

		largeNumber = -1;
		return lowPulses * highPulses;

		void SendPulse(Pulse p)
		{
			//Console.WriteLine($"{p.Source} -{p.Value}-> {p.Destination}");
			AddPulse(p.Value);
			pulses.Enqueue(p);
		}

		void AddPulse(bool pulse)
		{
			if(part1Done) return;
			
			if (pulse)
			{
				highPulses++;
			}
			else
			{
				lowPulses++;
			}
		}
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
	
	private Module FindProducer(Module[] modules, string destination)
	{
		Module? source = null;
		foreach (Module module in modules)
		{
			if (module.Outputs.Contains(destination))
			{
				source = module;
				break;
			}
		}
		ArgumentNullException.ThrowIfNull(source, nameof(destination));
		return source;
	}
	
	private IEnumerable<Module> FindProducers(Module[] modules, string destination)
	{
		foreach (Module module in modules)
		{
			if (module.Outputs.Contains(destination))
			{
				yield return module;
			}
		}
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