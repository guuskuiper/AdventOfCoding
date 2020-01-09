using System;
using System.IO;

namespace Day18
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllLines("input.txt");

            var maze = new Maze(text);
        }
    }
}
