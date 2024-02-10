using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads armor set bonuses.
 * @author godson, Luno, UnAfraid
 */
public class ArmorSetData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ArmorSetData));
	
	private ArmorSet[] _armorSets;
	private readonly Map<int, ArmorSet> _armorSetMap = new();
	private List<ArmorSet>[] _itemSets;
	private readonly Map<int, List<ArmorSet>> _armorSetItems = new();
	
	protected ArmorSetData()
	{
		load();
	}
	
	public void load()
	{
		parseDatapackDirectory("data/stats/armorsets", false);
		
		_armorSets = new ArmorSet[Collections.max(_armorSetMap.Keys) + 1];
		foreach (var armorSet in _armorSetMap)
		{
			_armorSets[armorSet.Key] = armorSet.Value;
		}
		
		_itemSets = new();
		foreach (var armorSet in _armorSetItems)
		{
			_itemSets[armorSet.Key] = armorSet.Value;
		}
		
		LOGGER.Info(GetType().Name + ": Loaded " + _armorSetMap.size() + " armor sets.");
		_armorSetMap.clear();
		_armorSetItems.clear();
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node setNode = n.getFirstChild(); setNode != null; setNode = setNode.getNextSibling())
				{
					if ("set".equalsIgnoreCase(setNode.getNodeName()))
					{
						int id = parseInteger(setNode.getAttributes(), "id");
						int minimumPieces = parseInteger(setNode.getAttributes(), "minimumPieces", 0);
						bool isVisual = parseBoolean(setNode.getAttributes(), "visual", false);
						Set<int> requiredItems = new LinkedHashSet<>();
						Set<int> optionalItems = new LinkedHashSet<>();
						List<ArmorsetSkillHolder> skills = new();
						Map<BaseStat, Double> stats = new();
						for (Node innerSetNode = setNode.getFirstChild(); innerSetNode != null; innerSetNode = innerSetNode.getNextSibling())
						{
							switch (innerSetNode.getNodeName())
							{
								case "requiredItems":
								{
									forEach(innerSetNode, b => "item".equals(b.getNodeName()), node =>
									{
										NamedNodeMap attrs = node.getAttributes();
										int itemId = parseInteger(attrs, "id");
										ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
										if (item == null)
										{
											LOGGER.Warn("Attempting to register non existing required item: " + itemId + " to a set: " + f.getName());
										}
										else if (!requiredItems.add(itemId))
										{
											LOGGER.Warn("Attempting to register duplicate required item " + item + " to a set: " + f.getName());
										}
									});
									break;
								}
								case "optionalItems":
								{
									forEach(innerSetNode, b => "item".equals(b.getNodeName()), node =>
									{
										NamedNodeMap attrs = node.getAttributes();
										int itemId = parseInteger(attrs, "id");
										ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
										if (item == null)
										{
											LOGGER.Warn("Attempting to register non existing optional item: " + itemId + " to a set: " + f.getName());
										}
										else if (!optionalItems.add(itemId))
										{
											LOGGER.Warn("Attempting to register duplicate optional item " + item + " to a set: " + f.getName());
										}
									});
									break;
								}
								case "skills":
								{
									forEach(innerSetNode, b => "skill".equals(b.getNodeName()), node =>
									{
										NamedNodeMap attrs = node.getAttributes();
										int skillId = parseInteger(attrs, "id");
										int skillLevel = parseInteger(attrs, "level");
										int minPieces = parseInteger(attrs, "minimumPieces", minimumPieces);
										int minEnchant = parseInteger(attrs, "minimumEnchant", 0);
										bool isOptional = parseBoolean(attrs, "optional", false);
										int artifactSlotMask = parseInteger(attrs, "slotMask", 0);
										int artifactBookSlot = parseInteger(attrs, "bookSlot", 0);
										skills.add(new ArmorsetSkillHolder(skillId, skillLevel, minPieces, minEnchant, isOptional, artifactSlotMask, artifactBookSlot));
									});
									break;
								}
								case "stats":
								{
									forEach(innerSetNode, b => "stat".equals(b.getNodeName()), node =>
									{
										NamedNodeMap attrs = node.getAttributes();
										stats.put(parseEnum(attrs, BaseStat.class, "type"), parseDouble(attrs, "val"));
									});
									break;
								}
							}
						}
						
						ArmorSet set = new ArmorSet(id, minimumPieces, isVisual, requiredItems, optionalItems, skills, stats);
						if (_armorSetMap.putIfAbsent(id, set) != null)
						{
							LOGGER.Warn("Duplicate set entry with id: " + id + " in file: " + f.getName());
						}
						
						Stream.concat(Arrays.stream(set.getRequiredItems()).boxed(), Arrays.stream(set.getOptionalItems()).boxed()).forEach(itemHolder => _armorSetItems.computeIfAbsent(itemHolder, key => new()).add(set));
					}
				}
			}
		}
	}
	
	/**
	 * @param setId the set id that is attached to a set
	 * @return the armor set associated to the given item id
	 */
	public ArmorSet getSet(int setId)
	{
		if (_armorSets.Length > setId)
		{
			return _armorSets[setId];
		}
		return null;
	}
	
	/**
	 * @param itemId the item id that is attached to a set
	 * @return the armor set associated to the given item id
	 */
	public List<ArmorSet> getSets(int itemId)
	{
		if (_itemSets.Length > itemId)
		{
			List<ArmorSet> sets = _itemSets[itemId];
			if (sets != null)
			{
				return sets;
			}
		}
		return new();
	}
	
	/**
	 * Gets the single instance of ArmorSetsData
	 * @return single instance of ArmorSetsData
	 */
	public static ArmorSetData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ArmorSetData INSTANCE = new ArmorSetData();
	}
}