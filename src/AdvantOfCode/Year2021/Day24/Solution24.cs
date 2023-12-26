namespace AdventOfCode.Year2021.Day24;

[DayInfo(2021, 24)]
public class Solution24 : Solution
{
    public enum InstructionType
    {
        Inp,
        Add,
        Mul,
        Div,
        Mod,
        Eql,
    }

    public class Instruction
    {
        public InstructionType Type { get; set; }
        public string Arg0 { get; set; }
        public string Arg1 { get; set; }

        public override string ToString()
        {
            return $"{Type} {Arg0} {Arg1}";
        }
    }

    private const int MODEL_SIZE = 14;
    private List<Instruction> _instructions = new();
    private Instruction[][] _numberChunks;
    
    private long w = 0;
    private long x = 0;
    private long y = 0;
    private long z = 0;

    private int[] input;
    private int inputPos = 0;

    public string Run()
    {
        string[] lines = this.ReadLines();
        ParseLines(lines);
        string A = ReturnIfCorrect(new [] { 9, 9, 8, 9, 3, 9, 9, 9, 2, 9, 1, 9, 6, 7 });
        string B = ReturnIfCorrect(new [] { 3, 4, 1, 7, 1, 9, 1, 1, 1, 8, 1, 2, 1, 1 });
        return A + "\n" + B;
    }

    private string ReturnIfCorrect(int[] code)
    {
        if (ALU(code) == 0)
        {
            return string.Join("", code);
        }

        return "Z WAS NOT ZERO";
    }
    
    private long ALU(int[] code)
    {
        //Console.WriteLine(string.Join("", code));
        input = code;
        inputPos = 0;
        x = 0;
        y = 0;
        z = 0;
        w = 0;

        int instructionId = 0;
        foreach (var instruction in _instructions)
        {
            if (instruction.Type == InstructionType.Inp)
            {
                if (instructionId > 0)
                {
                    //Console.WriteLine($"x={x} y={y} z={z} w={w}, pos={inputPos - 1} val={input[inputPos - 1]}");
                }
            }
            Execute(instruction);
            instructionId++;
        }

        return z;
    }

    private void Execute(Instruction i)
    {
        long a = GetValue(i.Arg0);
        long b = GetValue(i.Arg1);

        long res = i.Type switch
        {
            InstructionType.Inp => input[inputPos++],
            InstructionType.Add => a + b,
            InstructionType.Mul => a * b,
            InstructionType.Div => a / b,
            InstructionType.Mod => a % b,
            InstructionType.Eql => a == b ? 1 : 0,
        };
        
        SetValue(i.Arg0[0], res);
    }

    private long GetValue(string c)
        => c switch
        {
            "w" => w,
            "x" => x,
            "y" => y,
            "z" => z,
            null => 0,
            _ => int.Parse(c),
        };

    private void SetValue(char c, long value)
    {
        _ = c switch
        {
            'w' => w = value,
            'x' => x = value,
            'y' => y = value,
            'z' => z = value,
        };
    }

    private void ParseLines(string[] lines)
    {
        foreach (var line in lines)
        {
            ParseLine(line);
        }
        
        _numberChunks = _instructions.Chunk(18).ToArray();
    }

    private void ParseLine(string line)
    {
        var instruction = new Instruction();
        var split = line.Split();
        instruction.Type = GetType(split[0]);
        instruction.Arg0 = split[1];
        if (split.Length > 2)
        {
            instruction.Arg1 = split[2];
        }
        _instructions.Add(instruction);
    }

    public InstructionType GetType(string s)
        => s switch
        {
            "inp" => InstructionType.Inp,
            "add" => InstructionType.Add,
            "mul" => InstructionType.Mul,
            "div" => InstructionType.Div,
            "mod" => InstructionType.Mod,
            "eql" => InstructionType.Eql,
        };
}
