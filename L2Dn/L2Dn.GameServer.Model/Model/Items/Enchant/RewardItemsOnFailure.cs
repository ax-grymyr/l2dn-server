using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Items.Enchant;

/**
 * @author Index
 */
public class RewardItemsOnFailure
{
	private readonly Map<CrystalType, Map<int, ItemChanceHolder>> _rewards = new();
	private int _minEnchantLevel = int.MaxValue;
	private int _maxEnchantLevel = int.MinValue;

	public RewardItemsOnFailure()
	{
	}

	public void addItemToHolder(int itemId, CrystalType grade, int enchantLevel, long count, double chance)
	{
		ItemChanceHolder item = new ItemChanceHolder(itemId, chance, count);
		_rewards.computeIfAbsent(grade, v => new()).put(enchantLevel, item);
		_minEnchantLevel = Math.Min(_minEnchantLevel, enchantLevel);
		_maxEnchantLevel = Math.Max(_maxEnchantLevel, enchantLevel);
	}

	public ItemChanceHolder getRewardItem(CrystalType grade, int enchantLevel)
	{
		return _rewards.getOrDefault(grade, new()).getOrDefault(enchantLevel, null);
	}

	public bool checkIfRewardUnavailable(CrystalType grade, int enchantLevel)
	{
		// reversed available
		if (_minEnchantLevel > enchantLevel)
		{
			return true;
		}

		if (_maxEnchantLevel < enchantLevel)
		{
			return true;
		}

		if (!_rewards.containsKey(grade))
		{
			return true;
		}

		return !_rewards.get(grade).containsKey(enchantLevel);
	}

	public int size()
	{
		int count = 0;
		foreach (Map<int, ItemChanceHolder> rewards in _rewards.values())
		{
			count += rewards.size();
		}

		return count;
	}
}