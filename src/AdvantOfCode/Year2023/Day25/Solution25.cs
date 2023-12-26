namespace AdventOfCode.Year2023.Day25;

[DayInfo(2023, 25)]
public class Solution25 : Solution
{
    public string Run()
    {
        string example = """
                         jqt: rhn xhk nvd
                         rsh: frs pzl lsr
                         xhk: hfx
                         cmg: qnr nvd lhk bvb
                         rhn: xhk bvb hfx
                         bvb: xhk hfx
                         pzl: lsr hfx nvd
                         qnr: nvd
                         ntq: jqt hfx bvb xhk
                         nvd: lhk
                         lsr: lhk
                         rzs: qnr cmg lsr
                         """;
        string[] input = this.ReadLines();
        string[] input2 = example.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        Wiring[] wirings = input.Select(ParseLine).ToArray();
        Dictionary<string, Wiring> wiringMap = wirings.ToDictionary(x => x.Name, x => x);
        
        AddConnections(wiringMap);

        List<Connection> removed = [];
        for (int i = 0; i < 3; i++)
        {
            Connection mostUsed = Busiest(wiringMap);
            //Console.WriteLine($"Removing {mostUsed}");

            wiringMap[mostUsed.Low].Connection.Remove(mostUsed.High);
            wiringMap[mostUsed.High].Connection.Remove(mostUsed.Low);
            removed.Add(mostUsed);
        }

        int cluster1 = CountClusterSize(wiringMap, removed[0].Low);
        int cluster2 = CountClusterSize(wiringMap, removed[0].High);

        int mult = cluster1 * cluster2;
        return mult + "\n";
    }

    private int CountClusterSize(Dictionary<string, Wiring> wirings, string start)
    {
        AQueue<string> frontier = new();
        frontier.Add(start);
        HashSet<string> visited = [start];
        
        while (!frontier.Empty)
        {
            string current = frontier.Get();

            foreach (string next in wirings[current].Connection)
            {
                if(!visited.Contains(next))
                {
                    frontier.Add(next);
                    visited.Add(next);
                }
            }
        }

        return visited.Count;
    }

    private Connection Busiest(Dictionary<string, Wiring> wirings)
    {
        Wiring[] nodes = wirings.Values.ToArray();
        nodes = PickUnique(nodes, 10);

        Dictionary<Connection, long> heatMap = [];
        ConnectionGraph graph = new(wirings);
        for (int x = 0; x < nodes.Length; x++)
        {
            string start = nodes[x].Name;
            for (int y = x + 1; y < nodes.Length; y++)
            {
                string end = nodes[y].Name;
                var path = BFS.SearchFrom(graph, start, end);

                string current = end;
                while (current != start)
                {
                    string source = path[current];
                    Connection c = new(source, current);
                    current = source;
                    heatMap.AddOrCreate(c, 1);
                }
            }
        }
        
        KeyValuePair<Connection, long>[] sortedDict = heatMap.OrderByDescending(x => x.Value).ToArray();
        return sortedDict[0].Key;
    }

    private static Wiring[] PickUnique(Wiring[] items, int maxNumber)
    {
        if (items.Length < maxNumber) return items.ToArray();
        
        HashSet<int> picked = [];
        Random random = new Random(42);
        while (picked.Count < maxNumber)
        {
            picked.Add(random.Next(items.Length));
        }

        return picked.Select(x => items[x]).ToArray();
    }

    private record Connection
    {
        public Connection(string source, string destination)
        {
            if (Comparer<string>.Default.Compare(source, destination) > 0)
            {
                this.Low = destination;
                this.High = source;
            }
            else
            {
                this.Low = source;
                this.High = destination;
            }
        }

        public string Low { get; }
        public string High { get; }

        public void Deconstruct(out string Low, out string High)
        {
            Low = this.Low;
            High = this.High;
        }
    }

    private class ConnectionGraph : IGraph<string>
    {
        private readonly Dictionary<string, Wiring> _wirings;

        public ConnectionGraph(Dictionary<string, Wiring> wirings)
        {
            _wirings = wirings;
        }

        public IEnumerable<string> Neighbors(string node)
        {
            return _wirings[node].Connection;
        }
    }

    private void AddConnections(Dictionary<string, Wiring> wirings)
    {
        Dictionary<string, Wiring> newWirings = [];
        foreach ((var key, Wiring value) in wirings)
        {
            foreach (var connection in value.Connection)
            {
                if (wirings.TryGetValue(connection, out Wiring? target))
                {
                    target.Connection.Add(key);
                }
                else if(newWirings.TryGetValue(connection, out target))
                {
                    target.Connection.Add(key);
                }
                else
                {
                    newWirings.Add(connection, new Wiring(connection, [key]));
                }
            }
        }

        foreach (KeyValuePair<string,Wiring> newWire in newWirings)
        {
            wirings.Add(newWire.Key, newWire.Value);
        }
    }

    private record Wiring(string Name, HashSet<string> Connection);

    private Wiring ParseLine(string line)
    {
        LineReader reader = new(line);
        string name = reader.ReadLetters().ToString();
        reader.ReadChars(": ");
        List<string> connection = [];
        while (!reader.IsDone)
        {
            connection.Add(reader.ReadLetters().ToString());
            reader.SkipWhitespaces();
        }

        return new Wiring(name, connection.ToHashSet());
    }
}    