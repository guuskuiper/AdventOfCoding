using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Day12
{
    public class NBody
    {
        Vector3[] postions;
        Vector3[] velocities;

        public NBody(IEnumerable<Vector3> bodiesPositions)
        {
            postions = bodiesPositions.ToArray();

            velocities = new Vector3[postions.Length];
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
                for(int j = 0; j < postions.Length; j++)
                {
                    if (postions[i].X != postions[j].X)
                    {
                        velocities[i].X += postions[i].X > postions[j].X ? -1 : 1;
                    } 

                    if (postions[i].Y != postions[j].Y)
                    {
                        velocities[i].Y += postions[i].Y > postions[j].Y ? -1 : 1;
                    } 

                    if (postions[i].Z != postions[j].Z)
                    {
                        velocities[i].Z += postions[i].Z > postions[j].Z ? -1 : 1;
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
    }
}
