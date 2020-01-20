using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Day25
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            var instructions = text.Split(',').Select(long.Parse).ToArray();

            var ascii = new ASCII(instructions);
            ascii.Start();
        }
    }
}
