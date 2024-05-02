namespace L2Dn.Geometry;

public readonly record struct Location(Location3D Location3D, int Heading): ILocation
{
    public Location(int x, int y, int z, int heading): this(new Location3D(x, y, z), heading)
    {
    }

    public int X => Location3D.X;
    public int Y => Location3D.Y;
    public int Z => Location3D.Z;

    public Location2D Location2D => Location3D.Location2D;

    public override string ToString() => $"({X}, {Y}, {Z}, Heading={Heading})";
}