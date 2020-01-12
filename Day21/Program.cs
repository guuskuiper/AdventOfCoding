using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Day21
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            var instructions = text.Split(',').Select(long.Parse).ToArray();

            var sprintDroid = new SpringDroid(instructions);

            // Max 15 instruction
            // AND X Y ( Y = X & Y )
            // OR X Y ( Y = X | Y)
            // NOT X Y (Y = !X)
            // ground = true; hole = false
            // registers:
            // A - 1 distance
            // B - 2
            // C - 3
            // D - 4
            // T temp
            // J jump at end

            // check if destination is safe: D == TRUE
            // check if jump is required: A FALSE | B FALSE | C FALSE

            // WALK start the SpringDroid

            //EXAMPLE
            // NOT A J // jump if hole in 1
            // NOT B T // temp if hole in 2
            // AND T J // jump if hole in 1 AND 2 => J = !A & !B
            // NOT C T // temp = !C => T = !C
            // AND T J // jump if hole in 1 AND 2 AND 3 => J = !A & !B & !C
            // AND D J // J = !A & !B & !C & D


            //#####...#########
            //     ABCD
            //     FFFT
            // D & (!A | ! B  | ! C)
            //#####..#.########
            //    ABCD
            var sb = new StringBuilder();
            sb.AppendLine("NOT A J"); // jump if hole in 1
            sb.AppendLine("NOT B T"); // jump if hole in 2
            sb.AppendLine("OR T J"); // !A | !B
            sb.AppendLine("NOT C T"); // jump if hole in 3
            sb.AppendLine("OR T J"); // !A | !B | !C
            sb.AppendLine("AND D J"); // jump to ground J = D & J
            sb.AppendLine("WALK"); // main

            sprintDroid.Start(sb.ToString());


            // PART2:
            // ABCDEFGHI

            //    J   J   J     
            //#####.#.#...#.###
            //  J   J 



            var sprintDroid2 = new SpringDroid(instructions);
            var sb2 = new StringBuilder();
            sb2.AppendLine("NOT A J"); // jump if hole in 1
            sb2.AppendLine("NOT B T"); // jump if hole in 2
            sb2.AppendLine("OR T J"); // !A | !B
            sb2.AppendLine("NOT C T"); // jump if hole in 3
            sb2.AppendLine("OR T J"); // !A | !B | !C
            sb2.AppendLine("AND D J"); // jump to ground J = D & J
            sb2.AppendLine("RUN"); // main

            sprintDroid2.Start(sb2.ToString());
        }
    }
}
