namespace L2Dn.Geometry;

public readonly record struct Location2D(int X, int Y): ILocation2D
{
    public Location3D ToLocation3D(int z) => new(X, Y, z);

    public override string ToString() => $"({X}, {Y})";
}