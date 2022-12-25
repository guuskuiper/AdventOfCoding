using System.Numerics;

namespace AdventOfCode.Extensions;

public static class MathGeneric
{
    public static T GCD<T>(T a, T b) where T : INumber<T>
    {
        while (b != T.Zero)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }

    public static T LCM<T>(T a, T b) where T : INumber<T>
        => a / GCD(a, b) * b;
    
    public static T LCM<T>(IEnumerable<T> values) where T : INumber<T>
        => values.Aggregate(LCM);
}