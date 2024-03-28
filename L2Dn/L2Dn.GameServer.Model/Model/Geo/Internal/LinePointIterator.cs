namespace L2Dn.GameServer.Model.Geo.Internal;

internal struct LinePointIterator
{
    // src is moved towards dst in next()
    private Point2D _src;
    private readonly Point2D _dst;

    private readonly long _dx;
    private readonly long _dy;
    private readonly long _sx;
    private readonly long _sy;
    private long _error;

    private bool _first;

    public LinePointIterator(Point2D src, Point2D dst)
    {
        _src = src;
        _dst = dst;
        _dx = Math.Abs((long)dst.X - src.X);
        _dy = Math.Abs((long)dst.Y - src.Y);
        _sx = src.X < dst.X ? 1 : -1;
        _sy = src.Y < dst.Y ? 1 : -1;
        if (_dx >= _dy)
            _error = _dx / 2;
        else
            _error = _dy / 2;

        _first = true;
    }

    public bool Next()
    {
        if (_first)
        {
            _first = false;
            return true;
        }

        if (_dx >= _dy)
        {
            if (_src.X != _dst.X)
            {
                _src = _src with { X = (int)(_src.X + _sx) };
                _error += _dy;
                if (_error >= _dx)
                {
                    _src = _src with { Y = (int)(_src.Y + _sy) };
                    _error -= _dx;
                }

                return true;
            }
        }
        else if (_src.Y != _dst.Y)
        {
            _src = _src with { Y = (int)(_src.Y + _sy) };
            _error += _dx;
            if (_error >= _dy)
            {
                _src = _src with { X = (int)(_src.X + _sx) };
                _error -= _dy;
            }

            return true;
        }

        return false;
    }

    public Point2D Point => _src;
}