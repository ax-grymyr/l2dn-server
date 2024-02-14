using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Pere, Mobius
 */
public class VariationData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(VariationData));
	
	private readonly Map<int, Set<int>> _itemGroups = new();
	private readonly Map<int, List<Variation>> _variations = new();
	private readonly Map<int, Map<int, VariationFee>> _fees = new();
	
	protected VariationData()
	{
		load();
	}
	
	public void load()
	{
		_itemGroups.clear();
		_variations.clear();
		_fees.clear();

		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/stats/augmentation/Variations.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("variations").Elements("variation").ForEach(parseVariationElement);
		document.Elements("list").Elements("itemGroups").Elements("itemGroup").ForEach(parseItemGroupElement);
		document.Elements("list").Elements("fees").Elements("fee").ForEach(parseFeeElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _itemGroups.size() + " item groups.");
		LOGGER.Info(GetType().Name + ": Loaded " + _variations.size() + " variations.");
		LOGGER.Info(GetType().Name + ": Loaded " + _fees.size() + " fees.");
	}

	private void parseVariationElement(XElement element)
	{
		int mineralId = element.Attribute("mineralId").GetInt32();
		int itemGroup = element.Attribute("itemGroup").GetInt32(-1);
		if (ItemData.getInstance().getTemplate(mineralId) == null)
		{
			LOGGER.Error(GetType().Name + ": Mineral with item id " + mineralId + " was not found.");
		}

		Variation variation = new Variation(mineralId, itemGroup);

		element.Elements("optionGroup").ForEach(el =>
		{
			int order = el.Attribute("order").GetInt32();
			List<OptionDataCategory> sets = new();
			el.Elements("optionCategory").ForEach(e =>
			{
				double chance = e.Attribute("chance").GetDouble();
				Map<Options, Double> options = new();
				e.Elements("option").ForEach(optEl =>
				{
					double optionChance = optEl.Attribute("chance").GetDouble();
					int optionId = optEl.Attribute("id").GetInt32();
					Options opt = OptionData.getInstance().getOptions(optionId);
					if (opt == null)
					{
						LOGGER.Error(GetType().Name + ": Null option for id " + optionId + " mineral " + mineralId);
						return;
					}

					options.put(opt, optionChance);
				});

				e.Elements("optionRange").ForEach(optEl =>
				{
					double optionChance = optEl.Attribute("chance").GetDouble();
					int fromId = optEl.Attribute("from").GetInt32();
					int toId = optEl.Attribute("to").GetInt32();
					for (int id = fromId; id <= toId; id++)
					{
						Options op = OptionData.getInstance().getOptions(id);
						if (op == null)
						{
							LOGGER.Error(GetType().Name + ": Null option for id " + id + " mineral " + mineralId);
							return;
						}

						options.put(op, optionChance);
					}
				});

				// Support for specific item ids.
				Set<int> itemIds = new();
				e.Elements("item").ForEach(itemEl =>
				{
					int itemId = itemEl.Attribute("id").GetInt32();
					itemIds.add(itemId);
				});

				e.Elements("items").ForEach(itemEl =>
				{
					int fromId = itemEl.Attribute("from").GetInt32();
					int toId = itemEl.Attribute("to").GetInt32();
					for (int id = fromId; id <= toId; id++)
						itemIds.add(id);
				});

				sets.add(new OptionDataCategory(options, itemIds, chance));
			});

			variation.setEffectGroup(order, new OptionDataGroup(sets));
		});

		List<Variation> list = _variations.get(mineralId);
		if (list == null)
		{
			list = new();
		}

		list.add(variation);

		_variations.put(mineralId, list);
		((EtcItem)ItemData.getInstance().getTemplate(mineralId)).setMineral();
	}

	private void parseItemGroupElement(XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		Set<int> items = new();
		element.Elements("item").ForEach(el =>
		{
			int itemId = el.Attribute("id").GetInt32();
			if (ItemData.getInstance().getTemplate(itemId) == null)
				LOGGER.Error(GetType().Name + ": Item with id " + itemId + " was not found.");
			
			items.add(itemId);
		});
				
		if (_itemGroups.containsKey(id))
		{
			_itemGroups.get(id).addAll(items);
		}
		else
		{
			_itemGroups.put(id, items);
		}
	}

	private void parseFeeElement(XElement element)
	{
		int itemGroupId = element.Attribute("itemGroup").GetInt32();
		Set<int> itemGroup = _itemGroups.get(itemGroupId);
		int itemId = element.Attribute("itemId").GetInt32(0);
		long itemCount = element.Attribute("itemCount").GetInt64(0);
		long adenaFee = element.Attribute("adenaFee").GetInt64(0);
		long cancelFee = element.Attribute("cancelFee").GetInt64(0);
		if (itemId != 0 && (ItemData.getInstance().getTemplate(itemId) == null))
		{
			LOGGER.Error(GetType().Name + ": Item with id " + itemId + " was not found.");
		}
				
		VariationFee fee = new VariationFee(itemId, itemCount, adenaFee, cancelFee);
		Map<int, VariationFee> feeByMinerals = new();
		element.Elements("mineral").ForEach(el =>
		{
			int mId = el.Attribute("id").GetInt32();
			feeByMinerals.put(mId, fee);
		});

		element.Elements("mineralRange").ForEach(el =>
		{
			int fromId = el.Attribute("from").GetInt32();
			int toId = el.Attribute("to").GetInt32();
			for (int id = fromId; id <= toId; id++)
				feeByMinerals.put(id, fee);
		});

		foreach (int item in itemGroup)
		{
			Map<int, VariationFee> fees = _fees.get(item);
			if (fees == null)
			{
				fees = new();
			}
			fees.putAll(feeByMinerals);
			_fees.put(item, fees);
		}
	}
	
	public int getVariationCount()
	{
		return _variations.size();
	}
	
	public int getFeeCount()
	{
		return _fees.size();
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
		Options option1 = variation.getRandomEffect(0, targetItemId);
		Options option2 = variation.getRandomEffect(1, targetItemId);
		return new VariationInstance(variation.getMineralId(), option1, option2);
	}
	
	public Variation getVariation(int mineralId, Item item)
	{
		List<Variation> variations = _variations.get(mineralId);
		if ((variations == null) || variations.isEmpty())
		{
			return null;
		}
		
		foreach (Variation variation in variations)
		{
			Set<int> group = _itemGroups.get(variation.getItemGroup());
			if ((group != null) && group.Contains(item.getId()))
			{
				return variation;
			}
		}
		
		return variations.get(0);
	}
	
	public bool hasVariation(int mineralId)
	{
		List<Variation> variations = _variations.get(mineralId);
		return (variations != null) && !variations.isEmpty();
	}
	
	public VariationFee getFee(int itemId, int mineralId)
	{
		return _fees.getOrDefault(itemId, new()).get(mineralId);
	}
	
	public long getCancelFee(int itemId, int mineralId)
	{
		Map<int, VariationFee> fees = _fees.get(itemId);
		if (fees == null)
			return -1;
		
		VariationFee fee = fees.get(mineralId);
		if (fee == null)
		{
			// FIXME This will happen when the data is pre-rework or when augments were manually given, but still that's a cheap solution
			LOGGER.Warn(GetType().Name + ": Cancellation fee not found for item [" + itemId + "] and mineral [" + mineralId + "]");
			fee = fees.values().FirstOrDefault();
			if (fee == null)
				return -1;
		}
		
		return fee.getCancelFee();
	}
	
	public bool hasFeeData(int itemId)
	{
		Map<int, VariationFee> itemFees = _fees.get(itemId);
		return (itemFees != null) && !itemFees.isEmpty();
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