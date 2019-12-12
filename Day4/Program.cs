using System;
using System.Collections.Generic;
using System.Linq;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "347312-805915";
            var split = input.Split('-').Select(x => int.Parse(x)).ToList();
            var min = split[0];
            var max = split[1];
            System.Console.WriteLine($"Min {min}-{max}");

            int count = 0;
            for(int i = min; i <= max; i++)
            {
                var digits = ToDigits(i);

                if(!CheckSix(digits)) continue;
                if(!CheckAdjecent(digits)) continue;
                if(!CheckNotDecreasing(digits)) continue;
                if(!CheckDouble(digits)) continue;

                System.Console.WriteLine(string.Join(' ', digits));
                count++;
            }
            System.Console.WriteLine(count);
        }

        static List<int> ToDigits(int i)
        {
            var digits = new List<int>();
            while(i > 0)
            {
                digits.Add(i % 10);
                i /= 10;
            }

            digits.Reverse();
            return digits;
        }

        static bool CheckSix(List<int> digits)
        {
            return digits.Count == 6;
        }

        static bool CheckAdjecent(List<int> digits)
        {
            int prev = digits[0];
            for(int i = 1; i < digits.Count; i++)
            {   
                var cur = digits[i];
                if(cur == prev) return true;
                prev = cur;
            }
            return false;
        }

        static bool CheckNotDecreasing(List<int> digits)
        {
            int prev = digits[0];
            for(int i = 1; i < digits.Count; i++)
            {   
                var cur = digits[i];
                if(cur < prev) return false;
                prev = cur;
            }
            return true;
        }

        static bool CheckDouble(List<int> digits)
        {
            int prevprev = digits[0];
            int prev = digits[1];
            bool potentialDouble = prev == prevprev;
            for(int i = 2; i < digits.Count; i++)
            {   
                var cur = digits[i];
                
                if(cur == prevprev) 
                {
                    potentialDouble = false;
                }
                else if(potentialDouble)
                {
                    return true;
                }
                else
                {
                    potentialDouble = cur == prev;
                }
                prevprev = prev;
                prev = cur;
            }
            return potentialDouble;
        }
    }
}
