using System.Numerics;

namespace AdventOfCode.Year2022.Day11;

public class Solution11 : Solution
{
    private const bool Print = false;
    private record Monkey(int Id, List<long> WorryLevelItems, Func<long, long> Operation, int DivisibleBy, int TargetTrue, int TargetFalse);

    private long[] inspects;
    private Func<long, long> func;

    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        
        List<Monkey> monkeys = Parse(lines);
        inspects = new long[monkeys.Count];
        Rounds(monkeys, 20);

        long sum = inspects.OrderByDescending(x => x)
            .Take(2)
            .Aggregate((x, y) => x * y);
        
        monkeys = Parse(lines);
        inspects = new long[monkeys.Count];
        Rounds2(monkeys, 10_000);
        
        long sum2 = inspects.OrderByDescending(x => x)
            .Take(2)
            .Aggregate((x, y) => x * y);
        
        return sum + "\n" + sum2;
    }
    
    private void Rounds(List<Monkey> monkeys, int count)
    {
        foreach (var round in Enumerable.Range(1, count))
        {
            Round(monkeys, 3, long.MaxValue);
        }
    }



    private void Rounds2(List<Monkey> monkeys, int count)
    {
        long divisors = monkeys.Aggregate(GetMultIdentity<long>(), (x, y) => x * y.DivisibleBy);
        
        foreach (var round in Enumerable.Range(1, count))
        {
            Round(monkeys, 1, divisors);

            if (Print && (round % 1000 == 0 || round == 20))
            {
                PrintRound(round);
            }
        }
    }
    
    private T GetMultIdentity<T>() where T : IMultiplicativeIdentity<T, T>
    {
        return T.MultiplicativeIdentity;
    }

    private void PrintRound(int round)
    {
        Console.WriteLine($"== After round {round} ==");
        for (int i = 0; i < inspects.Length; i++)
        {
            Console.WriteLine($"Monkey {i} inspected items {inspects[i]} times. ");
        }

        Console.WriteLine();
    }

    private void Round(List<Monkey> monkeys, double divisor, long modulo)
    {
        foreach (Monkey monkey in monkeys)
        {
            foreach (var item in monkey.WorryLevelItems)
            {
                inspects[monkey.Id] += 1;
                long result;
                checked
                { 
                    result = monkey.Operation(item);
                }

                long bored = (long)Math.Floor(result / divisor);
                long reduced = bored % modulo;

                int target = reduced % monkey.DivisibleBy == 0 ? monkey.TargetTrue : monkey.TargetFalse;

                monkeys[target].WorryLevelItems.Add(reduced);
            }
            monkey.WorryLevelItems.Clear();
        }
    }
    
    private List<Monkey> Parse(string[] lines)
    {
        List<Monkey> monkeys = new();
        foreach (var chunk in lines.Chunk(6))
        {
            int id = ParseId(chunk[0]);
            List<long> items = ParseItems(chunk[1]);
            Func<long, long> operation = ParseOperation(chunk[2]);
            int divisibleBy = ParseDivisibleBy(chunk[3]);
            int targetTrue = ParseTarget(chunk[4]);
            int targetFalse = ParseTarget(chunk[5]);

            Monkey monkey = new Monkey(id, items, operation, divisibleBy, targetTrue, targetFalse);
            monkeys.Add(monkey);
        }

        return monkeys;
    }

    private static int ParseTarget(string line)
    {
        string[] split = line.Split("monkey ");
        int targetTrue = int.Parse(split[1]);
        return targetTrue;
    }

    private static int ParseDivisibleBy(string line)
    {
        string[] split = line.Split("by ");
        int divisibleBy = int.Parse(split[1]);
        return divisibleBy;
    }

    private static List<long> ParseItems(string line)
    {
        string[] split = line.Split(':');
        List<long> items = split[1].Split(',').Select(long.Parse).ToList();
        return items;
    }

    private static int ParseId(string line)
    {
        string[] split = line.Split(' ');
        split = split[1].Split(':');
        int id = int.Parse(split[0]);
        return id;
    }

    private Func<long, long> ParseOperation(string line)
    {
        string[] split = line.Split("old ");
        split = split[1].Split(' ');

        char op = split[0][0];
        string arg = split[1];
        if (arg == "old")
        {
            if (op == '*')
            {
                func = x => x * x;
            }
            else
            {
                func = x => x + x;
            }
        }
        else
        {
            int value = int.Parse(arg);
            if (op == '*')
            {
                func = x => x * value;
            }
            else
            {
                func = x => x + value;
            }
        }

        return func;
    }
}
