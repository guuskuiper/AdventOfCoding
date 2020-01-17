using System;
using System.Collections.Generic;
using System.Linq;
using Day5;

namespace Day23
{
    public class Packet
    {
        public long X;
        public long Y;

        public override string ToString()
        {
            return $"(X={X}, Y={Y})";
        }
    }
}