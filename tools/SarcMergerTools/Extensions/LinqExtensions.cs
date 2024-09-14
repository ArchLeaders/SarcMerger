namespace SarcMergerTools.Extensions;

public static class LinqExtensions
{
    public static IEnumerable<T> Eval<T>(this IEnumerable<T> src)
    {
        // ReSharper disable PossibleMultipleEnumeration, EmptyEmbeddedStatement
        foreach (T _ in src);
        return src;
    }
    
    /// <summary>
    /// Force enumeration and return a 
    /// </summary>
    /// <param name="src"></param>
    /// <param name="predicate"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool EvalAny<T>(this IEnumerable<T> src, Func<T, bool> predicate)
    {
        return src
            .Select(predicate)
            .Aggregate(false, (current, predicateResult) => current || predicateResult);
    }
}