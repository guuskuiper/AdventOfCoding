namespace AdventOfCode.Year2021.Day10;

[DayInfo(2021, 10)]
public class Solution10 : Solution
{
    private const char OPEN_PARENTHESIS = '(';
    private const char CLOSE_PARENTHESIS = ')';
    private const char OPEN_BRACE = '{';
    private const char CLOSE_BRACE = '}';
    private const char OPEN_BRACKET = '[';
    private const char CLOSE_BRACKET = ']';
    private const char OPEN_ANGLE = '<';
    private const char CLOSE_ANGLE = '>';

    public class SyntaxError
    {
        public char Character { get; set; }
        public char Expected { get; set; }
        public int LineNumber { get; set; }
        public int CharacterNumber { get; set; }
    }

    private readonly List<SyntaxError> _syntaxErrors = new ();
    private readonly List<string> _completion = new ();
    private Stack<char>? _openedChunks;
    private int _lineNumber = 0;
    private int _charNumber = 0;

    public string Run()
    {
        var lines = InputReader.ReadFileLines();
        
        ParseLines(lines);
        int A = GetSyntaxSore();
        long B = GetCompletionSore();
        
        return A + "\n" + B;
    }

    private void ParseLines(List<string> lines)
    {
        _lineNumber = -1;

        foreach (var line in lines)
        {
            _lineNumber++;
            
            if (!ParseLine(line))
            {
                continue;
            }
            
            CompleteLine();
        }
    }

    private bool ParseLine(string line)
    {
        bool success = true;
        _charNumber = 0;
        _openedChunks = new Stack<char>();
        foreach (var c in line)
        {
            if (!ParseSyntaxErrorChar(c))
            {
                success = false;
                break; 
            }
            
            _charNumber++;
        }

        return success;
    }

    private void CompleteLine()
    {
        string complete = "";
        while (_openedChunks!.Count > 0)
        {
            char openTop = _openedChunks.Pop();
            complete += Pair(openTop);
        }
        _completion.Add(complete);
    }

    private bool ParseSyntaxErrorChar(char c)
    {
        bool success = true;
        if (IsOpen(c))
        {
            _openedChunks!.Push(c);
        }
        else
        {
            char top = _openedChunks!.Peek();
            char expected = Pair(top);

            if (expected == c)
            {
                _openedChunks.Pop();
            }
            else
            {
                _syntaxErrors.Add(new SyntaxError
                {
                    Character = c,
                    Expected = expected,
                    CharacterNumber = _charNumber,
                    LineNumber = _lineNumber
                });
                success = false;
            }
        }

        return success;
    }

    private bool IsOpen(char c)
    {
        return c == OPEN_ANGLE ||
               c == OPEN_BRACE ||
               c == OPEN_BRACKET ||
               c == OPEN_PARENTHESIS;
    }

    private char Pair(char c)
        => c switch
        {
            OPEN_PARENTHESIS => CLOSE_PARENTHESIS,
            CLOSE_PARENTHESIS => OPEN_PARENTHESIS,
            OPEN_BRACKET => CLOSE_BRACKET,
            CLOSE_BRACKET => OPEN_BRACKET,
            OPEN_BRACE => CLOSE_BRACE,
            CLOSE_BRACE => OPEN_BRACE,
            OPEN_ANGLE => CLOSE_ANGLE,
            CLOSE_ANGLE => OPEN_ANGLE,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    
    private int GetSyntaxSore()
    {
        int score = 0;
        foreach (var syntaxError in _syntaxErrors)
        {
            score += GetErrorScore(syntaxError.Character);
        }

        return score;
    }

    private int GetErrorScore(char c)
        => c switch
        {
            CLOSE_PARENTHESIS => 3,
            CLOSE_ANGLE => 25137,
            CLOSE_BRACKET => 57,
            CLOSE_BRACE => 1197,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    
    private long GetCompletionSore()
    {
        List<long> scores = new List<long>();
        foreach (var completion in _completion)
        {
            scores.Add(GetCompletionLineScore(completion));
        }

        scores.Sort();

        int middle = (scores.Count - 1) / 2;

        return scores[middle];
    }

    private long GetCompletionLineScore(string line)
    {
        long score = 0;
        foreach (var c in line)
        {
            score *= 5;
            score += GetMissingScore(c);
        }

        return score;
    }
    
    private int GetMissingScore(char c)
        => c switch
        {
            CLOSE_PARENTHESIS => 1,
            CLOSE_ANGLE => 4,
            CLOSE_BRACKET => 2,
            CLOSE_BRACE => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
}
