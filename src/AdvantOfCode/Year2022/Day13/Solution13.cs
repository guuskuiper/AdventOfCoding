using System.Diagnostics;
using System.Text.Json.Nodes;

namespace AdventOfCode.Year2022.Day13;

public class Solution13 : Solution
{
    private record Pair(Data Left, Data Right);
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        var pairs = Parse(lines);
        
        // TODO: parse as Json.
        JsonNode? root = JsonNode.Parse(lines[0]);
        
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
        var divider = ParsePair(new [] {"[[2]]", "[[6]]"});
        packets.Add(divider.Left);
        packets.Add(divider.Right);
        
        packets.Sort(CompareData);

        int divider1 = 1 + packets.IndexOf(divider.Left);
        int divider2 = 1 + packets.IndexOf(divider.Right);

        int key = divider1 * divider2;

        return sumValidIndices + "\n" + key;
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
        for (int leftIndex = 0; leftIndex < left.Items.Count; leftIndex++)
        {
            if (leftIndex >= right.Items.Count) return +1;

            int result = CompareData(left.Items[leftIndex], right.Items[leftIndex]);
            if (result != 0) return result;
        }

        return left.Items.Count == right.Items.Count ? 0 : -1;
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
    
    private abstract record Data
    {
        public static Data Parse(string data)
        {
            if (char.IsDigit(data[0]))
            {
                return IntegerData.Parse(data);
            }

            return ListData.Parse(data);
        }
    }
    private record IntegerData(int Value) : Data, IParsable<IntegerData>
    {
        public override string ToString() => Value.ToString();

        public static IntegerData Parse(string data, IFormatProvider? provider = null) => new (int.Parse(data));
        public static bool TryParse(string? s, IFormatProvider? provider, out IntegerData result)
        {
            if (int.TryParse(s, provider, out int value))
            {
                result = new IntegerData(value);
            }

            result = default!;
            return false;
        }
    }

    private record ListData(List<Data> Items) : Data, IParsable<ListData>
    {
        public override string ToString() => "[" + string.Join(',', Items) + "]";

        public static bool TryParse(string? s, IFormatProvider? provider, out ListData result)
        {
            result = Parse(s!, provider);
            return true;
        }
        
        public static ListData Parse(string data, IFormatProvider? provider = null)
        {
            string removeList = data.Substring(1, data.Length - 2);
            List<Data> content = new List<Data>();

            int depth = 0;
            int containedListStart = -1;
            string digits = "";

            for (int i = 0; i < removeList.Length; i++)
            {
                char c = removeList[i];

                if (c == '[')
                {
                    if (depth == 0) containedListStart = i;
                    depth++;
                }
                else if (c == ']')
                {
                    depth--;
                }
            
                if (depth == 0)
                {
                    if(char.IsDigit(c))
                    {
                        digits += c;
                    }
                    else if (c == ']')
                    {
                        int containedListEnd = i;
                        string innerList = removeList.Substring(containedListStart, containedListEnd - containedListStart + 1);
                        content.Add(Data.Parse(innerList));
                    }
                    else if(digits.Length > 0)
                    {
                        content.Add(IntegerData.Parse(digits));
                        digits = "";
                    }
                }
            }

            if (digits.Length > 0)
            {
                content.Add(IntegerData.Parse(digits));
            }

            return new ListData(content);
        }
    }
}
