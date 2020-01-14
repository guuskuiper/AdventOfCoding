using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;

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
        
        private List<(Operation op, int n)> Parse(IEnumerable<string> shuffles)
        {
            List<(Operation op, int n)> ops2 = new List<(Operation, int)>();

            foreach(var s in shuffles)
            {
                if(s.StartsWith(DEALINTO))
                {
                    //resultIndex = InverseSingleDealIntoStack(resultIndex, length);
                    ops2.Add( (Operation.DEALINTO, 0) );
                }
                else if(s.StartsWith(DEALINCREMENT))
                {
                    var nStr = s.Substring(DEALINCREMENT.Length + 1);
                    var n = int.Parse(nStr);
                    //resultIndex = InverseSingleDealWith(n, resultIndex, length);
                    ops2.Add( (Operation.DEALINCREMENT, n) );
                }
                else if(s.StartsWith(CUT))
                {
                    var nStr = s.Substring(CUT.Length + 1);
                    var n = int.Parse(nStr);
                    //resultIndex = InverseSingleCut(n, resultIndex, length);
                    ops2.Add( (Operation.CUT, n) );
                }
            }

            return ops2;
        }

        public long Inverse(long index, long length, long count, IEnumerable<string> shuffles)
        {
            long resultIndex = index;

            offsets = new Dictionary<long, long[]>();
            var operation = shuffles.Reverse();

            List<(Operation op, int n)> ops2 = Parse(operation);

            for (long i = 0; i < count; i++)
            {
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
            }

            return resultIndex;
        }

        public long InverseAB(long index, long length, long count, IEnumerable<string> shuffles)
        {
            var operation = shuffles.Reverse();

            List<(Operation op, int n)> ops2 = Parse(operation);

            var ab = CalcCycleAB(length, ops2);

            // TODO polypow
            // modpow the polynomial: (ax+b)^m % n
            // f(x) = ax+b
            // g(x) = cx+d
            // f^2(x) = a(ax+b)+b = aax + ab+b
            // f(g(x)) = a(cx+d)+b = acx + ad+b

            checked{
                var abCount = PolyPow(ab.a, ab.b, length, count);

                System.Console.WriteLine($"{ab},{abCount}");

                var negative = (abCount.a * index + abCount.b) % length;

                return (negative + length) % length;
            }
        }

        public long BigMult(long a, long b, long length)
        {
            BigInteger bigA = new BigInteger(a);
            BigInteger bigB = new BigInteger(b);

            checked{
                var ab = bigA * bigB;

                BigInteger.DivRem(ab, length, out var abMod);
                BigInteger.DivRem(abMod + length, length, out var abMod2);

                return (long)abMod2;
            }
        }

        public long BigMultAdd(long a, long b, long c, long length)
        {
            BigInteger bigA = new BigInteger(a);
            BigInteger bigB = new BigInteger(b);
            BigInteger bigC = new BigInteger(c);

            checked{
                var abc = bigA * bigB + bigC;

                BigInteger.DivRem(abc, length, out var abMod);
                BigInteger.DivRem(abMod + length, length, out var abMod2);

                return (long)abMod2;
            }
        }

        public (long a, long b) PolyPow(long a, long b, long length, long count)
        {
            checked {
                if(count == 0) return (1, 0);
                else if( (count % 2) == 0)
                {
                    return PolyPow(BigMult(a, a, length), BigMultAdd(a, a, b, length), length, count / 2);
                }
                else
                {
                    (long c, long d) = PolyPow(a, b, length, count - 1);
                    return ( BigMult(a, c, length), BigMultAdd(a, d, b, length));
                }
            }
        }

        private (long a, long b) CalcCycleAB(long length, IEnumerable<(Operation op, int n)> operations)
        {
            // y = a*x + b % length
            long a = 1;
            long b = 0;
            foreach (var kvp in operations)
            {
                switch(kvp.op)
                {
                    case Operation.CUT:
                        //y = 1*x + length + n % length
                        //a *= 1;
                        b = (b + length + kvp.n) % length;
                        break;
                    case Operation.DEALINTO:
                        //y = -1*x + length - 1 (% length)
                        a = -a;
                        b = (length - 1 - b) % length;
                        break;
                    case Operation.DEALINCREMENT:
                        //y = invMod(n, length)*x % length (inverse)
                        var z = InvMod(kvp.n, length);
                        a = a * z % length;
                        b = b * z % length;
                        break;
                }

                a = (a + length) % length;
                b = (b + length) % length;
                //System.Console.WriteLine($"y = {a}*x + {b}");
            }

            return (a, b);
        }

        public static (long, long, long) EGCD(long a, long b)
        {
            if( b == 0) return (a, 1, 0);
            var (gcd, x, y) = EGCD(b, a % b);
            return (gcd, y, x - (long)(a/b) * y); // or (long)Math.Floor(a/b)
        }

        public static long InvMod(long a, long n)
        {
            var (g, x, y) = EGCD(n, a);
            return (y + n) % n;
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

//https://github.com/metalim/metalim.adventofcode.2019.python/blob/master/22_cards_shuffle.ipynb
// python '%' != c# '%'!!!!
// ## Returns gcd, x, y such that a*x + b*y = gcd
// def ext_gcd(a, b):
//     if b == 0:
//         return a, 1, 0
//     gcd, x, y = ext_gcd(b, a % b)
//     return gcd, y, x - (a//b) * y

// def inv_0(a, n):
//   g, x, y = ext_gcd(n, a)
//   assert g == 1  # n is prime
//   # Now we know x*n + y*a == 1, and x*n mod n is 0, so y is what we want:
//   # (Return % n to keep numbers small):
//   return y % n

// # modpow the polynomial: (ax+b)^m % n
// # f(x) = ax+b
// # g(x) = cx+d
// # f^2(x) = a(ax+b)+b = aax + ab+b
// # f(g(x)) = a(cx+d)+b = acx + ad+b