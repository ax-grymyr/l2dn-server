namespace L2Dn.GameServer.Utilities;

public abstract class Crossings
{
    int limit = 0;
    double[] yranges = new double[10];
    double xlo, ylo, xhi, yhi;

    public Crossings(double xlo, double ylo, double xhi, double yhi)
    {
        this.xlo = xlo;
        this.ylo = ylo;
        this.xhi = xhi;
        this.yhi = yhi;
    }

    public double getXLo()
    {
        return xlo;
    }

    public double getYLo()
    {
        return ylo;
    }

    public double getXHi()
    {
        return xhi;
    }

    public double getYHi()
    {
        return yhi;
    }

    public abstract void record(double ystart, double yend, int direction);

    public bool isEmpty()
    {
        return (limit == 0);
    }

    public bool accumulateLine(double x0, double y0, double x1, double y1)
    {
        if (y0 <= y1)
        {
            return accumulateLine(x0, y0, x1, y1, 1);
        }
        else
        {
            return accumulateLine(x1, y1, x0, y0, -1);
        }
    }

    public bool accumulateLine(double x0, double y0,
        double x1, double y1,
        int direction)
    {
        if (yhi <= y0 || ylo >= y1)
        {
            return false;
        }

        if (x0 >= xhi && x1 >= xhi)
        {
            return false;
        }

        if (y0 == y1)
        {
            return (x0 >= xlo || x1 >= xlo);
        }

        double xstart, ystart, xend, yend;
        double dx = (x1 - x0);
        double dy = (y1 - y0);
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
        {
            return false;
        }

        if (xstart > xlo || xend > xlo)
        {
            return true;
        }

        record(ystart, yend, direction);
        return false;
    }

    public class EvenOdd: Crossings
    {
        public EvenOdd(double xlo, double ylo, double xhi, double yhi): base(xlo, ylo, xhi, yhi)
        {
        }

        public override void record(double ystart, double yend, int direction)
        {
            if (ystart >= yend)
            {
                return;
            }

            int from = 0;
            // Quickly jump over all pairs that are completely "above"
            while (from < limit && ystart > yranges[from + 1])
            {
                from += 2;
            }

            int to = from;
            while (from < limit)
            {
                double yrlo = yranges[from++];
                double yrhi = yranges[from++];
                if (yend < yrlo)
                {
                    // Quickly handle insertion of the new range
                    yranges[to++] = ystart;
                    yranges[to++] = yend;
                    ystart = yrlo;
                    yend = yrhi;
                    continue;
                }

                // The ranges overlap - sort, collapse, insert, iterate
                double yll, ylh, yhl, yhh;
                if (ystart < yrlo)
                {
                    yll = ystart;
                    ylh = yrlo;
                }
                else
                {
                    yll = yrlo;
                    ylh = ystart;
                }

                if (yend < yrhi)
                {
                    yhl = yend;
                    yhh = yrhi;
                }
                else
                {
                    yhl = yrhi;
                    yhh = yend;
                }

                if (ylh == yhl)
                {
                    ystart = yll;
                    yend = yhh;
                }
                else
                {
                    if (ylh > yhl)
                    {
                        ystart = yhl;
                        yhl = ylh;
                        ylh = ystart;
                    }

                    if (yll != ylh)
                    {
                        yranges[to++] = yll;
                        yranges[to++] = ylh;
                    }

                    ystart = yhl;
                    yend = yhh;
                }

                if (ystart >= yend)
                {
                    break;
                }
            }

            if (to < from && from < limit)
            {
                Array.Copy(yranges, from, yranges, to, limit - from);
            }

            to += (limit - from);
            if (ystart < yend)
            {
                if (to >= yranges.Length)
                {
                    double[] newranges = new double[to + 10];
                    Array.Copy(yranges, 0, newranges, 0, to);
                    yranges = newranges;
                }

                yranges[to++] = ystart;
                yranges[to++] = yend;
            }

            limit = to;
        }
    }
}