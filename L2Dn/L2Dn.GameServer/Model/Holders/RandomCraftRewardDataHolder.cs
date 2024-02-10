namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mode
 */
public class RandomCraftRewardDataHolder
{
	private readonly int _itemId;
	private readonly long _count;
	private readonly double _chance;
	private readonly bool _announce;

	public RandomCraftRewardDataHolder(int itemId, long count, double chance, bool announce)
	{
		_itemId = itemId;
		_count = count;
		_chance = chance;
		_announce = announce;
	}

	public int getItemId()
	{
		return _itemId;
	}

	public long getCount()
	{
		return _count;
	}

	public double getChance()
	{
		return _chance;
	}

	public bool isAnnounce()
	{
		return _announce;
	}
}