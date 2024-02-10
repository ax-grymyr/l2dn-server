using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class AppearanceItemData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AppearanceItemData));
	
	private AppearanceStone[] _stones;
	private readonly Map<int, AppearanceStone> _stoneMap = new();
	
	protected AppearanceItemData()
	{
		load();
	}
	
	public void load()
	{
		parseDatapackFile("data/AppearanceStones.xml");
		
		if (!_stoneMap.isEmpty())
		{
			_stones = new AppearanceStone[Collections.max(_stoneMap.keySet()) + 1];
			foreach (Entry<int, AppearanceStone> stone in _stoneMap.entrySet())
			{
				_stones[stone.getKey()] = stone.getValue();
			}
			
			LOGGER.Info(GetType().Name + ": Loaded " + _stoneMap.size() + " stones.");
		}
		
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
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("appearance_stone".equalsIgnoreCase(d.getNodeName()))
					{
						AppearanceStone stone = new AppearanceStone(new StatSet(parseAttributes(d)));
						for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
						{
							switch (c.getNodeName())
							{
								case "grade":
								{
									CrystalType type = CrystalType.valueOf(c.getTextContent());
									stone.addCrystalType(type);
									break;
								}
								case "targetType":
								{
									AppearanceTargetType type = AppearanceTargetType.valueOf(c.getTextContent());
									stone.addTargetType(type);
									break;
								}
								case "bodyPart":
								{
									long part = ItemData.SLOTS.get(c.getTextContent());
									stone.addBodyPart(part);
									break;
								}
								case "race":
								{
									Race race = Race.valueOf(c.getTextContent());
									stone.addRace(race);
									break;
								}
								case "raceNot":
								{
									Race raceNot = Race.valueOf(c.getTextContent());
									stone.addRaceNot(raceNot);
									break;
								}
								case "visual":
								{
									stone.addVisualId(new AppearanceHolder(new StatSet(parseAttributes(c))));
								}
							}
						}
						if (ItemData.getInstance().getTemplate(stone.getId()) != null)
						{
							_stoneMap.put(stone.getId(), stone);
						}
						else
						{
							LOGGER.Info(GetType().Name + ": Could not find appearance stone item " + stone.getId());
						}
					}
				}
			}
		}
	}
	
	public AppearanceStone getStone(int stone)
	{
		if (_stones.Length > stone)
		{
			return _stones[stone];
		}
		return null;
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