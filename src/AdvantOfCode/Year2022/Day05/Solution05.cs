namespace AdventOfCode.Year2022.Day05;

[DayInfo(2022, 05)]
public class Solution05 : Solution
{
    private const string START = "move ";
    private const string FROM = " from ";
    private const string TO = " to ";
    
    private record Move(int Count, int From, int To);

    private List<Move> _moves = new();
    private List<Stack<char>> _stacks = new();
    
    public string Run()
    {
        string[] input = this.ReadLines(StringSplitOptions.None);

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
        foreach (Stack<char> stack in _stacks)
        {
            top += stack.Peek();
        }

        return top;
    }

    private void ExecuteB()
    {
        foreach (Move move in _moves)
        {
            var items = Pop(move.From, move.Count);
            Push(move.To, items.Reverse());
        }
    }

    private void Execute()
    {
        foreach (Move move in _moves)
        {
            var items = Pop(move.From, move.Count);
            Push(move.To, items);
        }
    }
    
    
    private char Pop(int number) => _stacks[number - 1].Pop();

    private IEnumerable<char> Pop(int number, int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return Pop(number);
        }
    }
    private void Push(int number, char c) => _stacks[number - 1].Push(c);

    private void Push(int number, IEnumerable<char> chars)
    {
        foreach (var c in chars)
        {
            Push(number, c);
        }
    }

    private void Parse(Span<string> lines)
    {
        int emptyLineIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i]))
            {
                emptyLineIndex = i;
                break;
            }
        }
        ParseStacks(lines.Slice(0, emptyLineIndex));
        ParseMoves(lines.Slice(emptyLineIndex+1));
    }
    
    private void ParseStacks(Span<string> lines)
    {
        _stacks = new List<Stack<char>>();

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
            _stacks.Add(stack);
        }
    }
    
    private void ParseMoves(Span<string> lines)
    {
        _moves = new List<Move>();

        foreach (var line in lines)
        {
            if (line.Length > 0)
            {
                _moves.Add(ParseMove(line));
            }
        }
    }

    private Move ParseMove(ReadOnlySpan<char> line)
    {
        var start = line.Slice(START.Length);
        var indexOfFrom = start.IndexOf(FROM);
        var count = int.Parse(start.Slice(0, indexOfFrom));

        var remaining = start.Slice(indexOfFrom + FROM.Length);
        var indexOfTo = remaining.IndexOf(TO);
        var from = int.Parse(remaining.Slice(0, indexOfTo));

        var to = int.Parse(remaining.Slice(indexOfTo + TO.Length));
        return new Move(count, from, to);
    }
}
