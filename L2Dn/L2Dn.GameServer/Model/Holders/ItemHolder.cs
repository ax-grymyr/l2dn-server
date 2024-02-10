using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model.Holders;

/**
 * A simple DTO for items; contains item ID and count.
 * @author UnAfraid
 */
public class ItemHolder: IIdentifiable
{
	private readonly int _id;
	private readonly long _count;

	public ItemHolder(StatSet set)
	{
		_id = set.getInt("id");
		_count = set.getLong("count");
	}

	public ItemHolder(int id, long count)
	{
		_id = id;
		_count = count;
	}

	/**
	 * @return the ID of the item contained in this object
	 */
	public int getId()
	{
		return _id;
	}

	/**
	 * @return the count of items contained in this object
	 */
	public long getCount()
	{
		return _count;
	}

	public override bool Equals(Object? obj)
	{
		if (!(obj is ItemHolder))
		{
			return false;
		}
		else if (obj == this)
		{
			return true;
		}

		ItemHolder objInstance = (ItemHolder)obj;
		return (_id == objInstance.getId()) && (_count == objInstance.getCount());
	}

	public override String ToString()
	{
		return "[" + GetType().Name + "] ID: " + _id + ", count: " + _count;
	}
}