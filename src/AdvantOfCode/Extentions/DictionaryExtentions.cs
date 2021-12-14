namespace AdventOfCode.Extentions;

public static class DictionaryExtensions
{
    public static void AddOrCreate<T>(this Dictionary<T, long> elements, T key, long value)
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