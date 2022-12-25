namespace AdventOfCode.Extensions;

public static class ArrayExtensions
{
    public static IEnumerable<T> ToEnumerable<T>(this T[,] target)
    {
        foreach (var item in target)
            yield return item;
    }
    
    public static IEnumerable<T> ToEnumerable<T>(this T[,,] target)
    {
        foreach (var item in target)
            yield return item;
    }
}