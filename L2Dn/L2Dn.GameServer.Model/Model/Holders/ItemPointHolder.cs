namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Sdw
 */
public class ItemPointHolder: ItemHolder
{
	private readonly int _points;

	public ItemPointHolder(StatSet @params): this(@params.getInt("id"), @params.getLong("count"),
		@params.getInt("points"))
	{
	}

	public ItemPointHolder(int id, long count, int points): base(id, count)
	{
		_points = points;
	}

	/**
	 * Gets the point.
	 * @return the number of point to get the item
	 */
	public int getPoints()
	{
		return _points;
	}

	public override string ToString()
	{
		return "[" + GetType().Name + "] ID: " + getId() + ", count: " + getCount() + ", points: " + _points;
	}
}