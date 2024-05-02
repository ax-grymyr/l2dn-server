namespace L2Dn.Geometry;

public readonly record struct Location3D(Location2D Location2D, int Z): ILocation3D
{
    public Location3D(int x, int y, int z): this(new Location2D(x, y), z)
    {
    }

    public int X => Location2D.X;
    public int Y => Location2D.Y;

    public override string ToString() => $"({X}, {Y}, {Z})";
}