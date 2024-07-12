namespace L2Dn.GameServer.Utilities;

public class Polygon
{
    private readonly int _npoints;
    private readonly int[] _xpoints;
    private readonly int[] _ypoints;
    private readonly Rectangle _bounds;
    private static readonly int MIN_LENGTH = 4;

    public int npoints => _npoints;
    public int[] xpoints => _xpoints;
    public int[] ypoints => _ypoints;
    
    public Polygon(int[] xpoints, int[] ypoints, int npoints)
    {
        if (npoints > xpoints.Length || npoints > ypoints.Length)
        {
            throw new IndexOutOfRangeException("npoints > xpoints.length || " +
                                               "npoints > ypoints.length");
        }

        // Fix 6191114: should throw NegativeArraySizeException with
        // negative npoints
        if (npoints < 0)
            throw new ArgumentOutOfRangeException(nameof(npoints), "npoints < 0");

        // Fix 6343431: Applet compatibility problems if arrays are not
        // exactly npoints in length
        this._npoints = npoints;
        this._xpoints = (int[])xpoints.Clone();
        this._ypoints = (int[])ypoints.Clone();
        _bounds = calculateBounds(xpoints, ypoints, npoints);
    }

    private static Rectangle calculateBounds(int[] xpoints, int[] ypoints, int npoints)
    {
        int boundsMinX = int.MaxValue;
        int boundsMinY = int.MaxValue;
        int boundsMaxX = int.MinValue;
        int boundsMaxY = int.MinValue;
        for (int i = 0; i < npoints; i++)
        {
            int x = xpoints[i];
            boundsMinX = Math.Min(boundsMinX, x);
            boundsMaxX = Math.Max(boundsMaxX, x);
            int y = ypoints[i];
            boundsMinY = Math.Min(boundsMinY, y);
            boundsMaxY = Math.Max(boundsMaxY, y);
        }

        return new Rectangle(boundsMinX, boundsMinY,
            boundsMaxX - boundsMinX,
            boundsMaxY - boundsMinY);
    }

    public Rectangle getBounds() => _bounds;

    public bool contains(int x, int y)
    {
        if (_npoints <= 2 || !_bounds.Contains(x, y))
        {
            return false;
        }

        int hits = 0;
        int lastx = _xpoints[_npoints - 1];
        int lasty = _ypoints[_npoints - 1];
        int curx, cury;
        // Walk the edges of the polygon
        for (int i = 0; i < _npoints; lastx = curx, lasty = cury, i++)
        {
            curx = _xpoints[i];
            cury = _ypoints[i];
            if (cury == lasty)
            {
                continue;
            }

            int leftx;
            if (curx < lastx)
            {
                if (x >= lastx)
                {
                    continue;
                }

                leftx = curx;
            }
            else
            {
                if (x >= curx)
                {
                    continue;
                }

                leftx = lastx;
            }

            double test1, test2;
            if (cury < lasty)
            {
                if (y < cury || y >= lasty)
                {
                    continue;
                }

                if (x < leftx)
                {
                    hits++;
                    continue;
                }

                test1 = x - curx;
                test2 = y - cury;
            }
            else
            {
                if (y < lasty || y >= cury)
                {
                    continue;
                }

                if (x < leftx)
                {
                    hits++;
                    continue;
                }

                test1 = x - lastx;
                test2 = y - lasty;
            }

            if (test1 < (test2 / (lasty - cury) * (lastx - curx)))
            {
                hits++;
            }
        }

        return ((hits & 1) != 0);
    }

    private Crossings getCrossings(double xlo, double ylo,
        double xhi, double yhi)
    {
        Crossings cross = new Crossings.EvenOdd(xlo, ylo, xhi, yhi);
        int lastx = _xpoints[_npoints - 1];
        int lasty = _ypoints[_npoints - 1];
        int curx, cury;
        // Walk the edges of the polygon
        for (int i = 0; i < _npoints; i++)
        {
            curx = _xpoints[i];
            cury = _ypoints[i];
            if (cross.accumulateLine(lastx, lasty, curx, cury))
            {
                return null;
            }

            lastx = curx;
            lasty = cury;
        }

        return cross;
    }

    public bool intersects(int x, int y, int w, int h)
    {
        if (_npoints <= 0 || !_bounds.Intersects(x, y, w, h))
        {
            return false;
        }

        Crossings cross = getCrossings(x, y, x + w, y + h);
        return (cross == null || !cross.isEmpty());
    }
}