using System.Collections;

namespace AdventOfCode.Day16;

public class Solution16 : Solution
{
    private const long ONE = 1;
    private const long ZERO = 0;
    private bool[] _bits;
    private int _position = 0;
    private long _versionSum = 0;
    
    public string Run()
    {
        var line = InputReader.ReadFileLines();

        _bits = GetBits(line[0]);
        long value = ParsePacket();
        
        return _versionSum + "\n" + value; // not 197445831835 to low
    }

    private long ParsePacket()
    {
        long value;
        
        var version = ParseNumber(3);
        var typeId = ParseNumber(3);

        _versionSum += version;

        if (typeId == 4)
        {
            value = ParseLiteralValue();
        }
        else
        {
            var values = ParseOperator();
            value = ExecuteOperator(typeId, values);
        }

        return value;
    }

    private long ExecuteOperator(long typeId, List<long> values) =>
        typeId switch
        {
            0 => values.Sum(),
            1 => values.Aggregate((long)1, (x,y) => x * y),
            2 => values.Min(),
            3 => values.Max(),
            5 => values[0] > values[1] ? 1 : 0,
            6 => values[0] < values[1] ? 1 : 0,
            7 => values[0] == values[1] ? 1 : 0,
        };

    private List<long> ParseOperator()
    {
        List<long> values = new();
        
        var lengthTypeId = ParseBit();
        if (lengthTypeId)
        {
            var numberOfSubPackets = ParseNumber(11);
            for (int i = 0; i < numberOfSubPackets; i++)
            {
                values.Add(ParsePacket());
            }
        }
        else
        {
            var totalLength = ParseNumber(15);
            long endPosition = _position + totalLength;
            while (_position < endPosition)
            {
                values.Add(ParsePacket());
            }
        }

        return values;
    }

    private long ParseLiteralValue()
    {
        List<bool> literalValue = new();
        bool[] currentBits;
        do
        {
            currentBits = ParseBits(5);
            literalValue.AddRange(currentBits[1..5]);
        } while (currentBits[0]);

        long value = GetNumber(literalValue.ToArray());

        //Console.WriteLine($"Literal: {value}");

        return value;
    }

    private bool ParseBit() => ParseBits(1).Single();
    
    private bool[] ParseBits(int length)
    {
        int start = _position;
        _position += length;
        return _bits[start .. _position];
    }

    private long ParseNumber(int length) => GetNumber(ParseBits(length));

    private long GetNumber(bool[] bits)
    {
        if (bits.Length >= 63)
        {
            throw new Exception("Long not enough bits");
        }
        long number = 0;
        for (int i = 0; i < bits.Length; i++)
        {
            number += (bits[i] ? ONE : ZERO) << (bits.Length - i - 1);
        }
        return number;
    }

    private bool[] GetBits(string s)
    {
        List<bool> bits = new();
        foreach (var c in s)
        {
            bits.AddRange(GetBits(c).Reverse());
        }
        return bits.ToArray();
    }

    private bool[] GetBits(char c)
    {
        int number = Convert.ToInt32(c.ToString(), 16);
        bool[] bits = new bool[4];
        for (int i = 0; i < bits.Length; i++)
        {
            int shift = 1 << i;
            bits[i] = (number & shift) == shift;
        }
        return bits;
    }
}
