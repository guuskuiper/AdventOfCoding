using System;
using System.IO;
using System.Linq;

namespace Day22
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllLines("input.txt");

            var deck = new SpaceCards(10); // 10007
            //deck.Shuffle(text);
            //deck.ShuffleN(text, 1);
            //deck.Find(2019);
            deck.DealWithIncrement(3);
            deck.Display();

            var undo = deck.InverseDealWithIncrement2(3);
            System.Console.WriteLine(string.Join(' ', undo));

            //deck.PrintCache();

            // var offsets = deck.FindOffsets(64, 10007);
            // System.Console.WriteLine(string.Join(',', offsets));

            // System.Console.WriteLine(1 % 7);
            // System.Console.WriteLine(11 % 7);
            // System.Console.WriteLine(21 % 7); // 3; ( % == 0)
            // System.Console.WriteLine(31 % 7);

            // System.Console.WriteLine(2 % 7);
            // System.Console.WriteLine(12 % 7);
            // System.Console.WriteLine(22 % 7);
            // System.Console.WriteLine(32 % 7);
            // System.Console.WriteLine(42 % 7); // 6 ( % == 0)

        }
    }
}
