namespace L2Dn.GameServer.Model.Geo.Internal;

internal struct LinePointIterator3D
{
	private Point2D _src;
	private int _srcZ;
	private readonly Point2D _dst;
	private readonly int _dstZ;
	private readonly long _dx;
	private readonly long _dy;
	private readonly long _dz;
	private readonly long _sx;
	private readonly long _sy;
	private readonly long _sz;
	private long _error;
	private long _error2;
	private bool _first;

	public LinePointIterator3D(Point2D src, int srcZ, Point2D dst, int dstZ)
	{
		_src = src;
		_srcZ = srcZ;
		_dst = dst;
		_dstZ = dstZ;
		_dx = Math.Abs((long)dst.X - src.X);
		_dy = Math.Abs((long)dst.Y - src.Y);
		_dz = Math.Abs((long)dstZ - srcZ);
		_sx = src.X < dst.X ? 1 : -1;
		_sy = src.Y < dst.Y ? 1 : -1;
		_sz = srcZ < dstZ ? 1 : -1;
		if (_dx >= _dy && _dx >= _dz)
			_error = _error2 = _dx / 2;
		else if (_dy >= _dx && _dy >= _dz)
			_error = _error2 = _dy / 2;
		else
			_error = _error2 = _dz / 2;

		_first = true;
	}

	public bool Next()
	{
		if (_first)
		{
			_first = false;
			return true;
		}

		if (_dx >= _dy && _dx >= _dz)
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

				_error2 += _dz;
				if (_error2 >= _dx)
				{
					_srcZ = (int)(_srcZ + _sz);
					_error2 -= _dx;
				}

				return true;
			}
		}
		else if (_dy >= _dx && _dy >= _dz)
		{
			if (_src.Y != _dst.Y)
			{
				_src = _src with { Y = (int)(_src.Y + _sy) };
				_error += _dx;
				if (_error >= _dy)
				{
					_src = _src with { X = (int)(_src.X + _sx) };
					_error -= _dy;
				}

				_error2 += _dz;
				if (_error2 >= _dy)
				{
					_srcZ = (int)(_srcZ + _sz);
					_error2 -= _dy;
				}

				return true;
			}
		}
		else if (_srcZ != _dstZ)
		{
			_srcZ = (int)(_srcZ + _sz);
			_error += _dx;
			if (_error >= _dz)
			{
				_src = _src with { X = (int)(_src.X + _sx) };
				_error -= _dz;
			}

			_error2 += _dy;
			if (_error2 >= _dz)
			{
				_src = _src with { Y = (int)(_src.Y + _sy) };
				_error2 -= _dz;
			}

			return true;
		}

		return false;
	}

	public Point2D Point => _src;
	public int Z => _srcZ;

	public override string ToString() => "[" + _src.X + ", " + _src.Y + ", " + _srcZ + "]";
}
