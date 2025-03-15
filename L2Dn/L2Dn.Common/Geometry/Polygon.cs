using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace L2Dn.Geometry;

public readonly struct Polygon
{
    public ImmutableArray<Location2D> Points { get; }
    public Rectangle Bounds { get; }

    public Polygon(ImmutableArray<Location2D> points)
    {
        if (points.IsDefaultOrEmpty || points.Length < 3)
            throw new ArgumentException("Polygon must have at least 3 points.");

        Points = points;
        Bounds = CalculateBounds(points);
    }

    private static Rectangle CalculateBounds(ImmutableArray<Location2D> points)
    {
        Location2D[] array = ImmutableCollectionsMarshal.AsArray(points)!;
        int boundsMinX = array.Min(p => p.X);
        int boundsMaxX = array.Max(p => p.X);

        int boundsMinY = array.Min(p => p.Y);
        int boundsMaxY = array.Max(p => p.Y);

        return new Rectangle(boundsMinX, boundsMinY, boundsMaxX - boundsMinX, boundsMaxY - boundsMinY);
    }

    public bool Contains(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        ImmutableArray<Location2D> points = Points;
        int hits = 0;
        Location2D last = points[^1];
        Location2D cur;

        // Walk the edges of the polygon
        for (int i = 0; i < points.Length; last = cur, i++)
        {
            cur = points[i];
            if (cur == last)
                continue;

            int leftx;
            if (cur.X < last.X)
            {
                if (x >= last.X)
                    continue;

                leftx = cur.X;
            }
            else
            {
                if (x >= cur.X)
                    continue;

                leftx = last.X;
            }

            double test1;
            double test2;
            if (cur.Y < last.Y)
            {
                if (y < cur.Y || y >= last.Y)
                    continue;

                if (x < leftx)
                {
                    hits++;
                    continue;
                }

                test1 = x - cur.X;
                test2 = y - cur.Y;
            }
            else
            {
                if (y < last.Y || y >= cur.Y)
                    continue;

                if (x < leftx)
                {
                    hits++;
                    continue;
                }

                test1 = x - last.X;
                test2 = y - last.Y;
            }

            if (test1 < test2 / (last.Y - cur.Y) * (last.X - cur.X))
                hits++;
        }

        return (hits & 1) != 0;
    }

    private Crossings? GetCrossings(double xlo, double ylo,
        double xhi, double yhi)
    {
        ImmutableArray<Location2D> points = Points;
        Crossings cross = new Crossings(xlo, ylo, xhi, yhi);
        Location2D last = points[^1];

        // Walk the edges of the polygon
        for (int i = 0; i < points.Length; i++)
        {
            Location2D cur = points[i];
            if (cross.AccumulateLine(last.X, last.Y, cur.X, cur.Y))
                return null;

            last = cur;
        }

        return cross;
    }

    public bool Intersects(int x, int y, int w, int h)
    {
        if (!Bounds.Intersects(x, y, w, h))
            return false;

        Crossings? cross = GetCrossings(x, y, x + w, y + h);
        return cross == null || !cross.IsEmpty();
    }

    private sealed class Crossings(double xlo, double ylo, double xhi, double yhi)
    {
        private int _limit;
        private double[] _yRanges = new double[10];

        private void Record(double yStart, double yEnd)
        {
            if (yStart >= yEnd)
                return;

            int from = 0;
            // Quickly jump over all pairs that are completely "above"
            while (from < _limit && yStart > _yRanges[from + 1])
            {
                from += 2;
            }

            int to = from;
            while (from < _limit)
            {
                double yrlo = _yRanges[from++];
                double yrhi = _yRanges[from++];
                if (yEnd < yrlo)
                {
                    // Quickly handle insertion of the new range
                    _yRanges[to++] = yStart;
                    _yRanges[to++] = yEnd;
                    yStart = yrlo;
                    yEnd = yrhi;
                    continue;
                }

                // The ranges overlap - sort, collapse, insert, iterate
                double yll, ylh, yhl, yhh;
                if (yStart < yrlo)
                {
                    yll = yStart;
                    ylh = yrlo;
                }
                else
                {
                    yll = yrlo;
                    ylh = yStart;
                }

                if (yEnd < yrhi)
                {
                    yhl = yEnd;
                    yhh = yrhi;
                }
                else
                {
                    yhl = yrhi;
                    yhh = yEnd;
                }

                if (ylh == yhl)
                {
                    yStart = yll;
                    yEnd = yhh;
                }
                else
                {
                    if (ylh > yhl)
                    {
                        yStart = yhl;
                        yhl = ylh;
                        ylh = yStart;
                    }

                    if (yll != ylh)
                    {
                        _yRanges[to++] = yll;
                        _yRanges[to++] = ylh;
                    }

                    yStart = yhl;
                    yEnd = yhh;
                }

                if (yStart >= yEnd)
                {
                    break;
                }
            }

            if (to < from && from < _limit)
            {
                Array.Copy(_yRanges, from, _yRanges, to, _limit - from);
            }

            to += _limit - from;
            if (yStart < yEnd)
            {
                if (to >= _yRanges.Length)
                {
                    double[] newranges = new double[to + 10];
                    Array.Copy(_yRanges, 0, newranges, 0, to);
                    _yRanges = newranges;
                }

                _yRanges[to++] = yStart;
                _yRanges[to++] = yEnd;
            }

            _limit = to;
        }

        public bool IsEmpty() => _limit == 0;

        public bool AccumulateLine(double x0, double y0, double x1, double y1)
        {
            if (yhi <= y0 || ylo >= y1)
                return false;

            if (x0 >= xhi && x1 >= xhi)
                return false;

            if (y0 == y1)
                return x0 >= xlo || x1 >= xlo;

            double xstart, ystart, xend, yend;
            double dx = x1 - x0;
            double dy = y1 - y0;
            if (y0 < ylo)
            {
                xstart = x0 + (ylo - y0) * dx / dy;
                ystart = ylo;
            }
            else
            {
                xstart = x0;
                ystart = y0;
            }

            if (yhi < y1)
            {
                xend = x0 + (yhi - y0) * dx / dy;
                yend = yhi;
            }
            else
            {
                xend = x1;
                yend = y1;
            }

            if (xstart >= xhi && xend >= xhi)
                return false;

            if (xstart > xlo || xend > xlo)
                return true;

            Record(ystart, yend);
            return false;
        }
    }
}