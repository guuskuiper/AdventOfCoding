using System.Diagnostics;

namespace AdventOfCode.Day18;

public class Solution18 : Solution
{
    public ref struct NumberReader
    {
        private readonly ReadOnlySpan<char> _data;
        private int _position;

        public NumberReader(ReadOnlySpan<char> data)
        {
            _data = data;
            _position = 0;
        }

        public int ReadInt()
        {
            return int.Parse(_data.Slice(_position++, 1));
        }

        public char ReadChar()
        {
            return _data[_position++];
        }

        public char Peek()
        {
            return _data[_position];
        }
    }

    public class Pair
    {
        public Pair(Pair left, Pair right)
        {
            Left = left;
            Right = right;
            Left.Parent = this;
            Right.Parent = this;
        }

#pragma warning disable CS8618
        protected Pair() { }
#pragma warning restore CS8618

        public Pair Left { get; private set; }
        public Pair Right { get; private set; }
        public Pair? Parent { get; private set; }
        public virtual bool IsLeaf => false;

        public override string ToString()
        {
            return $"[{Left},{Right}]";
        }

        public void ReplaceBy(Pair newPair)
        {
            Pair parent = Parent ?? throw new InvalidOperationException();

            if (parent.Left == this)
            {
                parent.Left = newPair;
            }
            else
            {
                parent.Right = newPair;
            }

            newPair.Parent = parent;
            Parent = null;
        }

        public static Pair Parse(ref NumberReader reader)
        {
            Pair p;
            if (char.IsDigit(reader.Peek()))
            {
                p = Number.Parse(ref reader);
            }
            else
            {
                var start = reader.ReadChar();
                Debug.Assert(start == '[');

                Pair left = Parse(ref reader);

                var comma = reader.ReadChar();
                Debug.Assert(comma == ',');

                Pair right = Parse(ref reader);

                var close = reader.ReadChar();
                Debug.Assert(close == ']');

                p = new Pair(left, right);
            }

            return p;
        }
    }

    public class Number : Pair
    {
        public Number(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
        public override bool IsLeaf => true;

        public override string ToString()
        {
            return Value.ToString();
        }

        public new static Number Parse(ref NumberReader reader)
        {
            return new Number(reader.ReadInt());
        }
    }

    public string Run()
    {
        var lines = InputReader.ReadFileLines();

        long A = AdditionsMagnitude(lines);
        long B = LargestCombination(lines);

        return A + "\n" + B;
    }

    private long AdditionsMagnitude(List<string> lines)
    {
        Pair root = ProcessLine(lines[0]);        
        foreach (var line in lines.Skip(1))
        {
            Pair current = ProcessLine(line);
            root = Add(root, current);
        }

        return Magnitude(root);
    }

    private long LargestCombination(List<string> lines)
    {
        long maxMagnitude = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < lines.Count; j++)
            {
                if(i == j) continue;
                
                Pair left = ProcessLine(lines[i]);
                Pair right = ProcessLine(lines[j]);
                Pair sum = Add(left, right);
                long magnitude = Magnitude(sum);
                if (magnitude > maxMagnitude)
                {
                    maxMagnitude = magnitude;
                }
            }
        }

        return maxMagnitude;
    }

    private long Magnitude(Pair p)
    {
        long sum = 0;
        
        if (p is Number n)
        {
            sum = n.Value;
        }
        else
        {
            sum += 3 * Magnitude(p.Left);
            sum += 2 * Magnitude(p.Right);
        }

        return sum;
    }

    private Pair ProcessLine(string line)
    {
        var reader = new NumberReader(line);
        var root = Pair.Parse(ref reader);
        return root;
    }

    private Pair Add(Pair left, Pair right)
    {
        Pair added = new Pair(left, right);

        Reduce(added);
        
        return added;
    }

    private void Reduce(Pair p)
    {
        bool moreWork = true; 
        while (moreWork)
        {
            moreWork = false;

            moreWork |= Explode(p);
            if (!moreWork)
            {
                moreWork |= Split(p);
            }
        }
    }

    private bool Explode(Pair root)
    {
        Pair toExplode = FindDepth(root, 0, 4, out int depth);
        if (depth < 4)
        {
            return false;
        }

        Number? leftLeave = FindLeaveLeft(toExplode);
        if (leftLeave is not null)
        {
            leftLeave.Value += toExplode.Left is Number n ? n.Value : 0;
        }

        Number? rightLeave = FindLeaveRight(toExplode);
        if (rightLeave is not null)
        {
            rightLeave.Value += toExplode.Right is Number n ? n.Value : 0;
        }

        toExplode.ReplaceBy(new Number(0));
        return true;
    }

    private Number? FindLeaveLeft(Pair start)
    {
        return FindLeave(start, pair => pair.Left, pair => pair.Right);
    }
    
    private Number? FindLeaveRight(Pair start)
    {
        return FindLeave(start, pair => pair.Right, pair => pair.Left);
    }
    
    private Number? FindLeave(Pair start, Func<Pair, Pair> targetBranchDirection, Func<Pair, Pair> leafDiction)
    {
        Pair current = start;
        Pair? branch = null;
        while (current.Parent is not null)
        {
            Pair previous = current;
            current = current.Parent;
            Pair targetDirection = targetBranchDirection(current);
            if (targetDirection != previous)
            {
                branch = targetDirection;
                break;
            }
        }

        if (branch is null) return null;
        
        Pair toValue = branch;
        while (!toValue.IsLeaf)
        {
            toValue = leafDiction(toValue);
        }

        return toValue is Number n ? n : null;

    }

    private Pair FindDepth(Pair current, int currentDepth, int targetDepth, out int depth)
    {
        Pair output;
        if (current.IsLeaf)
        {
            output = current;
            depth = -1;
        }
        else if (currentDepth < targetDepth)
        {
            Pair l = FindDepth(current.Left, currentDepth + 1, targetDepth, out int depthL);
            Pair r = FindDepth(current.Right, currentDepth + 1, targetDepth, out int depthR);
            if (depthR > depthL)
            {
                depth = depthR;
                output = r;
            }
            else
            {
                depth = depthL;
                output = l;
            }
        }
        else
        {
            depth = currentDepth;
            output = current;
        }

        return output;
    }

    private bool Split(Pair root)
    {
        bool splitted = false;

        if (root.Left is Number l)
        {
            if (l.Value > 9)
            {
                root.Left.ReplaceBy(CreateSplit(l));
                splitted = true;
            }
        }
        else
        {
            splitted = Split(root.Left);
        }

        if (!splitted)
        {
            if (root.Right is Number r)
            {
                if (r.Value > 9)
                {
                    root.Right.ReplaceBy(CreateSplit(r));
                    splitted = true;
                }
            }
            else
            {
                splitted = Split(root.Right);
            }
        }

        return splitted;
    }

    private Pair CreateSplit(Number n)
    {
        int value = n.Value;

        Number left = new Number((int)Math.Floor(value / 2.0));
        Number right = new Number((int)Math.Ceiling(value / 2.0));

        return new Pair(left, right);
    }
}
