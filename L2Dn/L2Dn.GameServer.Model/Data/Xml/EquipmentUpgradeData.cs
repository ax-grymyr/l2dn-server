using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public sealed class EquipmentUpgradeData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EquipmentUpgradeData));

	private static FrozenDictionary<int, EquipmentUpgradeHolder> _upgrades =
		FrozenDictionary<int, EquipmentUpgradeHolder>.Empty;

	private EquipmentUpgradeData()
	{
		load();
	}

	public void load()
	{
		Dictionary<int, EquipmentUpgradeHolder> upgrades = [];

		XmlEquipmentUpgradeData document =
			LoadXmlDocument<XmlEquipmentUpgradeData>(DataFileLocation.Data, "EquipmentUpgradeData.xml");

		foreach (XmlEquipmentUpgrade xmlEquipmentUpgrade in document.Upgrades)
		{
			int id = xmlEquipmentUpgrade.Id;
			string[] item = xmlEquipmentUpgrade.Item.Split(",");
			if (item.Length != 2 || !int.TryParse(item[0], CultureInfo.InvariantCulture, out int requiredItemId) ||
			    !int.TryParse(item[1], CultureInfo.InvariantCulture, out int requiredItemEnchant))
			{
				_logger.Error(GetType().Name + ": Invalid required item id " + id + ".");
				continue;
			}

			if (ItemData.getInstance().getTemplate(requiredItemId) == null)
			{
				_logger.Error(GetType().Name + ": Required item with id " + requiredItemId + " does not exist.");
				continue;
			}

			List<ItemHolder> materialList = [];
			if (!string.IsNullOrEmpty(xmlEquipmentUpgrade.Materials))
			{
				foreach (string mat in xmlEquipmentUpgrade.Materials.Split(";"))
				{
					string[] matValues = mat.Split(",");
					if (matValues.Length != 2 ||
					    !int.TryParse(matValues[0], CultureInfo.InvariantCulture, out int matItemId)
					    || !long.TryParse(matValues[1], CultureInfo.InvariantCulture, out long matCount))
					{
						_logger.Error(GetType().Name + ": Invalid material list, id " + id + ".");
						continue;
					}

					if (ItemData.getInstance().getTemplate(matItemId) == null)
					{
						_logger.Error(GetType().Name + ": Material item with id " + matItemId + " does not exist.");
					}
					else
					{
						materialList.Add(new ItemHolder(matItemId, matCount));
					}
				}
			}

			string[] resultItem = xmlEquipmentUpgrade.Result.Split(",");
			if (resultItem.Length != 2 || !int.TryParse(item[0], CultureInfo.InvariantCulture, out int resultItemId) ||
			    !int.TryParse(item[1], CultureInfo.InvariantCulture, out int resultItemEnchant))
			{
				_logger.Error(GetType().Name + ": Invalid result item id " + id + ".");
				continue;
			}

			if (ItemData.getInstance().getTemplate(resultItemId) == null)
			{
				_logger.Error(GetType().Name + ": Result item with id " + resultItemId + " does not exist.");
				continue;
			}

			if (!upgrades.TryAdd(id, new EquipmentUpgradeHolder(id, requiredItemId, requiredItemEnchant,
				    materialList.ToImmutableArray(), xmlEquipmentUpgrade.Adena, resultItemId, resultItemEnchant,
				    xmlEquipmentUpgrade.Announce)))
			{
				_logger.Error(GetType().Name + ": Duplicated item id " + id + ".");
			}
		}

		_upgrades = upgrades.ToFrozenDictionary();

		_logger.Info(GetType().Name + ": Loaded " + _upgrades.Count + " upgrade equipment data.");
	}

	public EquipmentUpgradeHolder? getUpgrade(int id)
	{
		return _upgrades.GetValueOrDefault(id);
	}

	public static EquipmentUpgradeData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly EquipmentUpgradeData INSTANCE = new();
	}
}