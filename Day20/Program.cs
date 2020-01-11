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
        }
    }
}
