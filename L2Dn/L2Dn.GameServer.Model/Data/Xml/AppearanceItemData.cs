using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class AppearanceItemData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AppearanceItemData));
	
	private readonly Map<int, AppearanceStone> _stoneMap = new();
	
	protected AppearanceItemData()
	{
		load();
	}
	
	public void load()
	{
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "AppearanceStones.xml");
		document.Elements("list").Elements("appearance_stone").ForEach(loadElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _stoneMap.size() + " stones.");
		
		//@formatter:off
		/*
		foreach (Item item in ItemData.getInstance().getAllItems())
		{
			if ((item == null) || !item.getName().contains("Appearance Stone"))
			{
				continue;
			}
			if (item.getName().contains("Pack") || _stones.containsKey(item.getId()))
			{
				continue;
			}
			
			System.out.println("Unhandled appearance stone: " + item);
		}
		*/
		//@formatter:on
		
		_stoneMap.clear();
	}

	private void loadElement(XElement element)
	{
		AppearanceStone stone = new AppearanceStone(element);
		
		element.Elements("grade").ForEach(el =>
		{
			CrystalType type = Enum.Parse<CrystalType>(el.Value);
			stone.addCrystalType(type);
		});
		
		element.Elements("targetType").ForEach(el =>
		{
			AppearanceTargetType type = Enum.Parse<AppearanceTargetType>(el.Value);
			stone.addTargetType(type);
		});
		
		element.Elements("bodyPart").ForEach(el =>
		{
			long part = ItemData.SLOTS.get(el.Value);
			stone.addBodyPart(part);
		});
		
		element.Elements("race").ForEach(el =>
		{
			Race race = Enum.Parse<Race>(el.Value);
			stone.addRace(race);
		});
		
		element.Elements("raceNot").ForEach(el =>
		{
			Race raceNot = Enum.Parse<Race>(el.Value);
			stone.addRaceNot(raceNot);
		});

		element.Elements("visual").ForEach(el =>
		{
			stone.addVisualId(new AppearanceHolder(el));
		});

		if (ItemData.getInstance().getTemplate(stone.getId()) != null)
		{
			_stoneMap.put(stone.getId(), stone);
		}
		else
		{
			LOGGER.Info(GetType().Name + ": Could not find appearance stone item " + stone.getId());
		}
	}

	public AppearanceStone getStone(int stone)
	{
		return _stoneMap.get(stone);
	}
	
	/**
	 * Gets the single instance of AppearanceItemData.
	 * @return single instance of AppearanceItemData
	 */
	public static AppearanceItemData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly AppearanceItemData INSTANCE = new AppearanceItemData();
	}
}