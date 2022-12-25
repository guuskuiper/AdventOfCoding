using System.Numerics;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2022.Day11;

public class Solution11 : Solution
{
    private const bool Print = false;
    private record Monkey(int Id, List<long> Items, Polynomial Operation, int DivisibleBy, int TargetTrue, int TargetFalse);
    private record Polynomial(int A2, int A1, int A0)
    {
        public long Apply(long x) => A2 * x * x + A1 * x + A0;
    }

    private long[] _inspects = Array.Empty<long>();

    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        
        List<Monkey> monkeys = Parse(lines);
        _inspects = new long[monkeys.Count];
        Rounds(monkeys, 20);

        long sum = _inspects.OrderByDescending(x => x)
            .Take(2)
            .Aggregate((x, y) => x * y);
        
        monkeys = Parse(lines);
        _inspects = new long[monkeys.Count];
        Rounds2(monkeys, 10_000);
        
        long sum2 = _inspects.OrderByDescending(x => x)
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
        long divisors = MathGeneric.LCM(monkeys.Select(x => x.DivisibleBy));
        
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
        for (int i = 0; i < _inspects.Length; i++)
        {
            Console.WriteLine($"Monkey {i} inspected items {_inspects[i]} times. ");
        }

        Console.WriteLine();
    }

    private void Round(List<Monkey> monkeys, double divisor, long modulo)
    {
        foreach (Monkey monkey in monkeys)
        {
            foreach (var item in monkey.Items)
            {
                _inspects[monkey.Id] += 1;
                long result;
                checked
                { 
                    result = monkey.Operation.Apply(item);
                }

                long bored = (long)Math.Floor(result / divisor);
                long reduced = bored % modulo;

                int target = reduced % monkey.DivisibleBy == 0 ? monkey.TargetTrue : monkey.TargetFalse;

                monkeys[target].Items.Add(reduced);
            }
            monkey.Items.Clear();
        }
    }
    
    private List<Monkey> Parse(string[] lines)
    {
        List<Monkey> monkeys = new();
        foreach (var chunk in lines.Chunk(6))
        {
            int id = ParseId(chunk[0]);
            List<long> items = ParseItems(chunk[1]);
            Polynomial operation = ParsePolynomial(chunk[2]);
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

    private Polynomial ParsePolynomial(string line)
    {
        string[] split = line.Split("old ");
        split = split[1].Split(' ');

        char op = split[0][0];
        string arg = split[1];


        int a2;
        int a1;
        int a0;
        if (op == '*')
        {
            if (arg == "old")
            {
                a2 = 1;
                a1 = 0;
            }
            else
            {
                a2 = 0;
                a1 = int.Parse(arg);
            }
            a0 = 0;
        }
        else
        {
            a2 = 0;
            a1 = 1;
            a0 = int.Parse(arg);
        }

        return new Polynomial(a2, a1, a0);
    }
}
