using L2Dn.GameServer.Model.Fishings;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Fishing information.
 * @author bit
 */
public class FishingData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(FishingData));
	private readonly Map<int, FishingBait> _baitData = new();
	private readonly Map<int, FishingRod> _rodData = new();
	private int _baitDistanceMin;
	private int _baitDistanceMax;
	private double _expRateMin;
	private double _expRateMax;
	private double _spRateMin;
	private double _spRateMax;
	
	/**
	 * Instantiates a new fishing data.
	 */
	protected FishingData()
	{
		load();
	}
	
	public void load()
	{
		_baitData.clear();
		parseDatapackFile("data/Fishing.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _baitData.size() + " bait and " + _rodData.size() + " rod data.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node listItem = n.getFirstChild(); listItem != null; listItem = listItem.getNextSibling())
				{
					switch (listItem.getNodeName())
					{
						case "baitDistance":
						{
							_baitDistanceMin = parseInteger(listItem.getAttributes(), "min");
							_baitDistanceMax = parseInteger(listItem.getAttributes(), "max");
							break;
						}
						case "xpRate":
						{
							_expRateMin = parseDouble(listItem.getAttributes(), "min");
							_expRateMax = parseDouble(listItem.getAttributes(), "max");
							break;
						}
						case "spRate":
						{
							_spRateMin = parseDouble(listItem.getAttributes(), "min");
							_spRateMax = parseDouble(listItem.getAttributes(), "max");
							break;
						}
						case "baits":
						{
							for (Node bait = listItem.getFirstChild(); bait != null; bait = bait.getNextSibling())
							{
								if ("bait".equalsIgnoreCase(bait.getNodeName()))
								{
									NamedNodeMap attrs = bait.getAttributes();
									int itemId = parseInteger(attrs, "itemId");
									int level = parseInteger(attrs, "level", 1);
									int minPlayerLevel = parseInteger(attrs, "minPlayerLevel");
									int maxPlayerLevel = parseInteger(attrs, "maxPlayerLevel", Config.PLAYER_MAXIMUM_LEVEL);
									double chance = parseDouble(attrs, "chance");
									int timeMin = parseInteger(attrs, "timeMin");
									int timeMax = parseInteger(attrs, "timeMax", timeMin);
									int waitMin = parseInteger(attrs, "waitMin");
									int waitMax = parseInteger(attrs, "waitMax", waitMin);
									bool isPremiumOnly = parseBoolean(attrs, "isPremiumOnly", false);
									if (ItemData.getInstance().getTemplate(itemId) == null)
									{
										LOGGER.Info(GetType().Name + ": Could not find item with id " + itemId);
										continue;
									}
									
									FishingBait baitData = new FishingBait(itemId, level, minPlayerLevel, maxPlayerLevel, chance, timeMin, timeMax, waitMin, waitMax, isPremiumOnly);
									for (Node c = bait.getFirstChild(); c != null; c = c.getNextSibling())
									{
										if ("catch".equalsIgnoreCase(c.getNodeName()))
										{
											NamedNodeMap cAttrs = c.getAttributes();
											int cId = parseInteger(cAttrs, "itemId");
											float cChance = parseFloat(cAttrs, "chance");
											float cMultiplier = parseFloat(cAttrs, "multiplier", 1f);
											if (ItemData.getInstance().getTemplate(cId) == null)
											{
												LOGGER.Info(GetType().Name + ": Could not find item with id " + itemId);
												continue;
											}
											
											baitData.addReward(new FishingCatch(cId, cChance, cMultiplier));
										}
									}
									_baitData.put(baitData.getItemId(), baitData);
								}
							}
							break;
						}
						case "rods":
						{
							for (Node rod = listItem.getFirstChild(); rod != null; rod = rod.getNextSibling())
							{
								if ("rod".equalsIgnoreCase(rod.getNodeName()))
								{
									NamedNodeMap attrs = rod.getAttributes();
									int itemId = parseInteger(attrs, "itemId");
									int reduceFishingTime = parseInteger(attrs, "reduceFishingTime", 0);
									float xpMultiplier = parseFloat(attrs, "xpMultiplier", 1f);
									float spMultiplier = parseFloat(attrs, "spMultiplier", 1f);
									if (ItemData.getInstance().getTemplate(itemId) == null)
									{
										LOGGER.Info(GetType().Name + ": Could not find item with id " + itemId);
										continue;
									}
									
									_rodData.put(itemId, new FishingRod(itemId, reduceFishingTime, xpMultiplier, spMultiplier));
								}
							}
						}
					}
				}
			}
		}
	}
	
	public FishingBait getBaitData(int baitItemId)
	{
		return _baitData.get(baitItemId);
	}
	
	public FishingRod getRodData(int rodItemId)
	{
		return _rodData.get(rodItemId);
	}
	
	public int getBaitDistanceMin()
	{
		return _baitDistanceMin;
	}
	
	public int getBaitDistanceMax()
	{
		return _baitDistanceMax;
	}
	
	public double getExpRateMin()
	{
		return _expRateMin;
	}
	
	public double getExpRateMax()
	{
		return _expRateMax;
	}
	
	public double getSpRateMin()
	{
		return _spRateMin;
	}
	
	public double getSpRateMax()
	{
		return _spRateMax;
	}
	
	/**
	 * Gets the single instance of FishingData.
	 * @return single instance of FishingData
	 */
	public static FishingData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly FishingData INSTANCE = new();
	}
}