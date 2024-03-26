namespace L2Dn;

public readonly record struct Range<T>(T Left, T Right)
    where T: IComparable<T>, IEquatable<T>
{
    public bool Contains(T value) => Left.CompareTo(value) <= 0 && Right.CompareTo(value) >= 0;

    public override string ToString() => $"[{Left}..{Right}]";
}