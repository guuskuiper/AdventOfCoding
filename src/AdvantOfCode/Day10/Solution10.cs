namespace AdventOfCode.Day10;

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

    public class Chunk
    {
        public List<Chunk> Chunks { get; set; }
    }

    private List<SyntaxError> _syntaxErrors = new List<SyntaxError>();
    private List<string> _completion = new List<string>();
    private Stack<char> opendChunks;
    private int lineNumber = 0;
    private int charNumber = 0;

    public string Run()
    {
        var lines = InputReader.ReadFileLines();
        
        ParseLines(lines);
        int A = GetSyntaxSore();
        long B = GetCompletionSore();
        
        return A + "\n" + B; // not: 192498059
    }

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
            _ => 0
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
            _ => 0
        };

    private void ParseLines(List<string> lines)
    {
        lineNumber = -1;

        foreach (var line in lines)
        {
            lineNumber++;
            
            if (!ParseLine(line))
            {
                continue;
            }
            
            CompleteLine();
        }
    }

    private bool ParseLine(string line)
    {
        bool succes = true;
        charNumber = 0;
        opendChunks = new Stack<char>();
        foreach (var c in line)
        {
            if (!ParseSyntaxErrorChar(c))
            {
                succes = false;
                break; 
            }
            
            charNumber++;
        }

        return succes;
    }

    private void CompleteLine()
    {
        string complete = "";
        while (opendChunks.Count > 0)
        {
            char openTop = opendChunks.Pop();
            complete += Pair(openTop);
        }
        _completion.Add(complete);
    }

    private bool ParseSyntaxErrorChar(char c)
    {
        bool success = true;
        if (IsOpen(c))
        {
            opendChunks.Push(c);
        }
        else
        {
            char top = opendChunks.Peek();
            char expected = Pair(top);

            if (expected == c)
            {
                opendChunks.Pop();
            }
            else
            {
                _syntaxErrors.Add(new SyntaxError
                {
                    Character = c,
                    Expected = expected,
                    CharacterNumber = charNumber,
                    LineNumber = lineNumber
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
            CLOSE_ANGLE => OPEN_ANGLE
        };
}
