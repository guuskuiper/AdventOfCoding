using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day18
{
    public class Maze
    {
        private const char WALL = '#';
        private const char EMPTY = '.';
        private const char START = '@';

        private char[,] grid;
        private int width;
        private int height;

        private Dictionary<char, (int x, int y)> specials;

        public Maze(string[] input)
        {
            width = input[0].Length;
            height = input.Length;

            System.Console.WriteLine($"Maze {width}x{height}");

            grid = new char[width, height];
            for (int y = 0; y < height; y++)
            {
                var line = input[y];
                System.Console.WriteLine(line);
                for (int x = 0; x < width; x++)
                {
                    grid[x,y] = line[x];
                }
            }

            // System.Console.WriteLine("@: " + FindChar('@'));
            // System.Console.WriteLine("A: " + FindChar('A'));
            // System.Console.WriteLine("a: " + FindChar('a'));

            FindAllSpecials();

            foreach(var ch in Enumerable.Range('a', specials.Count).Select(c => (char)c).Where(x => char.IsLower(x)))
            {
                System.Console.WriteLine(ch + CalcuteShortestPath(START, ch).ToString());
            }
        }

        public void Solve()
        {
            // Total length < 4252
            System.Console.WriteLine(CalcuteShortestPath(START, 'n'));
            //CalcuteShortestPath('h', 'p', true);

            //Display();

            foreach(var q in Enumerable.Range(0, 4))
            {
                FindInQuarter(q);
            }
            // Q0: dlmr            (cp)
            // Q1: cfqwx           (ghnr)
            // Q2: abgijknstuvyz   (fx)
            // Q3: ehop            (none)
            System.Console.WriteLine(SolveExact("nrfgcqx", "e")); // Q3 + Q0
            // Q2: ng ("many", svtz) requires: CP
            // Q1: cqf (wx) requires: RHN
            // Q0: r (dlm) requires: -

            System.Console.WriteLine(SolveExact("ehop", START.ToString())); // Q3
            // System.Console.WriteLine(SolveExact("nr", "e")); // Q3 -> Q2 / Q1
            // System.Console.WriteLine(CalcuteShortestPath('n', 'f')); // Q2 -> Q1
            // System.Console.WriteLine(CalcuteShortestPath('f', 'g')); // Q1 -> Q2
            // System.Console.WriteLine(SolveExact("cqx", "g")); //Q2 -> Q1
            System.Console.WriteLine(SolveExact("dlmr", "x")); // Q1 -> Q0
            //System.Console.WriteLine(CalcuteShortestPath('d', 'z', true));
            //System.Console.WriteLine(SolveExact("tvzs", "d"));
            System.Console.WriteLine(CalcuteShortestPath('d', 's')); // Q0 -> Q2
            System.Console.WriteLine(CalcuteShortestPath('s', 'v'));
            System.Console.WriteLine(CalcuteShortestPath('v', 't'));
            System.Console.WriteLine(CalcuteShortestPath('t', 'z'));

            System.Console.WriteLine(CalcuteShortestPath('e', 'h', true));

            Display();

            var res = CalcuteShortestPathSeries(START, 'e', 'h', 'o', 'p', START);
            System.Console.WriteLine("Final: " + CalcuteShortestPathSeries(START, 'p', 'o', 'h', 'e', 'n', 'r', 'f', 'c', 'g', 'q', 'x', 'm', 'l', 'r', 'd', 's', 'v', 't', 'z').ToString());
            System.Console.WriteLine(res);

            // Top-left: d, l, m, r ( r before d)
            //Combination('m', 'l', 'r', 'd');
            // 448, multiple, requires: CP
            // foreach(var word in permute("dlmr").Where(x => x.IndexOf('r') < x.IndexOf('d')))
            // {
            //     System.Console.WriteLine(word);
            //     var word2 = START + word + START;
            //     CalcuteShortestPathSeries(word2.ToCharArray());
            // }
            //CalcuteShortestPathSeries("@lrdm@".ToCharArray());
            // CalcuteShortestPathSeries(START, 'm', 'l', 'r', 'd', START);
            // CalcuteShortestPathSeries(START, 'l', 'm', 'r', 'd', START);
            // CalcuteShortestPathSeries(START, 'l', 'r', 'd', 'm', START);
            // CalcuteShortestPathSeries(START, 'r', 'd', 'm', 'l', START);
            

            // Top-right: f, w, q, x, c
            // requires: RHNG
            //var count = 0;
            // foreach(var word in permute("fwqxc"))
            // {
            //     count++;
            //     System.Console.WriteLine(word);
            //     var word2 = START + word + START;
            //     CalcuteShortestPathSeries(word2.ToCharArray());
            // }
            // var G = FindChar('G');
            // grid[G.x, G.y] = '#';
            // CalcuteShortestPath(START, 'x');
            // grid[G.x, G.y] = 'G';
            // System.Console.WriteLine(count);
            // CalcuteShortestPathSeries("@fwqxc@".ToCharArray());

            // Bot-right: p, e, o, h  ( h + e )
            // ALL 684, requires: none, provides: pheo
            // CalcuteShortestPathSeries(START, 'p', 'h', 'e', 'o', START);
            // CalcuteShortestPathSeries(START, 'p', 'e', 'h', 'o', START);
            // CalcuteShortestPathSeries(START, 'p', 'o', 'h', 'e', START);
            // CalcuteShortestPathSeries(START, 'p', 'o', 'e', 'h', START);
            // foreach(var word in permute("peoh"))
            // {
            //     System.Console.WriteLine(word);
            //     var word2 = START + word + START;
            //     CalcuteShortestPathSeries(word2.ToCharArray());
            // }

            // Bot-left:
            // end of path: .. -> t -> z.
            //CalcuteShortestPathSeries(START, 't', 'z'); // requires: FX, provides: ugi
        }

        public void Solve2()
        {
            ModifyGrid();

            System.Console.WriteLine(SolveExact("ehop", "3", startChar: '3')); // Q3

            // Q0: dlmr            (cp)
            // Q1: cfqwx           (ghnr)
            // Q2: abgijknstuvyz   (fx)
            // Q3: ehop            (none)
            //System.Console.WriteLine(SolveExact("nrfgcqx", "e")); // Q3 + Q0
            // Q2: ng ("many", svtz) requires: CP
            // Q1: cqf (wx) requires: RHN
            // Q0: r (dlm) requires: -
            System.Console.WriteLine(CalcuteShortestPath('0', 'r'));
            System.Console.WriteLine(CalcuteShortestPath('2', 'n'));
            //System.Console.WriteLine(CalcuteShortestPathSeries('0', 'r'));
            //System.Console.WriteLine(SolveExact("r", "0", startChar: '0'));
            //System.Console.WriteLine(SolveExact("n", "2", startChar: '2'));
            System.Console.WriteLine(SolveExact("cqf", "1", startChar: '1'));
            System.Console.WriteLine(SolveExact("dlm", "r", startChar: 'r')); // last in Q0
            System.Console.WriteLine(CalcuteShortestPath('n', 'g'));
            System.Console.WriteLine(SolveExact("wx", "f", startChar: 'f')); // last in Q1
            System.Console.WriteLine(SolveExact("svtz", "g", startChar: 'g')); // last in Q2

            var R0 = CalcuteShortestPathSeries('0', 'm', 'l', 'r', 'd');
            var R1 = CalcuteShortestPathSeries('1', 'q', 'c', 'w', 'f', 'x');
            var R2 = CalcuteShortestPathSeries('2', 'n', 's', 'g', 'v', 't', 'z');
            var R3 = CalcuteShortestPathSeries('3', 'p', 'o', 'h', 'e');
            System.Console.WriteLine("Final: " + R0.ToString());
            System.Console.WriteLine("Final: " + R1.ToString());
            System.Console.WriteLine("Final: " + R2.ToString());
            System.Console.WriteLine("Final: " + R3.ToString());
            System.Console.WriteLine("TOTAL + " + (R0 + R1 + R2 + R3).ToString()); // < 2118, != 1930

            System.Console.WriteLine(SolveExact("cfqwx", "1", startChar: '1')); // Q1

            //System.Console.WriteLine(SolveExact("gsvtz", "n", startChar: 'n')); // last in Q2
            System.Console.WriteLine(SolveExact("gsvtz", "2n", startChar: '2')); // Q2

            System.Console.WriteLine(SolveExact("dlmr", "0", startChar: '0')); // Q2

            // 2: ng (g requires F)
            // f requires N
            // n -> f -> g
            // 1: x requires G (not needed anywhere)
        }

        private void ModifyGrid()
        {
            var oldStart = specials[START];

            grid[oldStart.x, oldStart.y] = WALL;
            grid[oldStart.x + 1, oldStart.y] = WALL;
            grid[oldStart.x - 1, oldStart.y] = WALL;
            grid[oldStart.x, oldStart.y + 1] = WALL;
            grid[oldStart.x, oldStart.y - 1] = WALL;

            grid[oldStart.x - 1, oldStart.y - 1] = '0';
            grid[oldStart.x + 1, oldStart.y - 1] = '1';
            grid[oldStart.x - 1, oldStart.y + 1] = '2';
            grid[oldStart.x + 1, oldStart.y + 1] = '3';

            FindAllSpecials();// find new

            Display();
        }

        private (int x, int y) FindChar(char ch)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if(grid[x,y] == ch) return (x, y);
                }
            }

            throw new Exception("Char not found: " + ch);
        }

        private void Display()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Console.Write(grid[x,y]);
                }
                System.Console.WriteLine();
            }
        }

        private void FindAllSpecials()
        {
            specials = new Dictionary<char, (int x, int y)>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var currentCh = grid[x,y];
                    if(currentCh == WALL || currentCh == EMPTY) continue;
                    specials.Add(currentCh, (x, y));
                }
            }

            //System.Console.WriteLine(string.Join(Environment.NewLine, specials.Select(kvp => kvp.Key + ": " + kvp.Value)));

            foreach(var ch in Enumerable.Range('a', specials.Count).Select(c => (char)c))
            {
                if(!specials.ContainsKey(char.ToUpper(ch))) continue;
                System.Console.WriteLine(ch + ": key " + specials[ch] + " door " + specials[char.ToUpper(ch)]);
            }
        }

        private void FindInQuarter(int q)
        {
            var sb = new StringBuilder();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if(q==0)
                    {
                        if(x > width / 2) continue;
                        if(y > width / 2) continue;
                    }
                    else if(q==1)
                    {
                        if(x < width / 2) continue;
                        if(y > width / 2) continue;
                    }
                    else if(q==2)
                    {
                        if(x > width / 2) continue;
                        if(y < width / 2) continue;
                    }
                    else if(q==3)
                    {
                        if(x < width / 2) continue;
                        if(y < width / 2) continue;
                    }
                    var ch = grid[x,y];
                    if(char.IsLower(ch))
                    {
                        sb.Append(ch);
                    }
                }
            }
            System.Console.WriteLine($"Q{q}: {sb.ToString()}");
        }

        private class Edge
        {
            public int X;
            public int Y;
            public char Door;
            public char Key;
            public Edge Parent;

            public Edge(int x, int y)
            {
                X = x;
                Y = y;
                Parent = null;
                Door = '\0';
                Key = '\0';
            }
        }

        private (int l, string keys, string doors) CalcuteShortestPath(char from, char to, bool display = false)
        {
            var visited = new bool[width, height];

            var start = specials[from];

            visited[start.x,start.y] = true;

            Queue<Edge> queue = new Queue<Edge>();
            queue.Enqueue(new Edge(start.x, start.y));

            Edge currentEdge = null;
            while(queue.Count > 0)
            {
                currentEdge = queue.Dequeue();

                if(grid[currentEdge.X, currentEdge.Y] == to)
                {
                    if(char.IsUpper(to)) currentEdge.Door = to;
                    if(char.IsLower(to)) currentEdge.Key = to;
                    break;
                }

                var edges = new Edge[4] {
                    new Edge(currentEdge.X + 1, currentEdge.Y), 
                    new Edge(currentEdge.X - 1, currentEdge.Y), 
                    new Edge(currentEdge.X    , currentEdge.Y + 1), 
                    new Edge(currentEdge.X    , currentEdge.Y - 1)
                };

                foreach(var edge in edges)
                {
                    var pos = grid[edge.X, edge.Y];
                    if(pos != WALL && !visited[edge.X, edge.Y])
                    {
                        if(char.IsUpper(pos)) edge.Door = pos;
                        if(char.IsLower(pos)) edge.Key = pos;
                        visited[edge.X, edge.Y] = true;
                        queue.Enqueue(edge);
                        edge.Parent = currentEdge;
                    }
                }
            }

            // count
            var pathLength = 0;
            StringBuilder doors = new StringBuilder();
            StringBuilder keys = new StringBuilder();
            if(currentEdge != null)
            {
                while(currentEdge.Parent != null)
                {
                    if(grid[currentEdge.X, currentEdge.Y] == EMPTY)
                    {
                        if(display)
                        {
                            grid[currentEdge.X, currentEdge.Y] = ' ';
                        }
                    }
                    if(currentEdge.Door != '\0')
                    {
                        // wrong order :(
                        //if(keys.ToString().IndexOf(char.ToLower(currentEdge.Door)) < 0)
                        {
                            doors.Append(currentEdge.Door);
                        }
                    }
                    if(currentEdge.Key != '\0')
                    {
                        keys.Append(currentEdge.Key);
                    }
                    currentEdge = currentEdge.Parent;
                    pathLength++;
                }
            }

            var reverseDoors = new string(doors.ToString().Reverse().ToArray());
            var reverseKeys = new string(keys.ToString().Reverse().ToArray());

            //System.Console.WriteLine(from +"-" + to + " length " + pathLength.ToString("D3") + " door: " + string.Format("{0,-10}", reverseDoors) + " keys: " + string.Format("{0,-10}", reverseKeys));

            return (pathLength, reverseKeys, reverseDoors);
        }

        private int CalcuteShortestPathSeries(params char[] chs)
        {
            int sum = 0;
            var previous = chs[0];
            for(int i = 1; i < chs.Length; i++)
            {
                var currentCh = chs[i];

                (var length, _, _) = CalcuteShortestPath(previous, currentCh);
                sum += length;
                previous = currentCh;
            }

            //System.Console.WriteLine($"Path length {sum}: {string.Join('-', chs)}");

            return sum;
        }

        private string[] Combination(params char[] chars)
        {
            var query = from a in chars
                        from b in chars
                        from c in chars
                        where a != b && a != c && b != c
                        select "" + a + b + c;
            
            foreach (var item in query)
            {
                Console.WriteLine(item);
            }

            return query.ToArray();
        }

        static public IEnumerable<string> permute(string word)
        {
            if (word.Length > 1)
            {

                char character = word[0];
                foreach (string subPermute in permute(word.Substring(1)))
                {

                    for (int index = 0; index <= subPermute.Length; index++)
                    {
                        string pre = subPermute.Substring(0, index);
                        string post = subPermute.Substring(index);

                        if (post.Contains(character))
                                continue;                       

                        yield return pre + character + post;
                    }

                }
            }
            else
            {
                yield return word;
            }
        }

        public IEnumerable<string> AddConstrainsts(IEnumerable<string> stream, string doors, char target)
        {
            IEnumerable<string> output = stream;
            foreach(var key in doors.ToLower())
            {
                output = output.Where(x => x.IndexOf(key) <= x.IndexOf(target));
            }
            return output;
        }

        private (int length, string path) SolveExact(string chars, string start = "", string end = "", char startChar = START)
        {
            var permutations = permute(chars);

            //System.Console.WriteLine(permutations.Count());

            //System.Console.WriteLine("Fact: " + Fact(chars.Length));

            if(Fact(chars.Length) > 10000000) throw new Exception("Impossible to solve");

            var improved = permutations;
            //foreach(var ch in Enumerable.Range('a', chars.Count()).Select(c => (char)c))
            foreach(var ch in chars)
            {
                var (l, keys, doors) = CalcuteShortestPath(startChar, ch);
                improved = AddConstrainsts(improved, doors, ch);
            }
            //System.Console.WriteLine("Added constraints");
            //System.Console.WriteLine("Auto: " + improved.Count());

            int min = int.MaxValue;
            string best = "";
            foreach(var word in improved)
            {
                //System.Console.WriteLine(word);
                var word2 = start + word + end;
                var length = CalcuteShortestPathSeries(word2.ToCharArray());
                if(length < min)
                {
                    min = length;
                    best = word2;
                }
            }
            return (min, best);
        }

        public int SolveAllCombinations()
        {
            var chars = new string(specials.Keys.Where(x => char.IsLower(x)).ToArray());
            
            var (best, min) = SolveExact(chars, START.ToString());
            System.Console.WriteLine($"Best {best} length {min}");

            return 0;
        }

        public static int Fact(int x)
        {
            if(x > 1) return x * Fact(x-1);
            return x;
        }
    }
}