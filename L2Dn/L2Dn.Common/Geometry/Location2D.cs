namespace L2Dn.Geometry;

public readonly record struct Location2D(int X, int Y)
{
    public override string ToString() => $"({X}, {Y})";
}