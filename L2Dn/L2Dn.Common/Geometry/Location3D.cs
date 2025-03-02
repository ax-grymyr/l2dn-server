using System.Text.Json.Serialization;

namespace L2Dn.Geometry;

public readonly record struct Location3D(int X, int Y, int Z): ILocation3D
{
    public Location3D(Location2D location2D, int z): this(location2D.X, location2D.Y, z)
    {
    }

    [JsonIgnore]
    public Location2D Location2D => new(X, Y);

    [JsonIgnore]
    public double Length2D => double.Sqrt((double)X * X + (double)Y * Y);

    [JsonIgnore]
    public double Length3D => double.Sqrt((double)X * X + (double)Y * Y + (double)Z * Z);

    [JsonIgnore]
    public double SquaredLength2D => (double)X * X + (double)Y * Y;

    [JsonIgnore]
    public double SquaredLength3D => (double)X * X + (double)Y * Y + (double)Z * Z;

    public Location3D Scale(double k) => new((int)(X * k), (int)(Y * k), (int)(Z * k));

    public Vector2D<double> Normalize2D()
    {
        double length = Length2D;
        return new Vector2D<double>(X / length, Y / length);
    }

    public override string ToString() => $"({X}, {Y}, {Z})";

    public static Location3D operator +(Location3D a, Location3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Location3D operator -(Location3D a, Location3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
}