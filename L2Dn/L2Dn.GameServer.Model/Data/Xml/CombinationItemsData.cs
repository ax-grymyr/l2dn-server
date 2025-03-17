using System.Collections.Immutable;
using L2Dn.GameServer.Model.Items.Combination;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public sealed class CombinationItemsData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(CombinationItemsData));
	private ImmutableArray<CombinationItem> _items = [];

	private CombinationItemsData()
	{
		load();
	}

	public void load()
	{
		List<CombinationItem> items = [];

		XmlCombinationItems document =
			LoadXmlDocument<XmlCombinationItems>(DataFileLocation.Data, "CombinationItems.xml");

		foreach (XmlCombinationItem combinationItem in document.Items)
		{
			if (!CheckItem(combinationItem.One) || !CheckItem(combinationItem.Two))
				continue;

			CombinationItemReward? rewardOnSuccess = null;
			CombinationItemReward? rewardOnFailure = null;

			foreach (XmlCombinationItemReward combinationItemReward in combinationItem.Rewards)
			{
				if (!CheckItem(combinationItemReward.Id))
					continue;

				if (combinationItemReward.Type != CombinationItemType.ON_SUCCESS &&
				    combinationItemReward.Type != CombinationItemType.ON_FAILURE)
				{
					_logger.Error(nameof(CombinationItemsData) + ": Invalid reward type for item combination " +
						combinationItem.One + " and " + combinationItem.Two);

					continue;
				}

				CombinationItemReward reward = new(combinationItemReward.Id, combinationItemReward.Count,
					combinationItemReward.Type == CombinationItemType.ON_SUCCESS, combinationItemReward.Enchant);

				if (reward.OnSuccess)
				{
					if (rewardOnSuccess is not null)
					{
						_logger.Error(nameof(CombinationItemsData) + ": Duplicated reward on success for item combination " +
							combinationItem.One + " and " + combinationItem.Two);

						continue;
					}

					rewardOnSuccess = reward;
				}
				else
				{
					if (rewardOnFailure is not null)
					{
						_logger.Error(nameof(CombinationItemsData) + ": Duplicated reward on failure for item combination " +
							combinationItem.One + " and " + combinationItem.Two);

						continue;
					}

					rewardOnFailure = reward;
				}
			}

			if (rewardOnSuccess is null)
			{
				_logger.Error(nameof(CombinationItemsData) + ": Missing reward on success for item combination " +
					combinationItem.One + " and " + combinationItem.Two);

				continue;
			}

			if (rewardOnFailure is null)
			{
				_logger.Error(nameof(CombinationItemsData) + ": Missing reward on failure for item combination " +
					combinationItem.One + " and " + combinationItem.Two);

				continue;
			}

			CombinationItem item = new(combinationItem.One, combinationItem.EnchantOne, combinationItem.Two,
				combinationItem.EnchantTwo, combinationItem.Commission, combinationItem.Chance,
				combinationItem.Announce, rewardOnSuccess, rewardOnFailure);

			items.Add(item);
		}

		_items = items.ToImmutableArray();

		_logger.Info(GetType().Name + ": Loaded " + _items.Length + " combinations.");
	}

	private static bool CheckItem(int itemId)
	{
		if (ItemData.getInstance().getTemplate(itemId) == null)
		{
			_logger.Error(nameof(CombinationItemsData) + ": Could not find item with id " + itemId);
			return false;
		}

		return true;
	}

	public CombinationItem? getItemsBySlots(int firstSlot, int enchantOne, int secondSlot, int enchantTwo)
	{
		foreach (CombinationItem item in _items)
		{
			if (item.getItemOne() == firstSlot && item.getItemTwo() == secondSlot &&
			    item.getEnchantOne() == enchantOne && item.getEnchantTwo() == enchantTwo)
			{
				return item;
			}
		}

		return null;
	}

	public ImmutableArray<CombinationItem> getItemsByFirstSlot(int id, int enchantOne)
	{
		return _items.Where(item => item.getItemOne() == id && item.getEnchantOne() == enchantOne).ToImmutableArray();
	}

	public static CombinationItemsData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly CombinationItemsData INSTANCE = new();
	}
}