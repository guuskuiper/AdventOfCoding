namespace AdventOfCode.Year2022.Day11;

public class Solution11 : Solution
{
    private record Monkey(int Id, List<long> WorryLevelItems, Func<long, long> Operation, int DivisibleBy, int TargetTrue, int TargetFalse);

    private long[] inspects;
    private Func<long, long> func;

    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        List<Monkey> monkeys = Parse(lines);
        inspects = new long[monkeys.Count];
        Rounds(monkeys, 20);

        long sum = inspects.OrderBy(x => x).Reverse().Take(2).Aggregate((x, y) => x * y);
        
        monkeys = Parse(lines);
        inspects = new long[monkeys.Count];
        Rounds2(monkeys, 10_000);
        
        long sum2 = inspects.OrderBy(x => x).Reverse().Take(2).Aggregate((x, y) => x * y);
        
        return sum + "\n" + sum2;
    }
    
    private void Rounds2(List<Monkey> monkeys, int count)
    {
        long divisors = monkeys.Aggregate(1, (x, y) => x * y.DivisibleBy);
        
        foreach (var round in Enumerable.Range(1, count))
        {
            Round2(monkeys, divisors);

            if (round % 1000 == 0 || round == 20)
            {
                Console.WriteLine($"== After round {round} ==");
                for (int i = 0; i < inspects.Length; i++)
                {
                    Console.WriteLine($"Monkey {i} inspected items {inspects[i]} times. ");
                }
                Console.WriteLine();
            }
        }
    }

    private void Round2(List<Monkey> monkeys, long divisors)
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

                long reduced = result % divisors;

                int target = reduced % monkey.DivisibleBy == 0 ? monkey.TargetTrue : monkey.TargetFalse;

                monkeys[target].WorryLevelItems.Add(reduced);
            }
            monkey.WorryLevelItems.Clear();
        }
    }

    private void Rounds(List<Monkey> monkeys, int count)
    {
        foreach (var round in Enumerable.Range(1, count))
        {
            Round(monkeys);
        }
    }

    private void Round(List<Monkey> monkeys)
    {
        foreach (Monkey monkey in monkeys)
        {
            foreach (var item in monkey.WorryLevelItems)
            {
                inspects[monkey.Id] += 1;
                long result = monkey.Operation(item);
                long bored = (long)Math.Floor(result / 3.0);

                int target = bored % monkey.DivisibleBy == 0 ? monkey.TargetTrue : monkey.TargetFalse;
                monkeys[target].WorryLevelItems.Add(bored);
            }
            monkey.WorryLevelItems.Clear();
        }
    }

    private List<Monkey> Parse(string[] lines)
    {
        List<Monkey> monkeys = new();
        foreach (var chunk in lines.Chunk(6))
        {
            string[] split = chunk[0].Split(' ');
            split = split[1].Split(':');
            int id = int.Parse(split[0]);

            split = chunk[1].Split(':');
            List<long> items = split[1].Split(',').Select(long.Parse).ToList();
            
            split = chunk[2].Split("old ");
            Func<long, long> operation = ParseOperation(split[1]);

            split = chunk[3].Split("by ");
            int divisibleBy = int.Parse(split[1]);
            
            split = chunk[4].Split("monkey ");
            int targetTrue = int.Parse(split[1]);
            
            split = chunk[5].Split("monkey ");
            int targetFalse = int.Parse(split[1]);

            Monkey monkey = new Monkey(id, items, operation, divisibleBy, targetTrue, targetFalse);
            monkeys.Add(monkey);
        }

        return monkeys;
    }

    private Func<long, long> ParseOperation(string operation)
    {
        string[] split = operation.Split(' ');

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
