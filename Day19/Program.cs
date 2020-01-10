﻿using System;
using System.IO;
using System.Linq;

namespace Day19
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            var instructions = text.Split(',').Select(long.Parse).ToArray();

            var tractorBeam = new TractorBeam(instructions);
            tractorBeam.Start();
        }
    }
}
