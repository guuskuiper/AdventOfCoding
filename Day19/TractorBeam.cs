using System;
using System.Collections.Generic;
using System.Linq;
using Day5;

namespace Day19
{
    public class TractorBeam
    {
        private const int WIDTH = 50;
        private const int HEIGHT = 50;
        private ElfComputer computer;
        private bool[,] grid;

        private Queue<int> inputSequence;

        private int outputCount;

        private List<long> inputCopy;

        public TractorBeam(IEnumerable<long> input)
        {
            inputCopy = new List<long> (input);

            //computer = new ElfComputer(input, Input, Output);

            grid = new bool[WIDTH, HEIGHT];

            inputSequence = new Queue<int> ();

            for(int x = 0; x < WIDTH; x++)
            {
                for(int y = 0; y < HEIGHT; y++)
                {
                    inputSequence.Enqueue(x);
                    inputSequence.Enqueue(y);
                }
            }
        }

        public void Start()
        {
            do
            {
                var pc = new ElfComputer(new List<long>(inputCopy), Input, Output);
                pc.ProcessInstructions();
            }
            while( inputSequence.Count > 0 );

            Display();
        }

        private void Display()
        {
            int count = 0;
            for(int y = 0; y < HEIGHT; y++)
            {
                for(int x = 0; x < WIDTH; x++)
                {
                    if(!grid[x,y]) count++;
                    System.Console.Write(grid[x,y] ? '.' : '#');
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine("Count " + count);
        }

        private long Input()
        {
            if(inputSequence.Count() > 0)
            {
                return inputSequence.Dequeue();
            }
            else
            {
                return -1;
            }
        }
        private void Output(long output)
        {
            if(outputCount >= WIDTH * HEIGHT) return;
            var x = outputCount % WIDTH;
            var y = outputCount / WIDTH;
            //System.Console.WriteLine($"({x},{y}) = {output}");
            grid[x,y] = output > 0 ? false : true;
            outputCount++;
        }
    }
}