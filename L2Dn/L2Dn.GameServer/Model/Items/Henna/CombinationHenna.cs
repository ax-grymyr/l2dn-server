using L2Dn.GameServer.Model.Items.Combination;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Items.Henna;

/**
 * @author Index
 */
public class CombinationHenna
{
	private readonly int _henna;
	private readonly int _itemOne;
	private readonly long _countOne;
	private readonly int _itemTwo;
	private readonly long _countTwo;
	private readonly long _commission;
	private readonly float _chance;
	private readonly Map<CombinationItemType, CombinationHennaReward> _rewards = new();

	public CombinationHenna(StatSet set)
	{
		_henna = set.getInt("dyeId");
		_itemOne = set.getInt("itemOne", -1);
		_countOne = set.getLong("countOne", 1);
		_itemTwo = set.getInt("itemTwo", -1);
		_countTwo = set.getLong("countTwo", 1);
		_commission = set.getLong("commission", 0);
		_chance = set.getFloat("chance", 33);
	}

	public int getHenna()
	{
		return _henna;
	}

	public int getItemOne()
	{
		return _itemOne;
	}

	public long getCountOne()
	{
		return _countOne;
	}

	public int getItemTwo()
	{
		return _itemTwo;
	}

	public long getCountTwo()
	{
		return _countTwo;
	}

	public long getCommission()
	{
		return _commission;
	}

	public float getChance()
	{
		return _chance;
	}

	public void addReward(CombinationHennaReward item)
	{
		_rewards.put(item.getType(), item);
	}

	public CombinationHennaReward getReward(CombinationItemType type)
	{
		return _rewards.get(type);
	}
}