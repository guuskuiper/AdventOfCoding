namespace AdventOfCode.Year2022.Day05;

public class Solution05 : Solution
{
    private record Move(int Count, int From, int To);

    private List<Move> moves = new();
    private List<Stack<char>> stacks = new();
    
    public string Run()
    {
        string[] input = InputReader.ReadFile().Replace("\r\n", "\n").Split('\n');

        Parse(input);
        Execute();
        string a = Top();

        Parse(input);
        ExecuteB();
        string b = Top();
        
        return a + "\n" + b;
    }

    private string Top()
    {
        string top = "";
        foreach (Stack<char> stack in stacks)
        {
            top += stack.Peek();
        }

        return top;
    }

    private void ExecuteB()
    {
        Stack<char> temp = new Stack<char>();
        foreach (Move move in moves)
        {
            for (int i = 0; i < move.Count; i++)
            {
                char c = stacks[move.From - 1].Pop();
                temp.Push(c);
            }

            while (temp.Count > 0)
            {
                char c = temp.Pop();
                stacks[move.To - 1].Push(c);
            }
        }
    }
    
    private void Execute()
    {
        foreach (Move move in moves)
        {
            for (int i = 0; i < move.Count; i++)
            {
                char c = stacks[move.From - 1].Pop();
                stacks[move.To - 1].Push(c);
            }
        }
    }

    private void Parse(Span<string> lines)
    {
        int split = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i]))
            {
                split = i;
                break;
            }
        }
        ParseStacks(lines.Slice(0, split));
        ParseMoves(lines.Slice(split+1));
    }
    
    private void ParseStacks(Span<string> lines)
    {
        stacks = new List<Stack<char>>();

        for (int i = 0; i < 9; i++)
        {
            int x = i * 4 + 1;
            Stack<char> stack = new Stack<char>();
            for (int j = lines.Length - 2; j >= 0; j--)
            {
                string line = lines[j];
                char c = line[x];
                if (char.IsAsciiLetter(c))
                {
                    stack.Push(c);
                }
            }
            stacks.Add(stack);
        }
    }
    
    private void ParseMoves(Span<string> lines)
    {
        moves = new List<Move>();

        foreach (var line in lines)
        {
            if (line.Length > 0)
            {
                moves.Add(ParseMove(line));
            }
        }
    }

    private const string START = "move ";
    private const string FROM = " from ";
    private const string TO = " to ";
    private Move ParseMove(ReadOnlySpan<char> line)
    {
        var start = line.Slice(START.Length);
        var indexOfMiddle = start.IndexOf(" from ");
        var count = int.Parse(start.Slice(0, indexOfMiddle));

        var remaining = start.Slice(indexOfMiddle + FROM.Length);
        var indexOfEnd = remaining.IndexOf(TO);
        var from = int.Parse(remaining.Slice(0, indexOfEnd));

        var to = int.Parse(remaining.Slice(indexOfEnd + TO.Length));
        return new Move(count, from, to);
    }
}
