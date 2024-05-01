namespace L2Dn.Geometry;

public readonly record struct Location3D(int X, int Y, int Z): ILocation3D
{
    public Location2D ToLocation2D() => new(X, Y);

    public override string ToString() => $"({X}, {Y}, {Z})";
}