namespace AdventOfCode;

public ref struct LineReader
{
    private readonly ReadOnlySpan<char> _data;
    private int _position;

    public LineReader(ReadOnlySpan<char> data)
    {
        _data = data;
        _position = 0;
    }

    public bool IsDone => _position >= _data.Length;
    public bool IsDigit => char.IsDigit(Peek());
    public bool IsLetter => char.IsLetter(Peek());
    public bool IsWhitespace => char.IsWhiteSpace(Peek());

    public int ReadInt()
    {
        int start = _position;
        if (!IsDone && Peek() == '-') _position++;
        while (!IsDone && IsDigit)
        {
            _position++;
        }

        ReadOnlySpan<char> numbers = _data.Slice(start, _position - start);
        return int.Parse(numbers);
    }
    
    public char ReadChar()
    {
        return _data[_position++];
    }
    
    public ReadOnlySpan<char> ReadLetters()
    {
        int start = _position;
        
        while (!IsDone && IsLetter)
        {
            _position++;
        }

        return _data.Slice(start, _position - start);
    }
        
    public void ReadChar(char expected)
    {
        char c = _data[_position++];
        if(c != expected) throw new Exception($"Incorrect character {c} at position {_position - 1}, expected {expected}");
    }
    
    public void ReadChars(ReadOnlySpan<char> expected)
    {
        foreach (var c in expected)
        {
            ReadChar(c);
        }
    }

    public void Skip(int characters)
    {
        _position += characters;
    }
    
    public void SkipWhitespaces()
    {
        while (!IsDone && IsWhitespace)
        {
            _position++;
        }
    }
    
    private char Peek()
    {
        return _data[_position];
    }
}