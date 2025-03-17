namespace L2Dn.Geometry;

public readonly record struct Rectangle(int X, int Y, int Width, int Height)
{
    public Location2D LeftTop => new(X, Y);
    public Location2D LeftBottom => new(X, Y + Height - 1);
    public Location2D RightTop => new(X + Width - 1, Y);
    public Location2D RightBottom => new(X + Width - 1, Y + Height - 1);

    public bool Contains(Location2D location)
    {
        if ((Width | Height) < 0)
        {
            // At least one of the dimensions is negative...
            return false;
        }

        // Note: if either dimension is zero, tests below must return false...
        if (location.X < X || location.Y < Y)
            return false;

        int w = Width + X;
        int h = Height + Y;

        // overflow || intersect
        return (w < X || w > location.X) && (h < Y || h > location.Y);
    }

    public bool Intersects(Rectangle rectangle)
    {
        int tw = Width;
        int th = Height;
        int rw = rectangle.Width;
        int rh = rectangle.Height;
        if (rw <= 0 || rh <= 0 || tw <= 0 || th <= 0)
            return false;

        int tx = X;
        int ty = Y;
        int rx = rectangle.X;
        int ry = rectangle.Y;
        rw += rx;
        rh += ry;
        tw += tx;
        th += ty;

        // overflow || intersect
        return (rw < rx || rw > tx) && (rh < ry || rh > ty) && (tw < tx || tw > rx) && (th < ty || th > ry);
    }
}