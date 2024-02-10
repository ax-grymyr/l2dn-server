namespace L2Dn.GameServer.Model.Geo.Internal;

public readonly struct GeoPoint(int x, int y): IEquatable<GeoPoint>
{
    public int X { get; } =
        x is >= 0 and < Constants.CellsInWorldX ? x : throw new ArgumentOutOfRangeException(nameof(x));

    public int Y { get; } =
        y is >= 0 and < Constants.CellsInWorldY ? y : throw new ArgumentOutOfRangeException(nameof(y));

    public bool Equals(GeoPoint other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is GeoPoint other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

    public static bool operator ==(GeoPoint left, GeoPoint right) => left.X == right.X && left.Y == right.Y;
    public static bool operator !=(GeoPoint left, GeoPoint right) => left.X != right.X || left.Y != right.Y;

    public static implicit operator GeoPoint(in Location location) => new((location.X - Location.WorldXMin) / 16,
        (location.Y - Location.WorldYMin) / 16);
}
