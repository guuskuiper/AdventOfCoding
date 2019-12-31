using System;

namespace Day12
{
    public struct IVector3 {
        public int X;
        public int Y;
        public int Z;

        public IVector3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int GetAxisValue(int axis)
        {
            switch(axis)
            {
                default:
                    throw new Exception("Invalid axis");
                case 0:
                    return X;
                    break;
                case 1:
                    return Y;
                    break;
                case 2:
                    return Z;
                    break;
            }
        }
    }
}