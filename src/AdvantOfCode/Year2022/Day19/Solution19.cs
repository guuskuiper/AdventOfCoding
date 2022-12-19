namespace AdventOfCode.Year2022.Day19;

public class Solution19 : Solution
{
    private const int TimeA = 24;
    private const int TimeB = 32;
    private static readonly Resources StartResources = new (0, 1, 0, 0, 0, 0, 0, 0);
    
    private record Blueprint(
        int Id,
        int OroRobotOreCosts,
        int ClayRobotOreCosts,
        int ObsidianRobotOreCosts,
        int ObsidianRobotClayCosts,
        int GeodeRobotOreCosts,
        int GeodeRobotObsidianCosts);

    private record Resources(
        int Ore,
        int OreRobots,
        int Clay,
        int ClayRobots,
        int Obsidian,
        int ObsidianRobots,
        int Geode,
        int GeodeRobots);
    
    public string Run()
    {
        string example = """
        Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
        Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.
        """;
        //var lines = example.Split("\r\n");
        var lines = InputReader.ReadFileLinesArray();
        var blueprints = Parse(lines).ToList();

        //int sumA = 1418;
        int sumA = SimulateAll(blueprints, TimeA).Sum(x => x.Value * x.Key.Id);
        int sumB = SimulateAll(blueprints.Take(3).ToList(), TimeB).Aggregate(1, (i, pair) => i * pair.Value);
        
        return sumA + "\n" + sumB;
    }
    
    private Dictionary<Blueprint, int> SimulateAll(List<Blueprint> blueprints, int minutes)
    {
        Dictionary<Blueprint, int> results = new();
        foreach (Blueprint blueprint in blueprints)
        {
            int max = Simulate(blueprint, minutes);
            results[blueprint] = max;
        }

        return results;
    }

    private int Simulate(Blueprint blueprint, int minutes)
    {
        Resources start = StartResources;

        HashSet<Resources> currentResources = new(10_000_000) { start};
        HashSet<Resources> nextResources = new(10_000_000);

        int currentMax = 0;
        foreach (var minute in Enumerable.Range(1, minutes))
        {
            foreach (Resources currentState in currentResources.Where(x => x.Geode >= currentMax - 2))
            {
                Traverse(blueprint, currentState, nextResources, minute);
            }

            currentMax = nextResources.Max(x => x.Geode);
            //Console.WriteLine($"Minute {minute} - {nextResources.Count}, round max {currentMax}");
            (currentResources, nextResources) = (nextResources, currentResources);
            nextResources.Clear();
        }

        return currentResources.Max(x => x.Geode);
    }
    
    private void Traverse(Blueprint blueprint, Resources current, ICollection<Resources> next, int minutes)
    {
        Resources nextResources = current with
        {
            Ore = current.Ore + current.OreRobots,
            Clay = current.Clay + current.ClayRobots,
            Obsidian = current.Obsidian + current.ObsidianRobots,
            Geode = current.Geode + current.GeodeRobots,
        };

        bool canBuyOreRobot = current.Ore >= blueprint.OroRobotOreCosts;
        if (canBuyOreRobot && 
            current.Ore <= Math.Max(
                Math.Max(blueprint.ClayRobotOreCosts, blueprint.GeodeRobotOreCosts), 
                Math.Max(blueprint.ObsidianRobotOreCosts, blueprint.OroRobotOreCosts)) + 1
            )
        {
            Resources buyOreRobot = nextResources with
            {
                Ore = nextResources.Ore - blueprint.OroRobotOreCosts, 
                OreRobots = current.OreRobots + 1
            };
            next.Add(buyOreRobot);
        }

        bool canBuyClayRobot = current.Ore >= blueprint.ClayRobotOreCosts;
        if (canBuyClayRobot && 
            current.Clay <= blueprint.ObsidianRobotClayCosts + 1)
        {
            Resources buyClayRobot = nextResources with
            {
                Ore = nextResources.Ore - blueprint.ClayRobotOreCosts, 
                ClayRobots = current.ClayRobots + 1
            };
            next.Add(buyClayRobot);
        }

        bool canBuyObsidianRobot = current.Ore >= blueprint.ObsidianRobotOreCosts &&
                                   current.Clay >= blueprint.ObsidianRobotClayCosts;
        if (canBuyObsidianRobot && 
            current.Obsidian <= blueprint.GeodeRobotObsidianCosts + 1)
        {
            Resources buyObsidianRobot = nextResources with
            {
                Ore = nextResources.Ore - blueprint.ObsidianRobotOreCosts, 
                Clay = nextResources.Clay - blueprint.ObsidianRobotClayCosts, 
                ObsidianRobots = current.ObsidianRobots + 1
            };
            next.Add(buyObsidianRobot);
        }

        bool canBuyGeodeRobot = current.Ore >= blueprint.GeodeRobotOreCosts &&
                                current.Obsidian >= blueprint.GeodeRobotObsidianCosts;
        if (canBuyGeodeRobot)
        {
            Resources buyGeodeRobot = nextResources with
            {
                Ore = nextResources.Ore - blueprint.GeodeRobotOreCosts, 
                Obsidian = nextResources.Obsidian - blueprint.GeodeRobotObsidianCosts, 
                GeodeRobots = current.GeodeRobots + 1
            };
            next.Add(buyGeodeRobot);
        }

        if (!canBuyGeodeRobot)
        {
            if (!(canBuyOreRobot && canBuyClayRobot && canBuyObsidianRobot))
            {
                next.Add(nextResources);
            }
        }
    }

    private IEnumerable<Blueprint> Parse(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            yield return Parse(line);
        }
    }

    private Blueprint Parse(string line)
    {
        string[] split = line.Split(' ');

        int id = int.Parse(split[1].TrimEnd(':'));
        return new Blueprint(
            id,
            int.Parse(split[6]),
            int.Parse(split[12]),
            int.Parse(split[18]),
            int.Parse(split[21]),
            int.Parse(split[27]),
            int.Parse(split[30])
        );
    }
}
