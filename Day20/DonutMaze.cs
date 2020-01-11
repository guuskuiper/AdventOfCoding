using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day20
{
    public class DonutMaze
    {
        private const char WALL = '#';
        private const char EMPTY = '.';
        private const char START = '@';

        private char[,] grid;
        private int width;
        private int height;

        private Dictionary<string, (int x, int y)> outsidePortals;
        private Dictionary<string, (int x, int y)> insidePortals;

        public DonutMaze(string[] input)
        {
            width = input[0].Length;
            height = input.Length;

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
            FindAllSpecials();
        }

        public void Solve()
        {
            var pathLength = CalcuteShortestPath();
            Display();
            System.Console.WriteLine($"Path length: {pathLength}");
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
            // find letter with EMPTY next to it, opposite side is the other letter
            outsidePortals = new Dictionary<string, (int x, int y)>();
            insidePortals = new Dictionary<string, (int x, int y)>();
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    var currentCh = grid[x,y];
                    if(!char.IsLetter(currentCh)) continue;
                    var topCh = grid[x,y - 1];
                    var botCh = grid[x,y + 1];
                    var leftCh = grid[x - 1,y];
                    var rightCh = grid[x + 1,y];
                    if(botCh == EMPTY)
                    {
                        Add(new string(topCh.ToString() + currentCh.ToString()), (x, y + 1));
                    }
                    else if(topCh == EMPTY)
                    {
                        Add( new string(currentCh.ToString() + botCh.ToString()), (x, y - 1));
                    }
                    else if(leftCh == EMPTY)
                    {
                        Add( new string(currentCh.ToString() + rightCh.ToString()), (x - 1, y));
                    }
                    else if(rightCh == EMPTY)
                    {
                        Add( new string(leftCh.ToString() + currentCh.ToString()), (x + 1, y));
                    }
                }
            }

            foreach(var kvp in insidePortals)
            {
                var outside = outsidePortals[kvp.Key];
                System.Console.WriteLine($"Portal inside: {kvp.Key}@{kvp.Value} <-> {outside}");
            }
            System.Console.WriteLine($"Start: AA@{outsidePortals["AA"]}");
            System.Console.WriteLine($"End: ZZ@{outsidePortals["ZZ"]}");

            void Add(string portal, (int x, int y) location)
            {
                if(IsOutide(location.x, location.y))
                {
                    if(outsidePortals.ContainsKey(portal))
                    {
                        System.Console.WriteLine("DupO: " + portal);
                        return;
                    }
                    outsidePortals.Add(portal, location);
                }
                else
                {
                    if(insidePortals.ContainsKey(portal))
                    {
                        System.Console.WriteLine("DupI: " + portal);
                        return;
                    }
                    insidePortals.Add(portal, location);
                }
            }
        }

        private bool IsOutide(int x, int y)
        {
            if(x < 3 || y < 3) return true;
            if(x > width - 4 || y > height - 4) return true;
            return false;
        }

        private class Edge
        {
            public int X;
            public int Y;
            public Edge Parent;

            public Edge(int x, int y)
            {
                X = x;
                Y = y;
                Parent = null;
            }
        }

        private class EdgeLevel : Edge
        {
            public int L;

            public EdgeLevel(int x, int y, int lvl) : base(x, y)
            {
                L = lvl;
            }
        }

        private int CalcuteShortestPath()
        {
            var visited = new bool[width, height];
            var start = outsidePortals["AA"];
            var target = outsidePortals["ZZ"];
            visited[start.x,start.y] = true;
            visited[start.x,start.y + 1] = true; // AA portal

            Queue<Edge> queue = new Queue<Edge>();
            queue.Enqueue(new Edge(start.x, start.y));

            Edge currentEdge = null;
            while(queue.Count > 0)
            {
                currentEdge = queue.Dequeue();

                if(currentEdge.X == target.x && currentEdge.Y == target.y)
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
                    var ch = grid[edge.X, edge.Y];

                    // change the location for a portal
                    if(char.IsUpper(ch) && !visited[edge.X, edge.Y])
                    {
                        visited[edge.X, edge.Y] = true;
                        var dx = edge.X - currentEdge.X;
                        var dy = edge.Y - currentEdge.Y;
                        var ch2 = grid[edge.X + dx, edge.Y + dy];

                        // protal
                        string portal = dx > 0 || dy > 0 ? ch.ToString() + ch2.ToString() : ch2.ToString() + ch.ToString();

                        (int x, int y) portalTo;
                        if(IsOutide(edge.X, edge.Y))
                        {
                            // outside -> inside
                            portalTo = insidePortals[portal];
                        }
                        else
                        {
                            // inside -> outside
                            portalTo = outsidePortals[portal];
                        }
                        edge.X = portalTo.x;
                        edge.Y = portalTo.y;
                    }

                    if(ch != WALL && !visited[edge.X, edge.Y])
                    {
                        visited[edge.X, edge.Y] = true;
                        queue.Enqueue(edge);
                        edge.Parent = currentEdge;

                    }
                }
            }

            // count
            var pathLength = 0;
            if(currentEdge != null)
            {
                while(currentEdge.Parent != null)
                {
                    if(grid[currentEdge.X, currentEdge.Y] == EMPTY)
                    {
                        grid[currentEdge.X, currentEdge.Y] = ' ';
                    }
                    currentEdge = currentEdge.Parent;
                    pathLength++;
                }
            }

            return pathLength;
        }

        public void Solve2()
        {
            var pathLength = CalcuteShortestPathLevels();
            //Display();
            System.Console.WriteLine($"Path length: {pathLength}");
        }

        private int CalcuteShortestPathLevels()
        {
            var visited = new bool[width, height, 50];
            var start = outsidePortals["AA"];
            var target = outsidePortals["ZZ"];
            visited[start.x,start.y, 0] = true;
            visited[start.x,start.y + 1, 0] = true; // AA portal

            Queue<EdgeLevel> queue = new Queue<EdgeLevel>();
            queue.Enqueue(new EdgeLevel(start.x, start.y, 0));

            EdgeLevel currentEdge = null;
            while(queue.Count > 0)
            {
                currentEdge = queue.Dequeue();

                if(currentEdge.X == target.x && currentEdge.Y == target.y && currentEdge.L == 0)
                {
                    break;
                }

                var edges = new EdgeLevel[4] {
                    new EdgeLevel(currentEdge.X + 1, currentEdge.Y, currentEdge.L), 
                    new EdgeLevel(currentEdge.X - 1, currentEdge.Y, currentEdge.L), 
                    new EdgeLevel(currentEdge.X    , currentEdge.Y + 1, currentEdge.L), 
                    new EdgeLevel(currentEdge.X    , currentEdge.Y - 1, currentEdge.L)
                };

                foreach(var edge in edges)
                {
                    var ch = grid[edge.X, edge.Y];

                    if(edge.L > 0)
                    {
                        // skip AA and ZZ
                        if(edge.X == start.x && edge.Y == start.y) continue;
                        if(edge.X == target.x && edge.Y == target.y) continue;
                    }

                    // change the location for a portal
                    if(char.IsUpper(ch) && !visited[edge.X, edge.Y, edge.L])
                    {
                        visited[edge.X, edge.Y, edge.L] = true;
                        var dx = edge.X - currentEdge.X;
                        var dy = edge.Y - currentEdge.Y;
                        var ch2 = grid[edge.X + dx, edge.Y + dy];

                        // protal
                        string portal = dx > 0 || dy > 0 ? ch.ToString() + ch2.ToString() : ch2.ToString() + ch.ToString();

                        (int x, int y) portalTo;
                        if(IsOutide(edge.X, edge.Y))
                        {
                            if(currentEdge.L == 0) continue;
                            // outside -> inside
                            portalTo = insidePortals[portal];
                            edge.L--;
                        }
                        else
                        {
                            // inside -> outside
                            if(currentEdge.L > visited.GetLength(2) - 2) continue; // assume this is the max required dimention
                            portalTo = outsidePortals[portal];
                            edge.L++;
                        }
                        edge.X = portalTo.x;
                        edge.Y = portalTo.y;
                    }

                    if(ch != WALL && !visited[edge.X, edge.Y, edge.L])
                    {
                        visited[edge.X, edge.Y, edge.L] = true;
                        queue.Enqueue(edge);
                        edge.Parent = currentEdge;
                    }
                }
            }

            // count
            var pathLength = 0;
            if(currentEdge != null)
            {
                while(currentEdge.Parent != null)
                {
                    if(grid[currentEdge.X, currentEdge.Y] == EMPTY)
                    {
                        grid[currentEdge.X, currentEdge.Y] = ' ';
                    }
                    currentEdge = (EdgeLevel)currentEdge.Parent;
                    pathLength++;
                }
            }

            return pathLength;
        }
    }
}