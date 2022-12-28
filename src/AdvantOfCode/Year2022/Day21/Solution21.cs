namespace AdventOfCode.Year2022.Day21;

[DayInfo(2022, 21)]
public class Solution21 : Solution
{
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        var monkeys = Parse(lines).ToArray();
        var monkeyDict = monkeys.ToDictionary(x => x.Name, x => x);

        MonkeyOperation root = (monkeyDict["root"] as MonkeyOperation)!;
        long value = root.Result(monkeyDict);

        Root rootMonkey = new Root(root.Left, root.Right);
        Human humn = new Human();
        monkeyDict[rootMonkey.Name] = rootMonkey;
        monkeyDict[humn.Name] = humn;

        rootMonkey.Simplify(monkeyDict, out _, out string toSolve);
        long humnValue = rootMonkey.MakeEqual(monkeyDict);
        long equal = rootMonkey.Result(monkeyDict);
        Console.WriteLine(toSolve);
        
        return value + "\n" + humnValue;
    }

    private abstract record Monkey(string Name)
    {
        public abstract long Result(Dictionary<string, Monkey> monkeys);
        public abstract bool Simplify(Dictionary<string, Monkey> monkeys, out long value, out string valueStr);
        public abstract long MakeEqual(Dictionary<string, Monkey> monkeys, long value);
    }
    private record MonkeyNumber(string Name, int Number) : Monkey(Name)
    {
        public override long Result(Dictionary<string, Monkey> monkeys)
        {
            return Number;
        }

        public override bool Simplify(Dictionary<string, Monkey> monkeys, out long value, out string valueStr)
        {
            value = Number;
            valueStr = Number.ToString();
            return true;
        }

        public override long MakeEqual(Dictionary<string, Monkey> monkeys, long value)
        {
            throw new Exception($"Can't make a constant of {Number} equal to {value}");
        }
    }

    private record Human() : Monkey("humn")
    {
        private long? _solvedValue;
        public override long Result(Dictionary<string, Monkey> monkeys)
        {
            return _solvedValue ?? throw new Exception("Value not known yet");
        }
        
        public override bool Simplify(Dictionary<string, Monkey> monkeys, out long value, out string valueStr)
        {
            value = -1;
            valueStr = Name;
            return false;
        }
        
        public override long MakeEqual(Dictionary<string, Monkey> monkeys, long value)
        {
            _solvedValue = value;
            return value;
        }
    }
    
    private record Root(string Left, string Right) : Monkey("root")
    {
        public override long Result(Dictionary<string, Monkey> monkeys)
        {
            Monkey left = monkeys[Left];
            Monkey right = monkeys[Right];

            long resultLeft = left.Result(monkeys);
            long resultRight = right.Result(monkeys);
            return resultLeft == resultRight ? 1 : 0;
        }
        
        public override bool Simplify(Dictionary<string, Monkey> monkeys, out long value, out string valueStr)
        {
            Monkey left = monkeys[Left];
            Monkey right = monkeys[Right];
            
            bool canSimplifyLeft = left.Simplify(monkeys, out long valueLeft, out string leftStr);
            bool canSimplifyRight = right.Simplify(monkeys, out long valueRight, out string rightStr);

            bool result;
            if (canSimplifyLeft && canSimplifyRight)
            {
                value = Result(monkeys);
                valueStr = value.ToString();
                result = true;
            }
            else
            {
                value = -1;
                valueStr = $"({leftStr} == {rightStr})";
                result = false;
            }
            
            return result;
        }

        public override long MakeEqual(Dictionary<string, Monkey> monkeys, long value) => MakeEqual(monkeys);
        
        public long MakeEqual(Dictionary<string, Monkey> monkeys)
        {
            Monkey left = monkeys[Left];
            Monkey right = monkeys[Right];
            
            bool canSimplifyLeft = left.Simplify(monkeys, out long valueLeft, out string _);
            bool canSimplifyRight = right.Simplify(monkeys, out long valueRight, out string _);

            long humnValue;
            if(!canSimplifyRight)
            {
                humnValue = right.MakeEqual(monkeys, valueLeft);
            }
            else if(!canSimplifyLeft)
            {
                humnValue = left.MakeEqual(monkeys, valueRight);
            }
            else
            {
                throw new Exception($"No humn found");
            }

            return humnValue;
        }
    }

    private record MonkeyOperation(string Name, string Left, char Operation, string Right) : Monkey(Name)
    {
        public override long Result(Dictionary<string, Monkey> monkeys)
        {
            Monkey left = monkeys[Left];
            Monkey right = monkeys[Right];

            long resultLeft = left.Result(monkeys);
            long resultRight = right.Result(monkeys);

            return Operation switch
            {
                '+' => resultLeft + resultRight,
                '-' => resultLeft - resultRight,
                '*' => resultLeft * resultRight,
                '/' => resultLeft / resultRight,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public override bool Simplify(Dictionary<string, Monkey> monkeys, out long value, out string valueStr)
        {
            Monkey left = monkeys[Left];
            Monkey right = monkeys[Right];
            
            bool canSimplifyLeft = left.Simplify(monkeys, out long _, out string leftStr);
            bool canSimplifyRight = right.Simplify(monkeys, out long _, out string rightStr);

            if (canSimplifyLeft && canSimplifyRight)
            {
                value = Result(monkeys);
                valueStr = value.ToString();
                return true;
            }
            
            value = -1;
            valueStr = $"({leftStr} {Operation} {rightStr})";
            return false;
        }
        
        public override long MakeEqual(Dictionary<string, Monkey> monkeys, long value)
        {
            Monkey left = monkeys[Left];
            Monkey right = monkeys[Right];
            
            bool canSimplifyLeft = left.Simplify(monkeys, out long valueLeft, out string _);
            bool canSimplifyRight = right.Simplify(monkeys, out long valueRight, out string _);

            long result;
            if (!canSimplifyLeft)
            {
                long leftTarget = Operation switch
                {
                    '+' => value - valueRight,
                    '-' => value + valueRight,
                    '*' => value / valueRight,
                    '/' => value * valueRight,
                    _ => throw new ArgumentOutOfRangeException()
                };
                result = left.MakeEqual(monkeys, leftTarget);
            }
            else if (!canSimplifyRight)
            {
                long rightTarget = Operation switch
                {
                    '+' => value - valueLeft,
                    '-' => valueLeft - value,
                    '*' => value / valueLeft,
                    '/' => value / valueLeft,
                    _ => throw new ArgumentOutOfRangeException()
                };
                result = right.MakeEqual(monkeys, rightTarget);
            }
            else
            {
                throw new Exception($"No humn found");
            }
            
            return result;
        }
    }

    private IEnumerable<Monkey> Parse(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            yield return Parse(line);
        }
    }

    private Monkey Parse(string lines)
    {
        Monkey monkey;
        string[] split = lines.Split(' ');
        string name = split[0].TrimEnd(':');
        if (split.Length == 2)
        {
            monkey = new MonkeyNumber(name, int.Parse(split[1]));
        }
        else
        {
            monkey = new MonkeyOperation(name, split[1], split[2][0], split[3]);
        }

        return monkey;
    }
}
