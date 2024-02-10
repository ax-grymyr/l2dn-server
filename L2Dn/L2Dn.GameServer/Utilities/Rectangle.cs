namespace L2Dn.GameServer.Utilities;

public readonly struct Rectangle(int _x, int _y, int _width, int _height)
{
    public int x => _x;
    public int y => _y;
    public int width => _width;
    public int height => _height;

    public bool contains(int xx, int yy)
    {
        int w = this.width;
        int h = this.height;
        if ((w | h) < 0)
        {
            // At least one of the dimensions is negative...
            return false;
        }

        // Note: if either dimension is zero, tests below must return false...
        int x = this.x;
        int y = this.y;
        if (xx < x || yy < y)
        {
            return false;
        }

        w += x;
        h += y;
        //    overflow || intersect
        return ((w < x || w > xx) &&
                (h < y || h > yy));
    }

    public bool intersects(int rx, int ry, int rw, int rh)
    {
        return intersects(new Rectangle(rx, ry, rw, rh));
    }

    public bool intersects(Rectangle r)
    {
        int tw = this.width;
        int th = this.height;
        int rw = r.width;
        int rh = r.height;
        if (rw <= 0 || rh <= 0 || tw <= 0 || th <= 0)
        {
            return false;
        }

        int tx = this.x;
        int ty = this.y;
        int rx = r.x;
        int ry = r.y;
        rw += rx;
        rh += ry;
        tw += tx;
        th += ty;
        //      overflow || intersect
        return ((rw < rx || rw > tx) &&
                (rh < ry || rh > ty) &&
                (tw < tx || tw > rx) &&
                (th < ty || th > ry));
    }
}