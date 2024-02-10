namespace L2Dn.GameServer.Model.Geo.Internal;

internal readonly record struct Point2D(int X, int Y)
{
    public Point2D AddX(int dx) => this with { X = X + dx };
    public Point2D AddY(int dy) => this with { Y = Y + dy };
    
    public static implicit operator Point2D(GeoPoint point) => new(point.X, point.Y);
}