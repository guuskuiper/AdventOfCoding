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

            // deck.DealWithIncrement(3);
            // deck.Display();

            // var undo = deck.InverseDealWithIncrement2(3);
            // System.Console.WriteLine(string.Join(' ', undo));

            //deck.PrintCache();

            deck.Display();
            deck.DealIntoStack();
            deck.CutN(4);
            deck.DealWithIncrement(3);
            deck.Display();
            for(int i = 0; i < 10; i++)
            {
                var res = deck.InverseSingleDealWith(3, i, 10);
                var res2 = deck.InverseSingleCut(4, res, 10);
                var res3 = deck.InverseSingleDealIntoStack(res2, 10);
                System.Console.WriteLine($"{i} = {res3}");
                //System.Console.WriteLine(deck.InverseSingleDealIntoStack(i, 10));
            }

            var part1 = deck.Inverse(8502, 10007, 1, text);
            System.Console.WriteLine($"Part1: 8502 = {part1} (should be 2019)");
            
            System.Console.WriteLine($"2020: {deck.Inverse(2020, 119315717514047, 1, text)}");
            System.Console.WriteLine($"2021: {deck.Inverse(2021, 119315717514047, 1, text)}");
            //var part2 = deck.Inverse(2020, 119315717514047, 5000, text);
            var part2 = deck.Inverse(2020, 119315717514047, 101741582076661, text);
            System.Console.WriteLine($"Part2: 2020 = {part2}");


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
