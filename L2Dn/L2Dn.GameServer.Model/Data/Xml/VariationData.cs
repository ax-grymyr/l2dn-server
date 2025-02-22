using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.DataPack;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Pere, Mobius
 */
public class VariationData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(VariationData));

	private readonly Map<int, Set<int>> _itemGroups = new();
	private readonly Map<int, List<Variation>> _variations = new();
	private readonly Map<int, Map<int, VariationFee>> _fees = new();

	private VariationData()
	{
		load();
	}

	public void load()
	{
		_itemGroups.Clear();
		_variations.Clear();
		_fees.Clear();

		XmlVariationData variationData = LoadXmlDocument<XmlVariationData>(DataFileLocation.Data,
			"stats/augmentation/Variations.xml");

		foreach (XmlVariation variation in variationData.Variations)
			parseVariationElement(variation);

		foreach (XmlVariationItemGroup itemGroup in variationData.ItemGroups)
			parseItemGroupElement(itemGroup);

		foreach (XmlVariationFee fee in variationData.Fees)
			parseFeeElement(fee);

		_logger.Info(GetType().Name + ": Loaded " + _itemGroups.Count + " item groups.");
		_logger.Info(GetType().Name + ": Loaded " + _variations.Count + " variations.");
		_logger.Info(GetType().Name + ": Loaded " + _fees.Count + " fees.");
	}

	private void parseVariationElement(XmlVariation xmlVariation)
	{
		int mineralId = xmlVariation.MineralId;
		int itemGroup = xmlVariation.ItemGroupSpecified ? xmlVariation.ItemGroup : -1;
		if (ItemData.getInstance().getTemplate(mineralId) == null)
		{
			_logger.Error(GetType().Name + ": Mineral with item id " + mineralId + " was not found.");
			return;
		}

		Variation variation = new Variation(mineralId, itemGroup);

		foreach (XmlVariationOptionGroup xmlVariationOptionGroup in xmlVariation.OptionGroups)
		{
			int order = xmlVariationOptionGroup.Order;
			List<OptionDataCategory> sets = new();
			foreach (XmlVariationOptionGroupCategory xmlOptionGroupCategory in xmlVariationOptionGroup.OptionCategories)
			{
				double chance = xmlOptionGroupCategory.Chance;
				Map<Options, double> options = new();

				// Support for specific item ids.
				Set<int> itemIds = new();

				foreach (object xmlItem in xmlOptionGroupCategory.Items)
				{
					switch (xmlItem)
					{
						case XmlVariationOptionGroupCategoryOption option:
						{
							double optionChance = option.Chance;
							int optionId = option.Id;
							Options? opt = OptionData.getInstance().getOptions(optionId);
							if (opt == null)
							{
								_logger.Error(GetType().Name + ": Null option for id " + optionId + " mineral " + mineralId);
								return;
							}

							options.put(opt, optionChance);
							break;
						}

						case XmlVariationOptionGroupCategoryOptionRange optionRange:
						{
							double optionChance = optionRange.Chance;
							int fromId = optionRange.From;
							int toId = optionRange.To;
							for (int id = fromId; id <= toId; id++)
							{
								Options? op = OptionData.getInstance().getOptions(id);
								if (op == null)
								{
									_logger.Error(GetType().Name + ": Null option for id " + id + " mineral " + mineralId);
									return;
								}

								options.put(op, optionChance);
							}

							break;
						}

						case XmlId item:
						{
							itemIds.add(item.Id);
							break;
						}

						case XmlIdRange itemRange:
						{
							int fromId = itemRange.From;
							int toId = itemRange.To;
							for (int id = fromId; id <= toId; id++)
								itemIds.add(id);

							break;
						}
					}
				}

				sets.Add(new OptionDataCategory(options, itemIds, chance));
			}

			variation.setEffectGroup(order, new OptionDataGroup(sets));
		}

		List<Variation>? list = _variations.get(mineralId);
		if (list == null)
		{
			list = new();
		}

		list.Add(variation);

		_variations.put(mineralId, list);

        EtcItem? etcItem = (EtcItem?)ItemData.getInstance().getTemplate(mineralId);
        if (etcItem == null)
        {
            _logger.Error("Mineral with item id " + mineralId + " was not found.");
        }
        else
        {
            etcItem.setMineral();
        }
	}

	private void parseItemGroupElement(XmlVariationItemGroup xmlVariationItemGroup)
	{
		int id = xmlVariationItemGroup.Id;
		Set<int> items = new();
		foreach (XmlId item in xmlVariationItemGroup.Items)
		{
			int itemId = item.Id;
			if (ItemData.getInstance().getTemplate(itemId) == null)
				_logger.Error(GetType().Name + ": Item with id " + itemId + " was not found.");

			items.add(itemId);
		}

		if (_itemGroups.TryGetValue(id, out Set<int>? group))
			group.addAll(items);
		else
			_itemGroups[id] = items;
	}

	private void parseFeeElement(XmlVariationFee xmlVariationFee)
	{
		int itemGroupId = xmlVariationFee.ItemGroup;
		int itemId = xmlVariationFee.ItemId;
		long itemCount = xmlVariationFee.ItemCount;
		long adenaFee = xmlVariationFee.AdenaFee;
		long cancelFee = xmlVariationFee.CancelFee;
		if (itemId != 0 && ItemData.getInstance().getTemplate(itemId) == null)
		{
			_logger.Error(GetType().Name + ": Item with id " + itemId + " was not found.");
		}

		VariationFee fee = new VariationFee(itemId, itemCount, adenaFee, cancelFee);

		Map<int, VariationFee> feeByMinerals = new();
		foreach (object mineral in xmlVariationFee.Minerals)
		{
			switch (mineral)
			{
				case XmlId mineralId:
				{
					feeByMinerals.put(mineralId.Id, fee);
					break;
				}

				case XmlIdRange mineralIdRange:
				{
					int fromId = mineralIdRange.From;
					int toId = mineralIdRange.To;
					for (int id = fromId; id <= toId; id++)
						feeByMinerals.put(id, fee);

					break;
				}
			}
		}

		Set<int>? itemGroup = _itemGroups.GetValueOrDefault(itemGroupId);
		if (itemGroup is null)
		{
			_logger.Error(GetType().Name + ": Item group with id " + itemGroupId + " was not found.");
			return;
		}

		foreach (int item in itemGroup)
		{
			Map<int, VariationFee> fees = _fees.GetValueOrDefault(item) ?? new Map<int, VariationFee>();
			fees.putAll(feeByMinerals);
			_fees.put(item, fees);
		}
	}

	public int getVariationCount()
	{
		return _variations.Count;
	}

	public int getFeeCount()
	{
		return _fees.Count;
	}

	/**
	 * Generate a new random variation instance
	 * @param variation The variation template to generate the variation instance from
	 * @param targetItem The item on which the variation will be applied
	 * @return VariationInstance
	 */
	public VariationInstance generateRandomVariation(Variation variation, Item targetItem)
	{
		return generateRandomVariation(variation, targetItem.getId());
	}

	private VariationInstance generateRandomVariation(Variation variation, int targetItemId)
	{
		Options? option1 = variation.getRandomEffect(0, targetItemId);
		Options? option2 = variation.getRandomEffect(1, targetItemId);
		return new VariationInstance(variation.getMineralId(), option1, option2);
	}

	public Variation? getVariation(int mineralId, Item item)
	{
		List<Variation>? variations = _variations.GetValueOrDefault(mineralId);
		if (variations == null || variations.Count == 0)
			return null;

		foreach (Variation variation in variations)
		{
			Set<int>? group = _itemGroups.GetValueOrDefault(variation.getItemGroup());
			if (group != null && group.Contains(item.getId()))
				return variation;
		}

		return variations[0];
	}

	public bool hasVariation(int mineralId)
	{
		return _variations.GetValueOrDefault(mineralId)?.Count != 0;
	}

	public VariationFee? getFee(int itemId, int mineralId)
	{
		return _fees.GetValueOrDefault(itemId)?.GetValueOrDefault(mineralId);
	}

	public long getCancelFee(int itemId, int mineralId)
	{
		if (!_fees.TryGetValue(itemId, out Map<int, VariationFee>? fees))
			return -1;

		if (!fees.TryGetValue(mineralId, out VariationFee? fee))
		{
			// FIXME This will happen when the data is pre-rework or when augments were manually given, but still that's a cheap solution
			_logger.Warn(GetType().Name + ": Cancellation fee not found for item [" + itemId + "] and mineral [" + mineralId + "]");
			fee = fees.Values.FirstOrDefault();
			if (fee == null)
				return -1;
		}

		return fee.getCancelFee();
	}

	public bool hasFeeData(int itemId)
	{
		return _fees.GetValueOrDefault(itemId)?.Count != 0;
	}

	public static VariationData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly VariationData INSTANCE = new();
	}
}