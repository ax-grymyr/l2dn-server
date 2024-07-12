namespace L2Dn.GameServer.Utilities;

public readonly record struct Rectangle(int X, int Y, int Width, int Height)
{
    public bool Contains(int x, int y)
    {
        if ((Width | Height) < 0)
        {
            // At least one of the dimensions is negative...
            return false;
        }

        // Note: if either dimension is zero, tests below must return false...
        if (x < X || y < Y)
            return false;

        int w = Width + X;
        int h = Height + Y;

        // overflow || intersect
        return (w < X || w > x) && (h < Y || h > y);
    }

    public bool Intersects(int rx, int ry, int rw, int rh) => Intersects(new Rectangle(rx, ry, rw, rh));

    public bool Intersects(Rectangle r)
    {
        int tw = Width;
        int th = Height;
        int rw = r.Width;
        int rh = r.Height;
        if (rw <= 0 || rh <= 0 || tw <= 0 || th <= 0)
            return false;

        int tx = X;
        int ty = Y;
        int rx = r.X;
        int ry = r.Y;
        rw += rx;
        rh += ry;
        tw += tx;
        th += ty;

        // overflow || intersect
        return (rw < rx || rw > tx) &&
            (rh < ry || rh > ty) &&
            (tw < tx || tw > rx) &&
            (th < ty || th > ry);
    }
}