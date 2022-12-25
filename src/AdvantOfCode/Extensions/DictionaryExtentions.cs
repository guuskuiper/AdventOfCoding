namespace AdventOfCode.Extensions;

public static class DictionaryExtensions
{
    public static void AddOrCreate<T>(this Dictionary<T, long> elements, T key, long value) where T : notnull
    {
        if (elements.ContainsKey(key))
        {
            elements[key] += value;
        }
        else
        {
            elements.Add(key, value);
        }
    }
}