using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.DataPack;
using L2Dn.Model.Enums;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads armor set bonuses.
 * @author godson, Luno, UnAfraid
 */
public class ArmorSetData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ArmorSetData));
	
	private ArmorSet[] _armorSets;
	private readonly Map<int, ArmorSet> _armorSetMap = new();
	private List<ArmorSet>[] _itemSets;
	private readonly Map<int, List<ArmorSet>> _armorSetItems = new();
	
	private ArmorSetData()
	{
		load();
	}
	
	public void load()
	{
		LoadXmlDocuments<XmlArmorSetList>(DataFileLocation.Data, "stats/armorsets", true)
			.SelectMany(t => t.Document.Sets.Select(s => (t.FilePath, Set: s)))
			.ForEach(t => LoadArmorSet(t.FilePath, t.Set));

		int count = _armorSetMap.Keys.Max() + 1;
		_armorSets = new ArmorSet[count];
		foreach (var armorSet in _armorSetMap)
		{
			_armorSets[armorSet.Key] = armorSet.Value;
		}
		
		count = _armorSetItems.Keys.Max() + 1;
		_itemSets = new List<ArmorSet>[count];
		foreach (var armorSet in _armorSetItems)
		{
			_itemSets[armorSet.Key] = armorSet.Value;
		}
		
		_logger.Info(GetType().Name + ": Loaded " + _armorSetMap.size() + " armor sets.");
		_armorSetMap.clear();
		_armorSetItems.clear();
	}

	private void LoadArmorSet(string filePath, XmlArmorSet xmlArmorSet)
	{
		int id = xmlArmorSet.Id;
		int minimumPieces = xmlArmorSet.MinimumPieces;
		bool isVisual = xmlArmorSet.Visual;
		Set<int> requiredItems = new();
		Set<int> optionalItems = new();
		List<ArmorsetSkillHolder> skills = new();
		Map<BaseStat, double> stats = new();

		foreach (XmlArmorSetItem xmlArmorSetItem in xmlArmorSet.RequiredItems)
		{
			int itemId = xmlArmorSetItem.Id;
			ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
			if (item == null)
			{
				_logger.Warn("Attempting to register non existing required item: " + itemId + " to a set: " +
				             filePath);
			}
			else if (!requiredItems.add(itemId))
				_logger.Warn("Attempting to register duplicate required item " + item + " to a set: " + filePath);
		}

		foreach (XmlArmorSetItem xmlArmorSetItem in xmlArmorSet.OptionalItems)
		{
			int itemId = xmlArmorSetItem.Id;
			ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
			if (item == null)
			{
				_logger.Warn("Attempting to register non existing optional item: " + itemId + " to a set: " +
				             filePath);
			}
			else if (!optionalItems.add(itemId))
				_logger.Warn("Attempting to register duplicate optional item " + item + " to a set: " + filePath);
		}

		foreach (XmlArmorSetSkill xmlArmorSetSkill in xmlArmorSet.Skills)
		{
			skills.add(new ArmorsetSkillHolder(xmlArmorSetSkill.SkillId, xmlArmorSetSkill.SkillLevel,
				xmlArmorSetSkill.MinimumPieces, xmlArmorSetSkill.MinimumEnchant, xmlArmorSetSkill.Optional,
				xmlArmorSetSkill.SlotMask, xmlArmorSetSkill.BookSlot));
		}

		foreach (XmlArmorSetStat xmlArmorSetStat in xmlArmorSet.Stats)
			stats[xmlArmorSetStat.Stat] = xmlArmorSetStat.Value;

		ArmorSet set = new ArmorSet(id, minimumPieces, isVisual, requiredItems, optionalItems, skills, stats);
		if (!_armorSetMap.TryAdd(id, set))
			_logger.Warn("Duplicate set entry with id: " + id + " in file: " + filePath);

		set.getRequiredItems().Concat(set.getOptionalItems()).ForEach(itemHolder =>
			_armorSetItems.computeIfAbsent(itemHolder, key => new()).add(set));
	}

	/**
	 * @param setId the set id that is attached to a set
	 * @return the armor set associated to the given item id
	 */
	public ArmorSet? getSet(int setId)
	{
		if (setId >= 0 && setId < _armorSets.Length)
			return _armorSets[setId];
	
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
		public static readonly ArmorSetData INSTANCE = new();
	}
}