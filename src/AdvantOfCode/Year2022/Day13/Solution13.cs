using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AdventOfCode.Year2022.Day13;

public class Solution13 : Solution
{
    private const string Divider2 = "[[2]]";
    private const string Divider6 = "[[6]]";

    public ref struct DataReader
    {
        private readonly ReadOnlySpan<char> _data;
        private int _position;

        public DataReader(ReadOnlySpan<char> data)
        {
            _data = data;
            _position = 0;
        }

        public bool Done() => _position >= _data.Length;

        public int ReadInt()
        {
            int start = _position;
            while (char.IsDigit(Peek()))
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
    }
    
    private record Pair(Data Left, Data Right);
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();

        string resultCustom = SolveCustomParser(lines);
        string resultJson = SolveJson(lines);
        if (resultCustom != resultJson) throw new Exception("Custom and Json result not the same");
        
        return resultCustom;
    }

    private string SolveCustomParser(IEnumerable<string> lines)
    {
        var pairs = Parse(lines);
        
        int sumValidIndices = 0;
        for (var index = 0; index < pairs.Count; index++)
        {
            Pair pair = pairs[index];
            int result = ComparePair(pair);
            if (result < 0)
            {
                sumValidIndices += index + 1;
            }
        }

        List<Data> packets = pairs.SelectMany(x => new [] {x.Left, x.Right}).ToList();
        var divider = ParsePair(new [] {Divider2, Divider6});
        packets.Add(divider.Left);
        packets.Add(divider.Right);
        
        packets.Sort(CompareData);

        int divider1 = 1 + packets.IndexOf(divider.Left);
        int divider2 = 1 + packets.IndexOf(divider.Right);

        int key = divider1 * divider2;

        return sumValidIndices + "\n" + key;
    }

    private string SolveJson(IEnumerable<string> lines)
    {
        JsonNode[] nodes = lines.Select(x => JsonNode.Parse(x)!).ToArray();
        var pairs = nodes.Chunk(2).ToList();

        int sumValidIndices = 0;
        for (var index = 0; index < pairs.Count; index++)
        {
            JsonNode[] pair = pairs[index];
            int result = CompareJson(pair[0], pair[1]);
            if (result < 0)
            {
                sumValidIndices += index + 1;
            }
        }

        List<JsonNode> packets = nodes.ToList();
        JsonNode divider2 = JsonNode.Parse(Divider2)!;
        JsonNode divider6 = JsonNode.Parse(Divider6)!;
        
        packets.Add(divider2);
        packets.Add(divider6);
        
        packets.Sort(CompareJson);

        int divider2Index = 1 + packets.IndexOf(divider2);
        int divider6Index = 1 + packets.IndexOf(divider6);

        int key = divider2Index * divider6Index;

        return sumValidIndices + "\n" + key;
    }
    
    private int CompareJson(JsonNode left, JsonNode right)
    {
        if (left is JsonValue li && right is JsonValue ri)
        {
            return CompareJson(li, ri);
        }
        
        JsonArray lld = left as JsonArray ?? CreateJsonArray(left);
        JsonArray rld = right as JsonArray ?? CreateJsonArray(right);
        return CompareJson(lld, rld);
    }

    private JsonArray CreateJsonArray(JsonNode value)
    {
        JsonValue valueNode = (JsonValue)value;
        return new JsonArray(JsonValue.Create(valueNode.GetValue<int>()));
    }
    
    private int CompareJson(JsonValue left, JsonValue right)
    {
        return left.GetValue<int>().CompareTo(right.GetValue<int>());
    }
    
    private int CompareJson(JsonArray left, JsonArray right)
    {
        for (int leftIndex = 0; leftIndex < left.Count; leftIndex++)
        {
            if (leftIndex >= right.Count) return +1;

            int result = CompareJson(left[leftIndex]!, right[leftIndex]!);
            if (result != 0) return result;
        }

        return left.Count == right.Count ? 0 : -1;
    }

    private int ComparePair(Pair pair) => CompareData(pair.Left, pair.Right);
    
    private int CompareData(Data left, Data right)
    {
        if (left is IntegerData li && right is IntegerData ri)
        {
            return CompareData(li, ri);
        }
        
        ListData lld = left as ListData ?? new ListData(new List<Data>{ left });
        ListData rld = right as ListData ?? new ListData(new List<Data>{ right });
        return CompareData(lld, rld);
    }
    
    private int CompareData(IntegerData left, IntegerData right)
    {
        return left.Value.CompareTo(right.Value);
    }
    
    private int CompareData(ListData left, ListData right)
    {
        for (int leftIndex = 0; leftIndex < left.Count; leftIndex++)
        {
            if (leftIndex >= right.Count) return +1;

            int result = CompareData(left[leftIndex], right[leftIndex]);
            if (result != 0) return result;
        }

        return left.Count == right.Count ? 0 : -1;
    }

    private List<Pair> Parse(IEnumerable<string> lines)
    {
        List<Pair> pairs = new();
        foreach (var chunk in lines.Chunk(2))
        {
            pairs.Add(ParsePair(chunk));
        }

        return pairs;
    }
        
    private Pair ParsePair(string[] lines)
    {
        Data left = Data.Parse(lines[0]);
        Data right = Data.Parse(lines[1]);
        
        Debug.Assert(left.ToString() == lines[0]);
        Debug.Assert(right.ToString() == lines[1]);
        
        return new (left, right);
    }
    
    private abstract record Data : IParsable<Data>
    {
        public static Data Parse(string data, IFormatProvider? provider = null)
        {
            DataReader reader = new DataReader(data);
            return Parse(ref reader);
        }

        public static bool TryParse(string? s, IFormatProvider? provider, out Data result)
        {
            result = default!;
            return false;
        }

        protected static Data Parse(ref DataReader reader)
        {
            if (char.IsDigit(reader.Peek()))
            {
                return IntegerData.ParseData(ref reader);
            }

            return ListData.ParseData(ref reader);
        }
    }
    private record IntegerData(int Value) : Data
    {
        public override string ToString() => Value.ToString();

        public static IntegerData ParseData(ref DataReader reader)
        {
            return new IntegerData(reader.ReadInt());
        }
    }

    private record ListData(List<Data> Items) : Data
    {
        private List<Data> Items { get; } = Items;
        
        public int Count => Items.Count;
        public Data this[int index] => Items[index];
        
        public override string ToString() => "[" + string.Join(',', Items) + "]";

        public static ListData ParseData(ref DataReader reader)
        {
            List<Data> content = new();
            reader.ReadChar('[');

            bool expectComma = false;
            while (reader.Peek() != ']')
            {
                if (expectComma)
                {
                    reader.ReadChar(',');
                }
                else
                {
                    Data current = Data.Parse(ref reader);
                    content.Add(current);
                }

                expectComma = !expectComma;
            }            
            
            reader.ReadChar(']');

            return new ListData(content);
        }
    }
}
