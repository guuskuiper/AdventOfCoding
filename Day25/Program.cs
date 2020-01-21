using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;

namespace Day25
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            var instructions = text.Split(',').Select(long.Parse).ToArray();

            var sb = new StringBuilder();
                                    // -> ( 0, 0) Start Hull Break NW
            sb.AppendLine("north"); // -> (-1, 0) Observatory NES
            sb.AppendLine("take dark matter");
            sb.AppendLine("north"); // -> (-2, 0) Gift Wrapping Center NES
            sb.AppendLine("north"); // -> (-3, 0) Warp Drive Maintenance ESW
            sb.AppendLine("take manifold");
            sb.AppendLine("east");  // -> (-3, 1) Sick Bay NW
            sb.AppendLine("take candy cane");
            sb.AppendLine("north"); // -> (-4, 1) Kitchen (dont pick 'molten lava') S
            sb.AppendLine("south"); // -> (-3, 1) Sick Bay NW
            sb.AppendLine("west");  // -> (-3, 0) Warp Drive Maintenance ESW
            sb.AppendLine("west");  // -> (-3,-1) Passages E
            sb.AppendLine("take jam");
            sb.AppendLine("east");  // -> (-3, 0) Warp Drive Maintenance ESW
            sb.AppendLine("south"); // -> (-2, 0) Gift Wrapping Center NES
            sb.AppendLine("east");  // -> (-2, 1) Engineering (dont pick 'photons') SW
            sb.AppendLine("south"); // -> (-1, 1) Hallway NSW
            sb.AppendLine("take antenna");
            sb.AppendLine("west");  // -> (-1, 0) Storage??
            sb.AppendLine("take hypercube");
            sb.AppendLine("north"); // -> ( 0, 0) Holodeck S
            sb.AppendLine("south"); // -> (-1, 0) Storage??
            sb.AppendLine("east");  // -> (-1, 1) Hallway NSW
            sb.AppendLine("south"); // -> (  ,  ) Bavigation (dont pick 'gaint electromagnet')
            sb.AppendLine("north"); // -> 
            sb.AppendLine("north"); // ->
            sb.AppendLine("west");  // ->
            sb.AppendLine("south"); // -> 
            sb.AppendLine("east");  // -> ( 1, 1) Crew Quarters ESW
            sb.AppendLine("east");  // -> ( 1, 2) Hol Chocolate Fountain
            sb.AppendLine("take bowl of rice");
            sb.AppendLine("west");  // -> ( 1, 1) Crew Quarters ESW
            sb.AppendLine("south"); // -> ( 2, 1) Corridor
            sb.AppendLine("take dehydrated water");
            sb.AppendLine("east");  // -> ( 2, 2) Science Lab
            //sb.AppendLine("take infinite loop");
            sb.AppendLine("north"); // ->
            sb.AppendLine("west");  // ->
            sb.AppendLine("north"); // ->
            sb.AppendLine("west");  // ->
            sb.AppendLine("south"); // -> START!!!
            sb.AppendLine("west");  // -> Stables ES
            //sb.AppendLine("take escape pod");
            sb.AppendLine("south"); // -> Arcade NW
            sb.AppendLine("west");  // -> Security Checkpoint EW
            sb.AppendLine("west");  // -> Pressure-Sensitive Floor -> Back to "Security Checkpoint"
            // 8 items 2 ^ 8 = 256 options
            sb.AppendLine("inv");
            // sb.AppendLine("drop manifold");
            // sb.AppendLine("west");
            // sb.AppendLine("drop dehydrated water");
            // sb.AppendLine("west");
            // sb.AppendLine("drop candy cane");
            // sb.AppendLine("west");
            // sb.AppendLine("drop dark matter");
            // sb.AppendLine("west");
            // sb.AppendLine("drop jam");
            // sb.AppendLine("west");
            // sb.AppendLine("drop bowl of rice");
            // sb.AppendLine("west");
            // sb.AppendLine("drop antenna");
            // sb.AppendLine("west");
            // sb.AppendLine("drop hypercube"); // heavy
            // sb.AppendLine("west");

            var ascii = new ASCII(instructions);
            ascii.Start(sb.ToString());

            //System.Console.WriteLine("8 items -> options: " + Math.Pow(2, 8));
        }
    }
}
