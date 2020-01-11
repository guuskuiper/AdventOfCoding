using System;
using System.IO;

namespace Day20
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllLines("input.txt");
            var donutMaze = new DonutMaze(text);
            donutMaze.Solve();

            // Part2: recurvice space
            // at level 0 only AA and ZZ on outside
            // inside portal teleport to level++
            // at level > 0 AA and ZZ are a WALL
            // at level > 0 outside portal teleport to level--

            donutMaze.Solve2();
        }
    }
}
