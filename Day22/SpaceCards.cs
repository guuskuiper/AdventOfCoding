using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Day22
{
    public class SpaceCards
    {
        private const string DEALINTO = "deal into new stack";
        private const string DEALINCREMENT = "deal with increment";
        private const string CUT = "cut";
        private Dictionary<long, long[]> offsets;

        private List<int> stack; // TOP = index 0
        public SpaceCards(int count)
        {
            stack = Enumerable.Range(0, count).ToList();
            offsets = new Dictionary<long, long[]>();
            //stack = Enumerable.Range(0, 10007).ToList();
            //stack = Enumerable.Range(0, count).ToList();
            // stack = new List<long>();
            // for(int i = 0; i < count; i++)
            // {
            //     stack.Add(i);
            // }
        }

        public void Shuffle(IEnumerable<string> shuffle)
        {
            foreach(var s in shuffle)
            {
                Parse(s);
            }
            //CutN(-4);
            //DealWithIncrement(3);
            //Display();
        }

        public void ShuffleN(IEnumerable<string> shuffle, int n)
        {
            for(int i = 0; i < n; i++)
            {
                Shuffle(shuffle);
            }
        }

        public void Parse(string s)
        {
            if(s.StartsWith(DEALINTO))
            {
                DealIntoStack();
            }
            else if(s.StartsWith(DEALINCREMENT))
            {
                var nStr = s.Substring(DEALINCREMENT.Length + 1);
                var n = int.Parse(nStr);
                DealWithIncrement(n);
            }
            else if(s.StartsWith(CUT))
            {
                var nStr = s.Substring(CUT.Length + 1);
                var n = int.Parse(nStr);
                CutN(n);
            }
        }

        public void CutN(int n)
        {
            // positive
            if(n >= 0)
            {
                var tempStack = stack.GetRange(0, n);
                stack.RemoveRange(0, n);
                stack.AddRange(tempStack);
            }
            else
            {
                var absN = Math.Abs(n);
                var tempStack = stack.GetRange(stack.Count - absN, absN);
                stack.RemoveRange(stack.Count - absN, absN);
                stack.InsertRange(0, tempStack);
            }
        }

        public void DealIntoStack()
        {
            stack.Reverse();
        }

        public void DealWithIncrement(int n)
        {
            int newPos = 0;
            var newStack = new int[stack.Count];
            for(int i = 0; i < stack.Count; i++)
            {
                newStack[newPos] = stack[i];
                newPos = (newPos + n) % stack.Count; 
            }

            stack = newStack.ToList();

            CacheOffsets(n, stack.Count);
        }

        public void Display()
        {
            System.Console.WriteLine(string.Join(' ', stack));
        }

        public void Find(int n)
        {
            var card = stack.IndexOf(n);
            System.Console.WriteLine($"Card {card} @ position {n}");
        }

        public void InverseCut(int n)
        {
            CutN(-n);
        }

        public void InverseDealIntoStack()
        {
            DealIntoStack();
        }

        public int[] InverseDealWithIncrement(int n)
        {
            // int newPos = 0;
            // var newStack = new int[stack.Count];
            // for(int i = 0; i < stack.Count; i++)
            // {
            //     newStack[newPos] = stack[i];
            //     newPos = (newPos + n) % stack.Count; 
            // }

            var offset = CacheOffsets(n, stack.Count);

            // stack = newStack.ToList();
            var newStack = new int[stack.Count];
            for(int i = 0; i < stack.Count; i+=n)
            {
                for(int j = 0; j < n; j++)
                {
                    var start = (int)offset[j];
                    if(i + j < stack.Count)// && start + i/n < stack.Count)
                    {
                        newStack[start + i/n] = stack[i+j];
                    }
                }
            }

            return newStack;
            // offset for pos > 0 && pos < n
            //offset1 = 
        }

        public int[] InverseDealWithIncrement2(int n)
        {
            var newStack = new int[stack.Count];
            for(int i = 0; i < stack.Count; i++)
            {
                var fromIndex = (int)InverseSingleDealWith(n, i, stack.Count);
                System.Console.WriteLine($"i = {i} <= {fromIndex}");
                newStack[fromIndex] = stack[i];
            }
            return newStack;
        }

        public long InverseSingleDealWith(int n, long index, long length)
        {
            var offset = CacheOffsets(n, length);

            var remain = index % n;
            var divid = index / n;
            var start = offset[remain];

            //return index - start + remain + n*divid;
            return start + divid;
        }

        public long InverseSingleCut(int n, long index, long length)
        {
            return (length + index + n) % length;
        }

        public long InverseSingleDealIntoStack(long index, long length)
        {
            return length - 1 - index;
        }

        public long[] CacheOffsets(int n, long length)
        {
            if(offsets.ContainsKey(n))
            {
                return offsets[n];
            }
            else
            {
                var offset = FindOffsets(n, length);
                offsets.Add(n, offset);
                return offset;
            }
        }

        public void PrintCache()
        {
            foreach(var kvp in offsets)
            {
                System.Console.WriteLine($"n={kvp.Key}, offsets {string.Join(',', kvp.Value)}");
            }
        }

        public long[] FindOffsets(int n, long length)
        {
            var res = new long[n];
            for(long i = 0; i < n; i++)
            {
                long offset = i;
                while(offset % n != 0)
                {
                    offset += length;
                    if(offset > (n * length))
                    {
                        throw new ArgumentOutOfRangeException($"Non-inversible operation? {n} - {length}");
                    }
                }
                res[i] = offset / n;
            }

            return res;
        }

        public enum Operation
        {
            CUT = 1,
            DEALINTO = 2,
            DEALINCREMENT = 3,
        }

        public long Inverse(long index, long length, long count, IEnumerable<string> shuffles)
        {
            long resultIndex = index;
            long startDiff = 0;
            long startOutput = 0;

            offsets = new Dictionary<long, long[]>();
            var operation = shuffles.Reverse();

            var percent = count > 1000000 ? (long)(count / 1000000) : 1;


            List<Action<long,long>> ops = new List<Action<long, long>>();

            List<(Operation op, int n)> ops2 = new List<(Operation, int)>();

            foreach(var s in operation)
            {
                if(s.StartsWith(DEALINTO))
                {
                    //ops.Add( InverseSingleDealIntoStack(resultIndex, length));
                    //resultIndex = InverseSingleDealIntoStack(resultIndex, length);
                    ops2.Add( (Operation.DEALINTO, 0) );
                }
                else if(s.StartsWith(DEALINCREMENT))
                {
                    var nStr = s.Substring(DEALINCREMENT.Length + 1);
                    var n = int.Parse(nStr);
                    //ops.Add( InverseSingleDealWith(n, resultIndex, length));
                    //resultIndex = InverseSingleDealWith(n, resultIndex, length);
                    ops2.Add( (Operation.DEALINCREMENT, n) );
                }
                else if(s.StartsWith(CUT))
                {
                    var nStr = s.Substring(CUT.Length + 1);
                    var n = int.Parse(nStr);
                    //ops.Add( InverseSingleCut(n, resultIndex, length));
                    //resultIndex = InverseSingleCut(n, resultIndex, length);
                    ops2.Add( (Operation.CUT, n) );
                }
            }

            System.Console.WriteLine(ops);

            var write = File.CreateText("output.txt");
            for (long i = 0; i < count; i++)
            {
                var startIndex = resultIndex;
                foreach (var kvp in ops2)
                {
                    switch(kvp.op)
                    {
                        case Operation.CUT:
                            resultIndex = InverseSingleCut(kvp.n, resultIndex, length);
                            break;
                        case Operation.DEALINCREMENT:
                            resultIndex = InverseSingleDealWith(kvp.n, resultIndex, length);
                            break;
                        case Operation.DEALINTO:
                            resultIndex = InverseSingleDealIntoStack(resultIndex, length);
                            break;
                    }
                }
                var endIndex = resultIndex;

                var diff = (length + endIndex - startIndex) % length;

                if(i == 0)
                {
                    startDiff = diff;
                    startOutput = endIndex;
                }
                else
                {
                    if(endIndex == startOutput)
                    {
                        throw new Exception("Repeated output at " + i);
                    }
                    if(diff == startDiff)
                    {
                        throw new Exception("Repeated diff at " + i);
                    }
                }

                if((i % percent) == 0) 
                {
                    System.Console.WriteLine($"i{i} => {resultIndex}");
                }
                write.WriteLine(endIndex);
            }
            write.Close();

            return resultIndex;
        }
    }
}
//0123456789012345678901234567890123456789
//0..1..2..3..4..5..6..7..8..9..            => 0, 7, 4
//0741852963
//0740740740 start
//0741852963 n * div
//0123456789

//0123456789012345678901234567890123456789012345678901234567890123456789
//0......1......2......3......4......5......6......7......8......9...... => 0, 3, 6, 9, 2, 5, 8 (1, 4, 7)