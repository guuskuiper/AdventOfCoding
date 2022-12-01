using System.Diagnostics;

namespace AdventOfCode.Year2021.Day18;

using Pair = Solution18.BinaryTreeNode<int>;

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

    public class BinaryTreeNode<T>
    {
        public BinaryTreeNode(T value, BinaryTreeNode<T> left, BinaryTreeNode<T> right) : this(value)
        {
            Left = left;
            Right = right;
            Left.Parent = this;
            Right.Parent = this;
        }

        public BinaryTreeNode(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
        public BinaryTreeNode<T>? Left { get; private set; }
        public BinaryTreeNode<T>? Right { get; private set; }
        public BinaryTreeNode<T>? Parent { get; private set; }
        
        public bool IsLeaf => Left is null;
        
        public override string ToString()
        {
            if (Left is null || Right is null)
            {
                return Value!.ToString()!;
            }
            return $"[{Left},{Right}]";
        }
        
        public void ReplaceBy(BinaryTreeNode<T> newNode)
        {
            BinaryTreeNode<T> parent = Parent ?? throw new InvalidOperationException();

            if (parent.Left == this)
            {
                parent.Left = newNode;
            }
            else
            {
                parent.Right = newNode;
            }

            newNode.Parent = parent;
            Parent = null;
        }
    }

    private static class PairExtensions
    {

        public static BinaryTreeNode<int> Parse(ref NumberReader reader)
        {
            BinaryTreeNode<int> p;
            if (char.IsDigit(reader.Peek()))
            {
                p = new BinaryTreeNode<int>(reader.ReadInt());
            }
            else
            {
                var start = reader.ReadChar();
                Debug.Assert(start == '[');

                BinaryTreeNode<int> left = Parse(ref reader);

                var comma = reader.ReadChar();
                Debug.Assert(comma == ',');

                BinaryTreeNode<int> right = Parse(ref reader);

                var close = reader.ReadChar();
                Debug.Assert(close == ']');

                p = new BinaryTreeNode<int>(0, left, right);
            }

            return p;
        }
    }

    public string Run()
    {
        var lines = InputReader.ReadFileLines();

        long A = AdditionsMagnitude(lines);
        long B = LargestCombination(lines);

        return $"{A}\n{B}";
    }

    private long AdditionsMagnitude(List<string> lines)
    {
        var result = lines.Skip(1).Aggregate(ProcessLine(lines[0]), (sum, line) => Add(sum, ProcessLine(line)));
        return Magnitude(result);
    }

    private long LargestCombination(List<string> lines)
    {
        long maxMagnitude = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < lines.Count; j++)
            {
                if(i == j) continue;
                
                Pair sum = Add(ProcessLine(lines[i]), ProcessLine(lines[j]));
                long magnitude = Magnitude(sum);
                if (magnitude > maxMagnitude)
                {
                    maxMagnitude = magnitude;
                }
            }
        }

        return maxMagnitude;
    }

    private long Magnitude(BinaryTreeNode<int> p)
    {
        long sum = 0;
        
        if (p.IsLeaf)
        {
            sum = p.Value;
        }
        else
        {
            sum += 3 * Magnitude(p.Left!);
            sum += 2 * Magnitude(p.Right!);
        }

        return sum;
    }

    private Pair ProcessLine(string line)
    {
        var reader = new NumberReader(line);
        return PairExtensions.Parse(ref reader);
    }

    private Pair Add(Pair left, Pair right)
    {
        Pair added = new Pair(0, left, right);
        return Reduce(added);
    }

    private Pair Reduce(Pair p)
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
        return p;
    }

    private bool Explode(Pair root)
    {
        Pair? toExplode = FindDepth(root, 0, 4, out int depth);
        if (toExplode is null)
        {
            return false;
        }

        Pair? leftLeave = FindLeaveLeft(toExplode);
        if (leftLeave is not null)
        {
            leftLeave.Value += toExplode.Left!.Value;
        }

        Pair? rightLeave = FindLeaveRight(toExplode);
        if (rightLeave is not null)
        {
            rightLeave.Value += toExplode.Right!.Value;
        }

        toExplode.ReplaceBy(new Pair(0));
        return true;
    }

    private Pair? FindLeaveLeft(Pair start)
    {
        return FindLeave(start, pair => pair.Left!, pair => pair.Right!);
    }
    
    private Pair? FindLeaveRight(Pair start)
    {
        return FindLeave(start, pair => pair.Right!, pair => pair.Left!);
    }
    
    private Pair? FindLeave(Pair start, Func<Pair, Pair> targetBranchDirection, Func<Pair, Pair> leafDiction)
    {
        Pair current = start;
        while (current.Parent is not null)
        {
            Pair targetDirection = targetBranchDirection(current.Parent);
            if (targetDirection != current)
            {
                Pair branch = targetDirection;
                while (!branch.IsLeaf)
                {
                    branch = leafDiction(branch);
                }

                return branch;
            }
            
            current = current.Parent;
        }

        return null;
    }

    private Pair? FindDepth(Pair current, int currentDepth, int targetDepth, out int depth)
    {
        Pair? output;
        if (current.IsLeaf)
        {
            output = null;
            depth = -1;
        }
        else if (currentDepth < targetDepth)
        {
            Pair? l = FindDepth(current.Left!, currentDepth + 1, targetDepth, out int depthL);
            Pair? r = FindDepth(current.Right!, currentDepth + 1, targetDepth, out int depthR);
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
        if (root.IsLeaf)
        {
            return SplitLeaf(root);
        }

        if (Split(root.Left!))
        {
            return true;
        }
        
        if (Split(root.Right!))
        {
            return true;
        }

        return false;
    }

    private bool SplitLeaf(Pair n)
    {
        if (n.Value > 9)
        {
            n.ReplaceBy(CreateSplit(n));
            return true;
        }

        return false;
    }

    private Pair CreateSplit(Pair n)
    {
        int value = n.Value;

        Pair left = new Pair((int)Math.Floor(value / 2.0));
        Pair right = new Pair((int)Math.Ceiling(value / 2.0));

        return new Pair(0, left, right);
    }
}
