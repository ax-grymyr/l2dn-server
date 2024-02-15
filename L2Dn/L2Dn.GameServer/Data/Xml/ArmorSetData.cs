using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads armor set bonuses.
 * @author godson, Luno, UnAfraid
 */
public class ArmorSetData: DataReaderBase
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
		LoadXmlDocuments(DataFileLocation.Data, "stats/armorsets", true).ForEach(t =>
		{
			t.Document.Elements("list").Elements("set").ForEach(x => loadElement(t.FilePath, x));
		});

		int count = _armorSetMap.Keys.Max() + 1;
		_armorSets = new ArmorSet[count];
		foreach (var armorSet in _armorSetMap)
		{
			_armorSets[armorSet.Key] = armorSet.Value;
		}
		
		_itemSets = new List<ArmorSet>[count];
		foreach (var armorSet in _armorSetItems)
		{
			_itemSets[armorSet.Key] = armorSet.Value;
		}
		
		LOGGER.Info(GetType().Name + ": Loaded " + _armorSetMap.size() + " armor sets.");
		_armorSetMap.clear();
		_armorSetItems.clear();
	}

	private void loadElement(string filePath, XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		int minimumPieces = element.Attribute("minimumPieces").GetInt32(0);
		bool isVisual = element.Attribute("visual").GetBoolean(false);
		Set<int> requiredItems = new();
		Set<int> optionalItems = new();
		List<ArmorsetSkillHolder> skills = new();
		Map<BaseStat, double> stats = new();

		element.Elements("requiredItems").ForEach(el =>
		{
			el.Elements("item").ForEach(e =>
			{
				int itemId = e.Attribute("id").GetInt32();
				ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
				if (item == null)
				{
					LOGGER.Warn("Attempting to register non existing required item: " + itemId + " to a set: " +
					            filePath);
				}
				else if (!requiredItems.add(itemId))
				{
					LOGGER.Warn("Attempting to register duplicate required item " + item + " to a set: " + filePath);
				}
			});
		});

		element.Elements("optionalItems").ForEach(el =>
		{
			el.Elements("item").ForEach(e =>
			{
				int itemId = e.Attribute("id").GetInt32();
				ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
				if (item == null)
				{
					LOGGER.Warn("Attempting to register non existing optional item: " + itemId + " to a set: " +
					            filePath);
				}
				else if (!optionalItems.add(itemId))
				{
					LOGGER.Warn("Attempting to register duplicate optional item " + item + " to a set: " + filePath);
				}
			});
		});

		element.Elements("skills").ForEach(el =>
		{
			el.Elements("skill").ForEach(e =>
			{
				int skillId = e.Attribute("id").GetInt32();
				int skillLevel = e.Attribute("level").GetInt32();
				int minPieces = e.Attribute("minimumPieces").GetInt32(minimumPieces);
				int minEnchant = e.Attribute("minimumEnchant").GetInt32(0);
				bool isOptional = e.Attribute("optional").GetBoolean(false);
				int artifactSlotMask = e.Attribute("slotMask").GetInt32(0);
				int artifactBookSlot = e.Attribute("bookSlot").GetInt32(0);
				skills.add(new ArmorsetSkillHolder(skillId, skillLevel, minPieces, minEnchant, isOptional,
					artifactSlotMask, artifactBookSlot));
			});
		});

		element.Elements("stats").ForEach(el =>
		{
			el.Elements("stat").ForEach(e =>
			{
				BaseStat stat = e.Attribute("type").GetEnum<BaseStat>();
				double val = e.Attribute("val").GetDouble();
				stats.put(stat, val);
			});
		});

		ArmorSet set = new ArmorSet(id, minimumPieces, isVisual, requiredItems, optionalItems, skills, stats);
		if (_armorSetMap.putIfAbsent(id, set) != null)
		{
			LOGGER.Warn("Duplicate set entry with id: " + id + " in file: " + filePath);
		}

		set.getRequiredItems().Concat(set.getOptionalItems()).ForEach(itemHolder =>
			_armorSetItems.computeIfAbsent(itemHolder, key => new()).add(set));
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