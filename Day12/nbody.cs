using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Day12
{
    public class NBody
    {
        readonly IVector3[] initalPositions;
        readonly int initialEnergy;

        IVector3[] postions;
        IVector3[] velocities;

        public NBody(IEnumerable<IVector3> bodiesPositions)
        {
            postions = bodiesPositions.ToArray();
            initalPositions = bodiesPositions.ToArray();

            velocities = new IVector3[postions.Length];

            initialEnergy = CalcEnergy();
        }

        public void Simulate(int steps)
        {
            for(int step = 0; step < steps; step++)
            {
                UpdateVelocities();
                UpdatePositions();
            }
            var energy = CalcEnergy();
            System.Console.WriteLine($"Total energy: {energy} after {steps} steps");
        }

        private int CalcEnergy()
        {
            var energy = 0;

            for(int i = 0; i < postions.Length; i++)
            {
                var pot = PotentialEnergy(i);
                var kin = KineticEnergy(i);

                energy += pot * kin;
            }

            return energy;
        }

        private int PotentialEnergy(int planet)
        {
            var energy = 0;
            energy += (int)Math.Abs(postions[planet].X);
            energy += (int)Math.Abs(postions[planet].Y);
            energy += (int)Math.Abs(postions[planet].Z);

            return energy;
        }

        private int KineticEnergy(int planet)
        {
            var energy = 0;
            energy += (int)Math.Abs(velocities[planet].X);
            energy += (int)Math.Abs(velocities[planet].Y);
            energy += (int)Math.Abs(velocities[planet].Z);
            return energy;
        }

        private void UpdateVelocities()
        {
            // gravity, attract if the value on an axis is not the same
            for (int i = 0; i < postions.Length; i++)
            {
                for(int j = i; j < postions.Length; j++)
                {
                    if (postions[i].X != postions[j].X)
                    {
                        var dx = postions[i].X > postions[j].X ? -1 : 1;
                        velocities[i].X += dx;
                        velocities[j].X -= dx;
                    } 

                    if (postions[i].Y != postions[j].Y)
                    {
                        var dy = postions[i].Y > postions[j].Y ? -1 : 1;
                        velocities[i].Y += dy;
                        velocities[j].Y -= dy;
                    } 

                    if (postions[i].Z != postions[j].Z)
                    {
                        var dz = postions[i].Z > postions[j].Z ? -1 : 1;
                        velocities[i].Z += dz;
                        velocities[j].Z -= dz;
                    } 
                }
            }
        }

        public void UpdatePositions()
        {
            for (int i = 0; i < postions.Length; i++)
            {
                postions[i].X += velocities[i].X;
                postions[i].Y += velocities[i].Y;
                postions[i].Z += velocities[i].Z;
            }
        }

        static long LCM(IEnumerable<long> numbers)
        {
            return numbers.Aggregate(lcm);
        }
        static long lcm(long a, long b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }
        static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        private long SimulateEntireAxis(int axis)
        {
            // reset
            Array.Copy(initalPositions, postions, initalPositions.Length);
            velocities = new IVector3[postions.Length];

            // TODO: all axis at the same time
            int step = 0;
            do{
                step++;
                UpdateVelocities();
                UpdatePositions();
            } while(!Same());

            return step;

            bool Same()
            {
                return velocities.All(x => x.GetAxisValue(axis) == 0) && 
                    postions.Select(x => x.GetAxisValue(axis)).SequenceEqual(initalPositions.Select(x => x.GetAxisValue(axis)));
            }
        }

        public void SimulateUntillRepeat()
        {
            // calculate all periods the axis X,Y,Z for all the bodies
            var periods = new List<long> ();
            for (int i = 0; i < 3; i++)
            {
                //periods2.Add(SimulateAxis(i));
                periods.Add(SimulateEntireAxis(i));
            }

            // least common multiple is anwser
            var totalPeriod = LCM(periods);
            System.Console.WriteLine($"Total period {totalPeriod}: {string.Join(',' , periods)}");
        }
    }
}
