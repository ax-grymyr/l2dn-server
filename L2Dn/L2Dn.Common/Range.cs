namespace L2Dn;

public readonly record struct Range<T>(T Left, T Right)
    where T: IComparable<T>, IEquatable<T>
{
    public bool Contains(T value) => Left.CompareTo(value) <= 0 && Right.CompareTo(value) >= 0;

    public override string ToString() => $"[{Left}..{Right}]";
}

public static class Range
{
    public static Range<T> Create<T>(T left, T right)
        where T: IComparable<T>, IEquatable<T> =>
        new(left, right);
}