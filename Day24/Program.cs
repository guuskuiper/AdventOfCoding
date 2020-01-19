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
        }
    }
}
