using System;
using System.Collections.Generic;

namespace Day6
{
    public class Orbit
    {
        public string Name;
        public List<Orbit> Children;

        public Orbit(string name)
        {
            Name = name;

            Children = new List<Orbit> ();
        }

        public int Traverse(int depth)
        {
            var currentSum = depth;
            foreach(var child in Children)
            {
                currentSum += child.Traverse(depth + 1);
            }

            return currentSum;
        }
    }

    public class Orbits
    {
        Dictionary<string, Orbit> CurrentOrbits;

        public Orbits(IEnumerable<string> orbits)
        {
            CurrentOrbits = new Dictionary<string, Orbit> ();
            var com = new Orbit("COM");
            CurrentOrbits.Add("COM", com);

            foreach(var orbit in orbits)
            {
                var orbitNames = orbit.Split(')');

                var leftName = orbitNames[0];
                Orbit leftOrbit;
                if(!CurrentOrbits.ContainsKey(leftName))
                {
                    leftOrbit = new Orbit(leftName);
                    CurrentOrbits.Add(leftName, leftOrbit);
                }
                else
                {
                    leftOrbit = CurrentOrbits[leftName];
                }

                var rightName = orbitNames[1];
                Orbit rightOrbit;
                if(!CurrentOrbits.ContainsKey(rightName))
                {
                    rightOrbit = new Orbit(rightName);
                    CurrentOrbits.Add(rightName, rightOrbit);
                }
                else
                {
                    rightOrbit = CurrentOrbits[rightName];
                }

                leftOrbit.Children.Add(rightOrbit);
            }
        }

        public int GetOrbitCount()
        {
            var start = CurrentOrbits["COM"];
            return start.Traverse(0);
        }
    }
}