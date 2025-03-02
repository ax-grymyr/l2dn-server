﻿using System.Text.Json.Serialization;

namespace L2Dn.Geometry;

public readonly record struct Location(int X, int Y, int Z, int Heading): ILocation
{
    public Location(Location3D location, int heading): this(location.X, location.Y, location.Z, heading)
    {
    }

    [JsonIgnore]
    public Location2D Location2D => new(X, Y);

    [JsonIgnore]
    public Location3D Location3D => new(X, Y, Z);

    public override string ToString() => $"({X}, {Y}, {Z}, Heading={Heading})";
}