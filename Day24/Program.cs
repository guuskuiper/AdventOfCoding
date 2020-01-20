using System;
using System.IO;

namespace Day24
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllLines("input.txt");
            //var text = File.ReadAllLines("example.txt");

            var gameOfLife = new GameOfLife(text);
            gameOfLife.CalcSame();

            var example = File.ReadAllLines("example.txt");
            var gameOfLifeExample = new GameOfLife(example);
            gameOfLifeExample.Steps(10);

            var gameOfLife2 = new GameOfLife(text);
            gameOfLife2.Steps(200);
        }
    }
}
