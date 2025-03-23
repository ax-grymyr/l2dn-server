using L2Dn.GameServer.Dto;
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
		ItemChanceHolder item = new ItemChanceHolder(itemId, count, chance);
		_rewards.GetOrAdd(grade, _ => []).put(enchantLevel, item);
		_minEnchantLevel = Math.Min(_minEnchantLevel, enchantLevel);
		_maxEnchantLevel = Math.Max(_maxEnchantLevel, enchantLevel);
	}

	public ItemChanceHolder? getRewardItem(CrystalType grade, int enchantLevel)
	{
		return _rewards.GetValueOrDefault(grade)?.GetValueOrDefault(enchantLevel);
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

		if (!_rewards.TryGetValue(grade, out Map<int, ItemChanceHolder>? holders))
		{
			return true;
		}

		return !holders.ContainsKey(enchantLevel);
	}

	public int size()
	{
		return _rewards.Values.Sum(rewards => rewards.Count);
	}
}