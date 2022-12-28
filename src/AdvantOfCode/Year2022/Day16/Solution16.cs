using System.Diagnostics;
using System.Numerics;

namespace AdventOfCode.Year2022.Day16;

[DayInfo(2022, 16)]
public class Solution16 : Solution
{
    private List<ValveInt> _valves;
    public string Run()
    {
        string example = """
            Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
            Valve BB has flow rate=13; tunnels lead to valves CC, AA
            Valve CC has flow rate=2; tunnels lead to valves DD, BB
            Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
            Valve EE has flow rate=3; tunnels lead to valves FF, DD
            Valve FF has flow rate=0; tunnels lead to valves EE, GG
            Valve GG has flow rate=0; tunnels lead to valves FF, HH
            Valve HH has flow rate=22; tunnel leads to valve GG
            Valve II has flow rate=0; tunnels lead to valves AA, JJ
            Valve JJ has flow rate=21; tunnel leads to valve II
            """;
        //string[] lines = example.Split("\r\n");
        var lines = InputReader.ReadFileLinesArray();
        _valves = Parse(lines);

        var max = SolveA();
        var maxA = SolveA2(30);
        Debug.Assert(max == maxA);
        
        var maxB = SolveB2(26);
        
        return max + "\n" + maxB;
    }


    private long SetBit(long value, int position) => value | (1L << position);
    private bool IsBitSet(long value, int position) => ((value >> position) & 1) != 0;

    private List<ValveInt> Parse(IEnumerable<string> lines)
    {
        List<Valve> valves = new();
        int id = 0;
        foreach (var line in lines)
        {
            Valve valve = Parse(line, ref id);
            valves.Add(valve);
        }
        
        var dict = valves.ToDictionary(x => x.Name, x => x);
        
        List<ValveInt> valves2 = new();

        foreach (var valve in valves)
        {
            ValveInt v2 = new ValveInt(valve.Name, valve.Id, valve.Flow, valve.Tunnels.Select(x => dict[x].Id).ToArray());
            valves2.Add(v2);
        }
        
        return valves2;
    }

    private record Valve(string Name, int Id, int Flow, string[] Tunnels);
    private record ValveInt(string Name, int Id, int Flow, int[] Tunnels);
    private Valve Parse(string line, ref int id)
    {
        const string RateText = "rate=";
        string[] split = line.Split(' ');

        string name = split[1];
        string flow = split[4];
        ReadOnlySpan<char> flowNumber = flow.AsSpan().Slice(RateText.Length, flow.Length - RateText.Length - 1);
        int flowRate = int.Parse(flowNumber);
        string[] tunnels = split.AsSpan().Slice(9).ToArray().Select(x => x.TrimEnd(',')).ToArray();

        int valveId = id++;
        return new Valve(name, valveId,flowRate, tunnels);
    }

    private record Valve3(ValveNode Node, ValveEdge[] Edges);
    private record ValveEdge(int From, int To, int Minutes);
    private record ValveNode(string Name, int Id, int Flow);

    private record State3(int ValveId, long ValvesOpened, int Pressure, int Minutes);
    private record StateA2(int ValveId, long ValvesOpened, int Pressure, int Minutes);
    private record StateB2(
        int ValveIdLow, 
        int ValveIdHigh, 
        long ValvesOpened, 
        int Pressure, 
        int MinutesLow,
        int MinutesHigh);
    
    private int SolveB2(int count)
    {
        List<Valve3> graph = SimplifyGraph();
        int[,] distances = GraphDistance(graph);
        int[] flows = graph.Select(x => x.Node.Flow).ToArray();

        var aaId = graph.Single(x => x.Node.Name == "AA").Node.Id;
        long aaVisited = SetBit(0, aaId);
        StateB2 state = new (aaId, aaId, aaVisited, 0, 1, 1);

        HashSet<StateB2> statesSet = new();
        HashSet<StateB2> nextStatesSet = new();
        statesSet.Add(state);
        int totalMax = 0;
        foreach (int i in Enumerable.Range(1, count))
        {
            foreach (StateB2 currentState in statesSet.Where(x => x.Pressure > totalMax - 500))
            {
                TraverseB2(currentState, nextStatesSet, flows, distances, i, count);
            }

            (statesSet, nextStatesSet) = (nextStatesSet, statesSet);
            nextStatesSet.Clear();
            var currentStates = statesSet.Where(x => x.MinutesLow == i + 1 || x.MinutesHigh == i + 1).ToList();
            var max = currentStates.Count > 0 ? currentStates.Max(x => x.Pressure) : 0;
            totalMax = Math.Max(totalMax, max);
            Console.WriteLine($"Round{i} - {statesSet.Count}, round max {max} - total {totalMax}");
        }

        return totalMax;
    }
    
    private void TraverseB2(StateB2 state, HashSet<StateB2> queue, int[] flows, int[,] distances, int time, int endTime)
    {
        if (state.MinutesLow > time && state.MinutesHigh > time)
        {
            if (state.MinutesLow <= endTime || state.MinutesHigh <= endTime)
            {
                queue.Add(state);
            }
            return;
        }

        if (state.MinutesLow != time)
        {
            // only high moves
            if (state.MinutesHigh == time)
            {
                for (int j = 0; j < distances.GetLength(0); j++)
                {
                    if (!NextStateValid(j, state.ValveIdHigh, 
                            state.ValvesOpened,
                            out int minutesAfter, 
                            out long valvesOpenTotal, 
                            state.Pressure, 
                            out int pressureAfter)) continue;
                    StateB2 move = new StateB2(state.ValveIdLow, j, valvesOpenTotal, pressureAfter, state.MinutesLow, minutesAfter);
                    queue.Add(move);
                }
            }
        }
        else
        {
            for (int i = 0; i < distances.GetLength(0); i++)
            {
                if (IsBitSet(state.ValvesOpened, i)) continue;

                if (!NextStateValid(i, state.ValveIdLow, 
                        state.ValvesOpened,
                        out int minutesLowAfter, 
                        out long valvesOpenAfter, 
                        state.Pressure, 
                        out int pressureAfter)) continue;

                if (state.MinutesHigh != time)
                {
                    // low moves
                    StateB2 move = new StateB2(i, state.ValveIdHigh, valvesOpenAfter, pressureAfter, minutesLowAfter, state.MinutesHigh);
                    queue.Add(move);
                }
                else
                {
                    // both move
                    for (int j = 0; j < distances.GetLength(0); j++)
                    {
                        if (i == j) continue;

                        if (!NextStateValid(j, state.ValveIdHigh,
                                valvesOpenAfter,
                                out int minutesHighAfter,
                                out long valvesOpenedCombined,
                                pressureAfter,
                                out int pressureAfterCombined)) continue;
                        StateB2 move = new StateB2(i, j, valvesOpenedCombined, pressureAfterCombined, minutesLowAfter, minutesHighAfter);
                        queue.Add(move);
                    }
                }
            }
        }

        bool NextStateValid(int targetId, int sourceId, long valvesOpen, out int minutesAfter, out long valveOpenedAfter, int pressure, out int pressureAfter)
        {
            valveOpenedAfter = valvesOpen;
            pressureAfter = pressure;
            minutesAfter = time;
            
            if (IsBitSet(valvesOpen, targetId)) return false;

            int distance = distances[sourceId, targetId];
            int openedValveTime = time + distance;
            if (openedValveTime >= endTime) return false;

            valveOpenedAfter = SetBit(valvesOpen, targetId);

            int targetFlow = flows[targetId];
            pressureAfter = (endTime - openedValveTime) * targetFlow + pressure;
            minutesAfter = openedValveTime + 1;
            return true;
        }
    }

    private int SolveA2(int count)
    {
        List<Valve3> graph = SimplifyGraph();
        int[,] distances = GraphDistance(graph);
        int[] flows = graph.Select(x => x.Node.Flow).ToArray();

        var aaId = graph.Single(x => x.Node.Name == "AA").Node.Id;
        StateA2 state = new (aaId, SetBit(0, aaId), 0, 1);

        HashSet<StateA2> statesSet = new();
        HashSet<StateA2> nextStatesSet = new();
        statesSet.Add(state);
        int totalMax = 0;
        foreach (int i in Enumerable.Range(1, count))
        {
            foreach (StateA2 currentState in statesSet.Where(x => x.Pressure > totalMax - 250))
            {
                TraverseA2(currentState, nextStatesSet, flows, distances, i, count);
            }

            (statesSet, nextStatesSet) = (nextStatesSet, statesSet);
            nextStatesSet.Clear();
            var currentStates = statesSet.Where(x => x.Minutes == i + 1).ToList();
            var max = currentStates.Count > 0 ? currentStates.Max(x => x.Pressure) : 0;
            totalMax = Math.Max(totalMax, max);
            Console.WriteLine($"Round{i} - {statesSet.Count}, round max {max} - total {totalMax}");
        }

        return totalMax;
    }
    
    private void TraverseA2(StateA2 state, HashSet<StateA2> queue, int[] flows, int[,] distances, int time, int endTime)
    {
        if (state.Minutes > time)
        {
            if (state.Minutes <= endTime)
            {
                queue.Add(state);
            }
            return;
        }

        for (int i = 0; i < distances.GetLength(0); i++)
        {
            if (!IsBitSet(state.ValvesOpened, i))
            {
                int distance = distances[state.ValveId, i];
                int openedValveTime = time + distance;
                if (openedValveTime < endTime)
                {
                    long valvesOpen = SetBit(state.ValvesOpened, i);

                    int targetFlow = flows[i];
                    int pressure = (endTime - openedValveTime) * targetFlow + state.Pressure;

                    StateA2 move = new StateA2(i, valvesOpen, pressure, openedValveTime + 1);
                    queue.Add(move);
                }
            }
        }
    }
    
    private int SolveA()
    {
        List<Valve3> graph = SimplifyGraph();

        Dictionary<int, Valve3> valveDict = graph.ToDictionary(x => x.Node.Id, x => x);

        var nodeAA = graph.Single(x => x.Node.Name == "AA").Node.Id;
        State3 state = new (nodeAA, SetBit(0, nodeAA), 0, 1);

        HashSet<State3> statesSet = new(1_000_000);
        HashSet<State3> nextStatesSet = new(1_000_000);
        statesSet.Add(state);
        int totalMax = 0;
        foreach (int i in Enumerable.Range(1, 30))
        {
            foreach (State3 currentState in statesSet.Where(x => i < 8 || x.Pressure > totalMax - 200))
            {
                Valve3 current = valveDict[currentState.ValveId];
                TraverseA(currentState, current, nextStatesSet, i);
            }

            (statesSet, nextStatesSet) = (nextStatesSet, statesSet);
            nextStatesSet.Clear();
            var max = statesSet.Count > 0 ? statesSet.Max(x => x.Pressure) : 0;
            totalMax = Math.Max(totalMax, max);
            Console.WriteLine($"Round{i} - {statesSet.Count}, round max {max} - total {totalMax}");
        }

        return totalMax;
    }

    private void TraverseA(State3 state, Valve3 current, HashSet<State3> queue, int time)
    {
        if (state.Minutes > time)
        {
            if (state.Minutes <= 30)
            {
                queue.Add(state);
            }
            return;
        }
        
        //long visited = SetBit(state.ValvesVisited, current.Id);
        if (current.Node.Flow > 0 && !IsBitSet(state.ValvesOpened, current.Node.Id))
        {
            long valvesOpen = SetBit(state.ValvesOpened, current.Node.Id);
            
            int pressure = (30 - time) * current.Node.Flow + state.Pressure;

            //State opened = new State(state.ValveId, valvesOpen, pressure, visited);
            State3 opened = new (state.ValveId, valvesOpen, pressure, state.Minutes + 1);
            queue.Add(opened);
        }

        foreach (var tunnel in current.Edges)
        {
            if(IsBitSet(state.ValvesOpened, tunnel.To)) continue;
            int arrivalTime = state.Minutes + tunnel.Minutes;
            if(arrivalTime + 1 >= 30) continue;
            State3 move = state with { ValveId = tunnel.To, Minutes = arrivalTime};
            queue.Add(move);
        }
    }
    
    private List<Valve3> SimplifyGraph()
    {
        List<ValveEdge> edges = new();
        List<ValveNode> nodes = new();

        foreach (ValveInt valve in _valves)
        {
            nodes.Add(new ValveNode(valve.Name, valve.Id, valve.Flow));
            foreach (var valveTunnel in valve.Tunnels)
            {
                edges.Add(new ValveEdge(valve.Id, valveTunnel, 1));
            }
        }

        bool simplified = true;
        while (simplified)
        {
            int edgeCount = edges.Count;
            int nodeCount = nodes.Count;
            simplified = false;
            
            List<ValveNode> toRemove = new();

            foreach (ValveNode valveNode in nodes)
            {
                if (valveNode.Flow == 0)
                {
                    var valveEdges = edges.Where(x => x.From == valveNode.Id).ToList();
                    if (valveEdges.Count == 2)
                    {
                        var edge0 = valveEdges[0];
                        var edge0Complement = edges.Single(x => x.From == edge0.To && x.To == edge0.From);

                        var edge1 = valveEdges[1];
                        var edge1Complement = edges.Single(x => x.From == edge1.To && x.To == edge1.From);

                        edges.Remove(edge0);
                        edges.Remove(edge0Complement);
                        edges.Remove(edge1);
                        edges.Remove(edge1Complement);
                        
                        edges.Add(new ValveEdge(edge0Complement.From, edge1.To, edge0Complement.Minutes + edge1.Minutes));
                        edges.Add(new ValveEdge(edge1.To, edge0Complement.From, edge0Complement.Minutes + edge1.Minutes));

                        toRemove.Add(valveNode);
                        simplified = true;
                    }
                }
            }

            foreach (ValveNode remove in toRemove)
            {
                nodes.Remove(remove);
            }

            Console.WriteLine($"Simplified {edgeCount}->{edges.Count}, {nodeCount}->{nodes.Count}");
        }

        Dictionary<string, int> renumberValves = new();
        List<ValveInt> flowValves = _valves.Where(x => x.Flow > 0 || x.Name == "AA").ToList();
        Dictionary<int, int> renumberTunnels = new();

        foreach (ValveInt flowValve in flowValves)
        {
            renumberTunnels.Add(flowValve.Id, renumberValves.Count);
            renumberValves.Add(flowValve.Name, renumberValves.Count);
        }

        List<Valve3> result = new();
        foreach (var node in nodes)
        {
            Valve3 v = new (node, edges.Where(x => x.From == node.Id).ToArray());
            result.Add(v);
        }

        List<Valve3> reduced = new();
        foreach (var node in nodes)
        {
            int id = renumberValves[node.Name];
            var newEdges = edges.Where(x => x.From == node.Id).Select(e =>
                new ValveEdge(renumberTunnels[e.From], renumberTunnels[e.To], e.Minutes)).ToArray();
            var newNode = node with { Id = id };
            reduced.Add(new Valve3(newNode, newEdges));
        }
        
        return reduced;
    }
    
    private static int[,] GraphDistance(List<Valve3> graph)
    {
        int valveCount = graph.Count;
        int[,] distances = new int[valveCount, valveCount];
        for (int i = 0; i < valveCount; i++)
        {
            for (int j = 0; j < valveCount; j++)
            {
                distances[i, j] = int.MaxValue;
            }
        }

        foreach (var valve3 in graph)
        {
            foreach (ValveEdge valveEdge in valve3.Edges)
            {
                distances[valveEdge.From, valveEdge.To] = valveEdge.Minutes;
            }

            distances[valve3.Node.Id, valve3.Node.Id] = 0;
        }

        for (int k = 0; k < valveCount; k++)
        {
            for (int i = 0; i < valveCount; i++)
            {
                for (int j = 0; j < valveCount; j++)
                {
                    if (distances[i, k] == int.MaxValue || distances[k, j] == int.MaxValue) continue;

                    int viaK = distances[i, k] + distances[k, j];
                    if (distances[i, j] > viaK)
                    {
                        distances[i, j] = viaK;
                    }
                }
            }
        }

        return distances;
    }
}
