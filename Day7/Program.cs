using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Day5;

namespace Day7
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            //string text = "1,9,10,3,2,3,11,0,99,30,40,50";
            //string text = "3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0";
            var instructions = text.Split(',').Select(long.Parse).ToArray();

            var sequences = GenerateSequence();
            long maxOutput = 0;
            List<int> bestSequence = null;
            foreach(var sequence in sequences)
            {
                var ampAOut = OutputAmp(instructions, sequence[0], 0);
                var ampBOut = OutputAmp(instructions, sequence[1], ampAOut);
                var ampCOut = OutputAmp(instructions, sequence[2], ampBOut);
                var ampDOut = OutputAmp(instructions, sequence[3], ampCOut);
                var ampEOut = OutputAmp(instructions, sequence[4], ampDOut);

                if(ampEOut > maxOutput)
                {
                    maxOutput = ampEOut;
                    bestSequence = sequence;
                }
            }

            System.Console.WriteLine(maxOutput);
            System.Console.WriteLine(string.Join(' ', bestSequence));

            Debug.Assert(maxOutput == 844468);

            //text = "3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5";
            //instructions = text.Split(',').Select(int.Parse).ToArray();

            var sequences2 = GenerateSequence2();
            long maxOutput2 = 0;
            List<int> bestSequence2 = null;
            //sequences2 = new List<List<int>> () { new List<int> {9, 8, 7, 6, 5}};
            foreach(var sequence in sequences2)
            {
                long [] outputs = new long[5];
                bool[] outputValid = {false, false, false, false, false}; 
                bool[] firsts = {true, true, true, true, true};

                Action<int, long> outputIdFunc = (id, x) => 
                {
                    outputs[id] = x;
                    outputValid[id] = true;
                };

                Func<int, long> inputIdFunc = (id) =>
                {
                    if(firsts[id])
                    {
                        firsts[id] = false;
                        if(id == 0)
                        {
                            //intialize ampA
                            outputIdFunc(0, 0);
                        }
                        return sequence[id];
                    }
                    while(!outputValid[id])
                    {
                        Task.Delay(1);
                        //Task.Yield();
                    }
                    outputValid[id] = false;
                    return outputs[id];
                };

                var ampA = AmpAsync(instructions, sequence[0], () => inputIdFunc(0), (x) => outputIdFunc(1, x));
                var ampB = AmpAsync(instructions, sequence[1], () => inputIdFunc(1), (x) => outputIdFunc(2, x));
                var ampC = AmpAsync(instructions, sequence[2], () => inputIdFunc(2), (x) => outputIdFunc(3, x));
                var ampD = AmpAsync(instructions, sequence[3], () => inputIdFunc(3), (x) => outputIdFunc(4, x));
                var ampE = AmpAsync(instructions, sequence[4], () => inputIdFunc(4), (x) => outputIdFunc(0, x));

                Task.WaitAll(ampA, ampB, ampC, ampD, ampE);

                var output = outputs[0];
                if(output > maxOutput2)
                {
                    maxOutput2 = output;
                    bestSequence2 = sequence;
                }
            }

            System.Console.WriteLine(maxOutput2);
            System.Console.WriteLine(string.Join(' ', bestSequence2));

            Debug.Assert(maxOutput2 == 4215746);
        }

        static int Input()
        {
            return 5;
            //return int.Parse(Console.ReadLine());
        }

        static void Output(int output)
        {
            System.Console.WriteLine("Output: " + output);
        }

        static long OutputAmp(IEnumerable<long> instructions, int phase, long input)
        {
            long output = -1;

            long currentInput = phase;

            Func<long> inputFunc = () => 
            {
                long inp = currentInput;
                currentInput = input;
                return inp;
            };

            var amp = new ElfComputer(instructions, inputFunc, (x) => output = x);
            amp.ProcessInstructions();
            return output;
        }

        static List<List<int>> GenerateSequence()
        {
            int[] item = {4, 3, 2, 1, 0};

            var items = new List<List<int>> ();

            for(int i = 01234; i <= 43210; i++)
            {
                var iStr = i.ToString("D5");
                var ints = iStr.Select(x => int.Parse(x.ToString())).ToList();

                bool invalid = false;
                foreach(var digit in ints)
                {
                    if(digit > 4)
                    {
                        invalid = true;
                        break;
                    }
                }
                if(invalid) continue;

                if(ints.Distinct().Count() == 5)
                {
                    items.Add(ints.ToList());
                }
            }

            return items;
            //yield return item;
        }

        
        static async Task<long> AmpAsync(IEnumerable<long> instructions, int phase, Func<long> inputFunc2, Action<long> outputFunc2)
        {
            return await Task.Run( 
            () => 
                {
                    var amp = new ElfComputer(instructions, inputFunc2, outputFunc2);
                    amp.ProcessInstructions();
                    return 0;
                }
            );
        }

        static List<List<int>> GenerateSequence2()
        {
            int[] item = {4, 3, 2, 1, 0};

            var items = new List<List<int>> ();

            for(int i = 0; i <= 98765; i++)
            {
                var iStr = i.ToString("D5");
                var ints = iStr.Select(x => int.Parse(x.ToString())).ToList();

                bool invalid = false;
                foreach(var digit in ints)
                {
                    if(digit < 5)
                    {
                        invalid = true;
                        break;
                    }
                }
                if(invalid) continue;

                if(ints.Distinct().Count() == 5)
                {
                    items.Add(ints.ToList());
                }
            }

            return items;
            //yield return item;
        }
    }
}
