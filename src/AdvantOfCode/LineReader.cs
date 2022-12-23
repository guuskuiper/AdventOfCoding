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

    public int ReadInt()
    {
        int start = _position;
        if (!IsDone && Peek() == '-') _position++;
        while (!IsDone && char.IsDigit(Peek()))
        {
            _position++;
        }

        var numbers = _data.Slice(start, _position - start);
        return int.Parse(numbers);
    }
    
    public char ReadChar()
    {
        return _data[_position++];
    }
        
    public void ReadChar(char expected)
    {
        char c = _data[_position++];
        if(c != expected) throw new Exception($"Incorrect character {c} at position {_position - 1}, expected {expected}");
    }

    public char Peek()
    {
        return _data[_position];
    }

    public void Skip(int characters)
    {
        _position += characters;
    }
}