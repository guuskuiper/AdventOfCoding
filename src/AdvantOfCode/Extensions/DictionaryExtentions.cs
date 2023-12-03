namespace AdventOfCode.Extensions;

public static class DictionaryExtensions
{
    public static void AddOrCreate<T>(this Dictionary<T, long> elements, T key, long value) 
        where T : notnull
    {
        if (!elements.TryAdd(key, value))
        {
            elements[key] += value;
        }
    }
    
    public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> elements, TKey key) 
        where TKey : notnull
        where TValue : new()
    {
        TValue value;
        if (elements.TryGetValue(key, out TValue? element))
        {
            value = element;
        }
        else
        {
            value = new TValue();
            elements.Add(key, value);
        }
        return value;
    }
}