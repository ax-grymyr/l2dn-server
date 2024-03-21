namespace L2Dn.Extensions;

public static class PredicateExtensions
{
    public static Predicate<T> And<T>(this Predicate<T> predicate, Predicate<T> other) => x => predicate(x) && other(x);
}