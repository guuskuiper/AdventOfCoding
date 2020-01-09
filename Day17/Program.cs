using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Day17
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            var instructions = text.Split(',').Select(long.Parse).ToArray();

            var ascii = new ASCII(instructions);
            ascii.Start();

            // manually splitting it up into 3 parts (A, B, C) and creating the main sequence:
            // A,B,A,C,A,B,C,B,C,B
            // A: 
            // L,10,R,8,L,6,R,6

            // B:
            // L,8,L,8,R,8

            // C:
            // R,8,L,6,L,10,L,10
            var sb = new StringBuilder();
            sb.AppendLine("A,B,A,C,A,B,C,B,C,B"); // main
            sb.AppendLine("L,10,R,8,L,6,R,6"); // A
            sb.AppendLine("L,8,L,8,R,8"); // B
            sb.AppendLine("R,8,L,6,L,10,L,10"); // C
            sb.AppendLine("y");// continues feed  y/ n

            var ascii2 = new ASCII(instructions);
            ascii2.Start2(sb.ToString());
        }
    }
}
