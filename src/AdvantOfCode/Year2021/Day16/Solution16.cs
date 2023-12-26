namespace AdventOfCode.Year2021.Day16;

[DayInfo(2021, 16)]
public class Solution16 : Solution
{
    private const long ONE = 1;
    private const long ZERO = 0;
    private bool[] _bits;
    private int _position = 0;
    private long _versionSum = 0;

    public class BitsReader
    {
        private readonly ReadOnlyMemory<char> _bits;
        private int _position;

        public BitsReader(ReadOnlyMemory<char> bits)
        {
            _bits = bits;
            _position = 0;
        }

        public int Position => _position;

        public bool GetBit()
        {
            return GetBits(1) != 0;
        }

        public long GetBits(int length)
        {
            if (length <= 0)
            {
                return 0;
            }
            long value = Convert.ToInt64(_bits.Slice(_position, length).ToString(), 2);
            _position += length;
            return value;
        }
    }

    private BitsReader _bitsReader;
    
    public string Run()
    {
        string[] lines = this.ReadLines();
        string line = lines[0];
        var binaryString = Hex2BitString(line);

        _bitsReader = new BitsReader(binaryString.AsMemory());
        long value = ParsePacket();
        
        return _versionSum + "\n" + value;
    }

    private string Hex2BitString(string hex) => 
        string.Join(null, hex.ToCharArray().Select(HexChar2BitString));

    private string HexChar2BitString(char h) => 
        Convert.ToString(Convert.ToInt32(h.ToString(), 16), 2).PadLeft(4, '0');
    
    private long ParsePacket()
    {
        var version = _bitsReader.GetBits(3);
        var typeId = _bitsReader.GetBits(3);

        _versionSum += version;

        List<long> values;
        if (typeId == 4)
        {
            values = new () { ParseLiteralValue() };
        }
        else
        {
            values = ParseOperator();
        }

        return ExecuteOperator(typeId, values);
    }

    private long ExecuteOperator(long typeId, List<long> values) =>
        typeId switch
        {
            0 => values.Sum(),
            1 => values.Aggregate(ONE, (x,y) => x * y),
            2 => values.Min(),
            3 => values.Max(),
            4 => values.Single(),
            5 => values[0] > values[1] ? 1 : 0,
            6 => values[0] < values[1] ? 1 : 0,
            7 => values[0] == values[1] ? 1 : 0,
        };

    private List<long> ParseOperator()
    {
        List<long> values = new();
        
        var lengthTypeId = _bitsReader.GetBit();
        if (lengthTypeId)
        {
            var numberOfSubPackets = _bitsReader.GetBits(11);
            for (int i = 0; i < numberOfSubPackets; i++)
            {
                values.Add(ParsePacket());
            }
        }
        else
        {
            var totalLength = _bitsReader.GetBits(15);
            long endPosition = _bitsReader.Position + totalLength;
            while (_bitsReader.Position < endPosition)
            {
                values.Add(ParsePacket());
            }
        }

        return values;
    }

    private long ParseLiteralValue()
    {
        long literalValue = 0;
        bool moreBits;
        do
        {
            literalValue <<= 4;
            moreBits = _bitsReader.GetBit();
            literalValue |= _bitsReader.GetBits(4);
        } while (moreBits);

        return literalValue;
    }
}
