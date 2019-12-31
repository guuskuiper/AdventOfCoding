using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Day12
{
    public class NBody
    {
        readonly Vector3[] initalPositions;
        readonly int initialEnergy;

        Vector3[] postions;
        Vector3[] velocities;

        public NBody(IEnumerable<Vector3> bodiesPositions)
        {
            postions = bodiesPositions.ToArray();
            initalPositions = bodiesPositions.ToArray();

            velocities = new Vector3[postions.Length];

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

        private List<long> SimulatePeriod(int body)
        {
            var periods = new List<long> ();
            for (int i = 0; i < 3; i++)
            {
                periods.Add(SimulateBodyAxis(body, i));
            }

            var lcm = LCM(periods);
            var lcm2 = MathUtils.LCM(periods);
            System.Console.WriteLine($"Body {body} lcm {lcm} lcm2 {lcm2} periods: {string.Join(',', periods)}");

            // lcm
            return periods;
        }

        private long SimulateAxis(int axis)
        {
            var periods = new List<long> ();
            for (int i = 0; i < postions.Length; i++)
            {
                periods.Add(SimulateBodyAxis(i, axis));
            }

            var lcm = LCM(periods);
            var lcm2 = MathUtils.LCM(periods);
            System.Console.WriteLine($"Axis {axis} lcm {lcm} lcm2 {lcm2} periods: {string.Join(',', periods)}");

            // lcm
            return lcm2;
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

        private long SimulateBodyAxis(int body, int axis)
        {
            // reset
            Array.Copy(initalPositions, postions, initalPositions.Length);
            velocities = new Vector3[postions.Length];

            int step = 0;
            do{
                step++;
                UpdateVelocities();
                UpdatePositions();
            } while(!Same());

            return step;

            bool Same()
            {
                return (velocities[body].GetAxisValue(axis) == 0) && 
                    (postions[body].GetAxisValue(axis) == initalPositions[body].GetAxisValue(axis));
            }
        }

        public void SimulateUntillRepeat()
        {
            // calculate all periods of all bodies and X,Y,Z
            var periods = new List<long> ();
            for (int i = 0; i < postions.Length; i++)
            {
                periods.AddRange(SimulatePeriod(i));
            }

            var periods2 = new List<long> ();
            for (int i = 0; i < 3; i++)
            {
                periods2.Add(SimulateAxis(i));
            }

            // least common multiple is anwser
            var totalPeriod = LCM(periods);
            var totalPeriod2 = MathUtils.LCM(periods);
            System.Console.WriteLine($"Total period {totalPeriod} or {totalPeriod2}");
        }
    }

    public static class Vector3Extentions
    {
        public static float GetAxisValue(this Vector3 v, int axis)
        {
            switch(axis)
            {
                default:
                    throw new Exception("Invalid axis");
                case 0:
                    return v.X;
                    break;
                case 1:
                    return v.Y;
                    break;
                case 2:
                    return v.Z;
                    break;
            }
        }
    }
}
