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

            foreach(var ch in Enumerable.Range('a', 'z' - 'a' + 1).Select(c => (char)c))
            {
                CalcuteShortestPath(START, ch);
            }
            CalcuteShortestPath(START, 'n', true);
            //CalcuteShortestPath('h', 'p', true);

            Display();

            // foreach(var q in Enumerable.Range(0, 4))
            // {
            //     FindInQuarter(q);
            // }
            // Q0: dlmr
            // Q1: fwqxc
            // Q2: ubzytsjkavgni
            // Q3: poeh

            // Top-left: d, l, m, r

            // Top-right: f, w, q, x, c
            // requires: RHN

            // Bot-right: p, e, o, h  ( h + e )
            // ALL 684, requires: none, provides: pheo
            // CalcuteShortestPathSeries(START, 'p', 'h', 'e', 'o', START);
            // CalcuteShortestPathSeries(START, 'p', 'e', 'h', 'o', START);
            // CalcuteShortestPathSeries(START, 'p', 'o', 'h', 'e', START);
            // CalcuteShortestPathSeries(START, 'p', 'o', 'e', 'h', START);

            // Bot-left:
            // end of path: .. -> t -> z.
            //CalcuteShortestPathSeries(START, 't', 'z'); // requires: FX, provides: ugi
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

            foreach(var ch in Enumerable.Range('a', 'z' - 'a' + 1).Select(c => (char)c))
            {
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

        private int CalcuteShortestPath(char from, char to, bool display = false)
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
                    currentEdge = currentEdge.Parent;
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
                    pathLength++;
                }
            }

            var reverseDoors = new string(doors.ToString().Reverse().ToArray());
            var reverseKeys = new string(keys.ToString().Reverse().ToArray());

            System.Console.WriteLine(from +"-" + to + " length " + pathLength.ToString("D3") + " door: " + string.Format("{0,-10}", reverseDoors) + " keys: " + string.Format("{0,-10}", reverseKeys));

            return pathLength;
        }

        private int CalcuteShortestPathSeries(params char[] chs)
        {
            int sum = 0;
            var previous = chs[0];
            for(int i = 1; i < chs.Length; i++)
            {
                var currentCh = chs[i];

                sum += CalcuteShortestPath(previous, currentCh);

                previous = currentCh;
            }

            System.Console.WriteLine($"Path length {sum}: {string.Join('-', chs)}");

            return sum;
        }
    }
}