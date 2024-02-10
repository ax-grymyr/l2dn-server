using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Items.Combination;

/**
 * @author UnAfraid, Mobius
 */
public class CombinationItem
{
	private readonly int _itemOne;
	private readonly int _enchantOne;
	private readonly int _itemTwo;
	private readonly int _enchantTwo;
	private readonly long _commission;
	private readonly float _chance;
	private readonly bool _announce;
	private readonly Map<CombinationItemType, CombinationItemReward> _rewards = new();

	public CombinationItem(StatSet set)
	{
		_itemOne = set.getInt("one");
		_enchantOne = set.getInt("enchantOne", 0);
		_itemTwo = set.getInt("two");
		_enchantTwo = set.getInt("enchantTwo", 0);
		_commission = set.getLong("commission", 0);
		_chance = set.getFloat("chance", 33);
		_announce = set.getBoolean("announce", false);
	}

	public int getItemOne()
	{
		return _itemOne;
	}

	public int getEnchantOne()
	{
		return _enchantOne;
	}

	public int getItemTwo()
	{
		return _itemTwo;
	}

	public int getEnchantTwo()
	{
		return _enchantTwo;
	}

	public long getCommission()
	{
		return _commission;
	}

	public float getChance()
	{
		return _chance;
	}

	public bool isAnnounce()
	{
		return _announce;
	}

	public void addReward(CombinationItemReward item)
	{
		_rewards.put(item.getType(), item);
	}

	public CombinationItemReward getReward(CombinationItemType type)
	{
		return _rewards.get(type);
	}
}