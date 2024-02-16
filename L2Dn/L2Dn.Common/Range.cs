namespace L2Dn;

public readonly record struct Range<T>(T Left, T Right)
    where T: IComparable<T>, IEquatable<T>
{
    public override string ToString() => $"[{Left}..{Right}]";
}