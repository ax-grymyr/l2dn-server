using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model.Holders;

/**
 * A simple DTO for items; contains item ID and count.
 * @author UnAfraid
 */
public class ItemHolder(int id, long count): IIdentifiable
{
	/**
	 * @return the ID of the item contained in this object
	 */
	public int getId() => id;

	/**
	 * @return the count of items contained in this object
	 */
	public long getCount() => count;

	public override bool Equals(object? obj)
		=> obj is ItemHolder other && id == other.getId() && count == other.getCount();

	public override string ToString() => $"[{GetType().Name}] ID: {id}, count: {count}";
}