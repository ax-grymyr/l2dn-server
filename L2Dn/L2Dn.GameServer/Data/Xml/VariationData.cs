using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Pere, Mobius
 */
public class VariationData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(VariationData.class.getSimpleName());
	
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
		parseDatapackFile("data/stats/augmentation/Variations.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _itemGroups.size() + " item groups.");
		LOGGER.Info(GetType().Name + ": Loaded " + _variations.size() + " variations.");
		LOGGER.Info(GetType().Name + ": Loaded " + _fees.size() + " fees.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode =>
		{
			forEach(listNode, "variations", variationsNode => forEach(variationsNode, "variation", variationNode =>
			{
				int mineralId = parseInteger(variationNode.getAttributes(), "mineralId");
				int itemGroup = parseInteger(variationNode.getAttributes(), "itemGroup", -1);
				if (ItemData.getInstance().getTemplate(mineralId) == null)
				{
					LOGGER.Warn(GetType().Name + ": Mineral with item id " + mineralId + " was not found.");
				}
				Variation variation = new Variation(mineralId, itemGroup);
				
				forEach(variationNode, "optionGroup", groupNode =>
				{
					int order = parseInteger(groupNode.getAttributes(), "order");
					List<OptionDataCategory> sets = new();
					forEach(groupNode, "optionCategory", categoryNode =>
					{
						double chance = parseDouble(categoryNode.getAttributes(), "chance");
						Map<Options, Double> options = new();
						forEach(categoryNode, "option", optionNode =>
						{
							double optionChance = parseDouble(optionNode.getAttributes(), "chance");
							int optionId = parseInteger(optionNode.getAttributes(), "id");
							Options opt = OptionData.getInstance().getOptions(optionId);
							if (opt == null)
							{
								LOGGER.Warn(GetType().Name + ": Null option for id " + optionId + " mineral " + mineralId);
								return;
							}
							options.put(opt, optionChance);
						});
						forEach(categoryNode, "optionRange", optionNode =>
						{
							double optionChance = parseDouble(optionNode.getAttributes(), "chance");
							int fromId = parseInteger(optionNode.getAttributes(), "from");
							int toId = parseInteger(optionNode.getAttributes(), "to");
							for (int id = fromId; id <= toId; id++)
							{
								Options op = OptionData.getInstance().getOptions(id);
								if (op == null)
								{
									LOGGER.Warn(GetType().Name + ": Null option for id " + id + " mineral " + mineralId);
									return;
								}
								options.put(op, optionChance);
							}
						});
						
						// Support for specific item ids.
						Set<int> itemIds = new();
						forEach(categoryNode, "item", optionNode =>
						{
							int itemId = parseInteger(optionNode.getAttributes(), "id");
							itemIds.add(itemId);
						});
						forEach(categoryNode, "items", optionNode =>
						{
							int fromId = parseInteger(optionNode.getAttributes(), "from");
							int toId = parseInteger(optionNode.getAttributes(), "to");
							for (int id = fromId; id <= toId; id++)
							{
								itemIds.add(id);
							}
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
				((EtcItem) ItemData.getInstance().getTemplate(mineralId)).setMineral();
			}));
			
			forEach(listNode, "itemGroups", variationsNode => forEach(variationsNode, "itemGroup", variationNode =>
			{
				int id = parseInteger(variationNode.getAttributes(), "id");
				Set<int> items = new();
				forEach(variationNode, "item", itemNode =>
				{
					int itemId = parseInteger(itemNode.getAttributes(), "id");
					if (ItemData.getInstance().getTemplate(itemId) == null)
					{
						LOGGER.Warn(GetType().Name + ": Item with id " + itemId + " was not found.");
					}
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
			}));
			
			forEach(listNode, "fees", variationNode => forEach(variationNode, "fee", feeNode =>
			{
				int itemGroupId = parseInteger(feeNode.getAttributes(), "itemGroup");
				Set<int> itemGroup = _itemGroups.get(itemGroupId);
				int itemId = parseInteger(feeNode.getAttributes(), "itemId", 0);
				long itemCount = Parse(feeNode.getAttributes(), "itemCount", 0L);
				long adenaFee = Parse(feeNode.getAttributes(), "adenaFee", 0L);
				long cancelFee = Parse(feeNode.getAttributes(), "cancelFee", 0L);
				if ((itemId != 0) && (ItemData.getInstance().getTemplate(itemId) == null))
				{
					LOGGER.Warn(GetType().Name + ": Item with id " + itemId + " was not found.");
				}
				
				VariationFee fee = new VariationFee(itemId, itemCount, adenaFee, cancelFee);
				Map<int, VariationFee> feeByMinerals = new();
				forEach(feeNode, "mineral", mineralNode =>
				{
					int mId = parseInteger(mineralNode.getAttributes(), "id");
					feeByMinerals.put(mId, fee);
				});
				forEach(feeNode, "mineralRange", mineralNode =>
				{
					int fromId = parseInteger(mineralNode.getAttributes(), "from");
					int toId = parseInteger(mineralNode.getAttributes(), "to");
					for (int id = fromId; id <= toId; id++)
					{
						feeByMinerals.put(id, fee);
					}
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
			}));
		});
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
		{
			return -1;
		}
		
		VariationFee fee = fees.get(mineralId);
		if (fee == null)
		{
			// FIXME This will happen when the data is pre-rework or when augments were manually given, but still that's a cheap solution
			LOGGER.Warn(GetType().Name + ": Cancellation fee not found for item [" + itemId + "] and mineral [" + mineralId + "]");
			fee = fees.values().iterator().next();
			if (fee == null)
			{
				return -1;
			}
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