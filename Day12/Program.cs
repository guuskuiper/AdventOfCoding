using System;
using System.Numerics;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Day12
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            //var text = File.ReadAllText("example2.txt");
            string pattern = @"<x=(?'x'.+), y=(?'y'.+), z=(?'z'.+)>";

            List<Vector3> bodies = new List<Vector3>();

            RegexOptions options = RegexOptions.Multiline;
        
            foreach (Match m in Regex.Matches(text, pattern, options))
            {
                var x = int.Parse(m.Groups["x"].Value);
                var y = int.Parse(m.Groups["y"].Value);
                var z = int.Parse(m.Groups["z"].Value);
                var body = new Vector3(x, y, z);
                bodies.Add(body);
            }

            var nbody = new NBody(bodies);
            nbody.Simulate(1000);
        }
    }
}
