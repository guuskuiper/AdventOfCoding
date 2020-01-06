using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Day5;

namespace Day15
{
    class Robot
    {
        private const int WIDTH = 45;
        private const int HEIGHT = 55;

        public enum Movement
        {
            North = 1, 
            South = 2, 
            West = 3, 
            East = 4
        }

        public enum Location
        {
            Unknown = 0,
            Wall = 1,
            Target = 2,
            Empty = 3,
            Start = 4,
            ShortestPath = 5,
        }

        private ElfComputer computer;
        private int curX;
        private int curY;
        private int targetX;
        private int targetY;

        readonly int startX;
        readonly int startY;

        private Location[,] grid;
        private Bitmap bitmap;
        private Movement currentMove;
        private bool foundWall;
        private bool [,] visited;
        

        public Robot(IEnumerable<long> instructions)
        {
            computer = new ElfComputer(instructions, Input, Output);

            grid = new Location[WIDTH, HEIGHT];
            bitmap = new Bitmap(WIDTH, HEIGHT);

            // start in the middle?
            startX = WIDTH / 2 + 1;
            startY = HEIGHT / 2 + 1;
            curX = startX;
            curY = startY;
            currentMove = Movement.North;

            targetX = startX;
            targetY = startY;
            grid[startX, startY] = Location.Start;
        }

        public int StartRobot()
        {
            computer.ProcessInstructions();

            return 0;
        }

        public long Input()
        {
            var previousMove = currentMove;
            // previous hit a wall?

            if(!foundWall)
            {
                currentMove = previousMove;
                UpdateTarget(currentMove);
            }
            else if(grid[targetX, targetY] != Location.Wall)
            {
                // no wall found from the previous target
                // look for a wall by turning left
                //currentMove = TurnLeft(previousMove);
                currentMove = TurnLeft(previousMove);
                UpdateTarget(currentMove);

                if(grid[targetX, targetY] == Location.Wall)
                {
                    // dont turn towards a wall
                    currentMove = previousMove;
                    UpdateTarget(currentMove);
                }
            }
            else
            {
                // avoid wall by turning right
                currentMove = TurnRight(previousMove);
                UpdateTarget(currentMove);
            }

            if(targetX == startX && targetY == startY)
            {
                var dist = CalcuteShortestPath();
                System.Console.WriteLine("Length: " + dist);
                Display();
                System.Console.WriteLine("Length: " + dist);
                throw new Exception("Done");
            }

            return (long)currentMove;
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

        private int CalcuteShortestPath()
        {
            visited = new bool[WIDTH, HEIGHT];

            visited[startX,startY] = true;

            Queue<Edge> queue = new Queue<Edge>();
            queue.Enqueue(new Edge(startX, startY));

            Edge currentEdge = null;
            while(queue.Count > 0)
            {
                currentEdge = queue.Dequeue();

                if(grid[currentEdge.X, currentEdge.Y] == Location.Target)
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
                    if(grid[edge.X, edge.Y] != Location.Wall && !visited[edge.X, edge.Y])
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
                    grid[currentEdge.X, currentEdge.Y] = Location.ShortestPath;
                    currentEdge = currentEdge.Parent;
                    pathLength++;
                }
            }

            return pathLength;
        }

// 1      procedure BFS(G, start_v) is
// 2      let Q be a queue
// 3      label start_v as discovered
// 4      Q.enqueue(start_v)
// 5      while Q is not empty do
// 6          v := Q.dequeue()
// 7          if v is the goal then
// 8              return v
// 9          for all edges from v to w in G.adjacentEdges(v) do
// 10             if w is not labeled as discovered then
// 11                 label w as discovered
// 12                 w.parent := v
// 13                 Q.enqueue(w) 

        private Movement TurnRight(Movement move)
        {
            switch(move)
            {
                case Movement.North:
                    return Movement.East;
                case Movement.East:
                    return Movement.South;
                case Movement.South:
                    return Movement.West;
                case Movement.West:
                    return Movement.North;
                default:
                    throw new Exception("Unknow movement");
            }
        }

        private Movement TurnLeft(Movement move)
        {
            switch(move)
            {
                case Movement.North:
                    return Movement.West;
                case Movement.East:
                    return Movement.North;
                case Movement.South:
                    return Movement.East;
                case Movement.West:
                    return Movement.South;
                default:
                    throw new Exception("Unknow movement");
            }
        }

        private void UpdateTarget(Movement move)
        {
            switch(move)
            {
                case Movement.North:
                    targetX = curX;
                    targetY = curY+1;
                    break;
                case Movement.East:
                    targetX = curX+1;
                    targetY = curY;
                    break;
                case Movement.South:
                    targetX = curX;
                    targetY = curY-1;
                    break;
                case Movement.West:
                    targetX = curX-1;
                    targetY = curY;
                    break;
                default:
                    throw new Exception("Unknow movement");
            }
        }

        public void Output(long output)
        {
            switch(output)
            {
                case 0:
                    grid[targetX, targetY] = Location.Wall;
                    System.Console.WriteLine($"Wall at ({targetX},{targetY})");
                    foundWall = true;
                    break;
                case 1:
                    grid[targetX, targetY] = Location.Empty;
                    break;
                case 2:
                    grid[targetX, targetY] = Location.Target;
                    // Display();
                    // throw new Exception("Done");
                    break;
                default:
                    throw new Exception("Unknow output");
            }

            if(output != 0)
            {
                curX = targetX;
                curY = targetY;
            }
        }

        private void Display()
        {
            Console.Clear();
            for(int y = 0; y < HEIGHT; y++)
            {
                Console.SetCursorPosition (0, y);
                for(int x = 0; x < WIDTH; x++)
                {
                    Console.Write(Draw(grid[x,y]));
                }
            }
            Console.SetCursorPosition(0, HEIGHT);
        }

        private char Draw(Location location)
        {
            switch(location)
            {
                case Location.Empty:
                    return ' ';
                case Location.Target:
                    return '2';
                case Location.Unknown:
                    return '?';
                case Location.Wall:
                    return '#';
                case Location.Start:
                    return 'S';
                case Location.ShortestPath:
                    return '.';
                default:
                    throw new Exception();
            }
        }
    }
}