using System;
using System.IO;

namespace Day18
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllLines("input.txt");
            //var text = File.ReadAllLines("sample81.txt");
            //var text = File.ReadAllLines("sample132.txt");
            //var text = File.ReadAllLines("sample136.txt");

            var maze = new Maze(text);
            //maze.SolveAllCombinations();
            maze.Solve();
        }
    }
}
