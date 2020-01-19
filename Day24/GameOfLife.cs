using System;
using System.Collections.Generic;
using System.Linq;

namespace Day24
{
    public class GameOfLife
    {
        private const int SIZE = 5;
        private const char BUG = '#';
        private const char EMPTY = '.';
        private bool[,] grid;
        private HashSet<int> bioDiversity;

        public GameOfLife(IEnumerable<string> input)
        {
            var inputEnum = input.GetEnumerator();
            grid = new bool[SIZE, SIZE];
            bioDiversity = new HashSet<int> ();

            for (int i = 0; i < SIZE; i++)
            {
                inputEnum.MoveNext();
                var line = inputEnum.Current;
                for (int j = 0; j < SIZE; j++)
                {   
                    grid[j, i] = line[j] == BUG;
                }
            }

            var bio = GridToBio(grid);
            bioDiversity.Add(bio);

            Display();
        }

        public void CalcSame()
        {
            while(Step())
            {

            }
            var bio = GridToBio(grid);
            System.Console.WriteLine($"Repeated for bio {bio}");
            Display();
        }

//Each minute, The bugs live and die based on the number of bugs in the four adjacent tiles:

// A bug dies (becoming an empty space) unless there is exactly one bug adjacent to it.
// An empty space becomes infested with a bug if exactly one or two bugs are adjacent to it.

        private bool Step()
        {
            var newGrid = new bool[SIZE, SIZE];
            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    newGrid[x, y] = LiveAndDie(x, y);
                }
            }

            grid = newGrid;
            var bio = GridToBio(newGrid);
            if(!bioDiversity.Contains(bio))
            {
                bioDiversity.Add(bio);
                return true;
            }
            else
            {
                Display();
                return false;
            }
        }

        private bool LiveAndDie(int x, int y)
        {
            if(grid[x, y])
            {
                return Adjecent(x, y) == 1;
            }
            else
            {
                var adjecent = Adjecent(x, y);
                if(adjecent == 1 || adjecent == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private int Adjecent(int x, int y)
        {
            int count = 0;
            if(x > 0 && grid[x - 1, y]) count ++;
            if(x < SIZE - 1 && grid[x + 1, y]) count ++;
            if(y > 0 && grid[x, y - 1]) count ++;
            if(y < SIZE - 1 && grid[x, y + 1]) count ++;
            return count;
        } 

        private void Display()
        {
            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {   
                    Console.Write(grid[x, y] ? BUG : EMPTY);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static int GridToBio(bool[,] array)
        {
            int sum = 0;
            int count = 0;
            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {   
                    var val = array[x, y];
                    if(val)
                    {
                        sum += 1 << count;
                    }

                    count++;
                }
            }
            return sum;
        }
    }
}