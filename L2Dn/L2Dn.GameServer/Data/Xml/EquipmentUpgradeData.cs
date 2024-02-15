using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class EquipmentUpgradeData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EquipmentUpgradeData));
	private static readonly Map<int, EquipmentUpgradeHolder> _upgrades = new();
	
	protected EquipmentUpgradeData()
	{
		load();
	}
	
	public void load()
	{
		_upgrades.clear();
			
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "EquipmentUpgradeData.xml");
		document.Elements("list").Elements("upgrade").ForEach(parseElement);
			
		LOGGER.Info(GetType().Name + ": Loaded " + _upgrades.size() + " upgrade equipment data.");
	}

	private void parseElement(XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		string[] item = element.Attribute("item").GetString().Split(",");
		int requiredItemId = int.Parse(item[0]);
		int requiredItemEnchant = int.Parse(item[1]);
		string materials = element.Attribute("materials").GetString("");
		List<ItemHolder> materialList = new();
		bool announce = element.Attribute("announce").GetBoolean(false);
		if (!materials.isEmpty())
		{
			foreach (String mat in materials.Split(";"))
			{
				String[] matValues = mat.Split(",");
				int matItemId = int.Parse(matValues[0]);
				if (ItemData.getInstance().getTemplate(matItemId) == null)
				{
					LOGGER.Info(GetType().Name + ": Material item with id " + matItemId + " does not exist.");
				}
				else
				{
					materialList.add(new ItemHolder(matItemId, long.Parse(matValues[1])));
				}
			}
		}

		long adena = element.Attribute("adena").GetInt64(0);
		String[] resultItem = element.Attribute("result").GetString().Split(",");
		int resultItemId = int.Parse(resultItem[0]);
		int resultItemEnchant = int.Parse(resultItem[1]);
		if (ItemData.getInstance().getTemplate(requiredItemId) == null)
		{
			LOGGER.Info(GetType().Name + ": Required item with id " + requiredItemId + " does not exist.");
		}
		else
		{
			_upgrades.put(id,
				new EquipmentUpgradeHolder(id, requiredItemId, requiredItemEnchant, materialList, adena, resultItemId,
					resultItemEnchant, announce));
		}
	}

	public EquipmentUpgradeHolder getUpgrade(int id)
	{
		return _upgrades.get(id);
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