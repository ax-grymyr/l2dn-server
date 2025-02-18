namespace L2Dn.GameServer.Geo.PathFindings;

public abstract class AbstractNode<Loc>
	where Loc: AbstractNodeLoc
{
	private Loc _loc;
	private AbstractNode<Loc>? _parent;

	public AbstractNode(Loc loc)
	{
		_loc = loc;
	}

	public void setParent(AbstractNode<Loc>? p)
	{
		_parent = p;
	}

	public AbstractNode<Loc>? getParent()
	{
		return _parent;
	}

	public Loc getLoc()
	{
		return _loc;
	}

	public void setLoc(Loc l)
	{
		_loc = l;
	}

	public override int GetHashCode()
	{
		return (31 * 1) + ((_loc == null) ? 0 : _loc.GetHashCode());
	}

	public override bool Equals(object? obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj == null)
		{
			return false;
		}
		if (!(obj is AbstractNode<Loc>))
		{
			return false;
		}

		AbstractNode<Loc> other = (AbstractNode<Loc>) obj;
		if (_loc == null)
		{
			if (other._loc != null)
			{
				return false;
			}
		}
		else if (!_loc.Equals(other._loc))
		{
			return false;
		}
		return true;
	}
}