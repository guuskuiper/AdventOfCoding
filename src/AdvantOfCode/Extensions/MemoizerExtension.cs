using System.Collections.Concurrent;

namespace AdventOfCode.Extensions;

public static class MemoizerExtension
{
    public static Func<TRes> Memoize<T1, TRes>(this Func<TRes> f) where TRes : class
    {
        TRes? cache = null;

        return () =>
        {
            if (cache is null)
            {
                cache = f();
            }

            return cache;
        };
    }
    
    public static Func<T1, TRes> Memoize<T1, TRes>(this Func<T1, TRes> f) where T1 : notnull
    {
        ConcurrentDictionary<T1, TRes> cache = new();
        return args1 => cache.GetOrAdd(args1, f(args1));
    }
    
    public static Func<T1, T2, TRes> Memoize<T1, T2, TRes>(this Func<T1, T2, TRes> f)
    {
        ConcurrentDictionary<(T1, T2), TRes> cache = new();
        return (args1, args2) => cache.GetOrAdd((args1, args2), f(args1, args2));
    }
    
    public static Func<T1, T2, T3, TRes> Memoize<T1, T2, T3, TRes>(this Func<T1, T2, T3, TRes> f)
    {
        ConcurrentDictionary<(T1, T2, T3), TRes> cache = new();
        return (args1, args2, args3) => cache.GetOrAdd((args1, args2, args3), f(args1, args2, args3));
    }
}