﻿namespace L2Dn.Packages.DatDefinitions.Definitions.Shared;

public class Location
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public override string ToString()
    {
        return $"X: {X}; Y: {Y}; Z: {Z}";
    }
}