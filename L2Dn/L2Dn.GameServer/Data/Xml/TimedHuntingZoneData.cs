using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class TimedHuntingZoneData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(TimedHuntingZoneData));
	
	private readonly Map<int, TimedHuntingZoneHolder> _timedHuntingZoneData = new();
	
	protected TimedHuntingZoneData()
	{
		load();
	}
	
	public void load()
	{
		_timedHuntingZoneData.clear();
		parseDatapackFile("data/TimedHuntingZoneData.xml");
		
		if (!_timedHuntingZoneData.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _timedHuntingZoneData.size() + " timed hunting zones.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": System is disabled.");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node xmlNode = doc.getFirstChild(); xmlNode != null; xmlNode = xmlNode.getNextSibling())
		{
			if ("list".equalsIgnoreCase(xmlNode.getNodeName()))
			{
				NamedNodeMap listAttributes = xmlNode.getAttributes();
				Node attribute = listAttributes.getNamedItem("enabled");
				if ((attribute != null) && Boolean.parseBoolean(attribute.getNodeValue()))
				{
					for (Node listNode = xmlNode.getFirstChild(); listNode != null; listNode = listNode.getNextSibling())
					{
						if ("zone".equalsIgnoreCase(listNode.getNodeName()))
						{
							NamedNodeMap zoneAttributes = listNode.getAttributes();
							int id = parseInteger(zoneAttributes, "id");
							String name = parseString(zoneAttributes, "name", "");
							int initialTime = 0;
							int maxAddedTime = 0;
							int resetDelay = 0;
							int entryItemId = 57;
							int entryFee = 10000;
							int minLevel = 1;
							int maxLevel = 999;
							int remainRefillTime = 3600;
							int refillTimeMax = 3600;
							bool pvpZone = false;
							bool noPvpZone = false;
							int instanceId = 0;
							bool soloInstance = true;
							bool weekly = false;
							bool useWorldPrefix = false;
							bool zonePremiumUserOnly = false;
							Location enterLocation = null;
							Location subEnterLocation1 = null;
							Location subEnterLocation2 = null;
							Location subEnterLocation3 = null;
							Location exitLocation = null;
							for (Node zoneNode = listNode.getFirstChild(); zoneNode != null; zoneNode = zoneNode.getNextSibling())
							{
								switch (zoneNode.getNodeName())
								{
									case "enterLocation":
									{
										String[] coordinates = zoneNode.getTextContent().split(",");
										enterLocation = new Location(int.Parse(coordinates[0]), int.Parse(coordinates[1]), int.Parse(coordinates[2]));
										break;
									}
									case "subEnterLocation1":
									{
										String[] coordinates = zoneNode.getTextContent().split(",");
										subEnterLocation1 = new Location(int.Parse(coordinates[0]), int.Parse(coordinates[1]), int.Parse(coordinates[2]));
										break;
									}
									case "subEnterLocation2":
									{
										String[] coordinates = zoneNode.getTextContent().split(",");
										subEnterLocation2 = new Location(int.Parse(coordinates[0]), int.Parse(coordinates[1]), int.Parse(coordinates[2]));
										break;
									}
									case "subEnterLocation3":
									{
										String[] coordinates = zoneNode.getTextContent().split(",");
										subEnterLocation3 = new Location(int.Parse(coordinates[0]), int.Parse(coordinates[1]), int.Parse(coordinates[2]));
										break;
									}
									case "exitLocation":
									{
										String[] coordinates = zoneNode.getTextContent().split(",");
										exitLocation = new Location(int.Parse(coordinates[0]), int.Parse(coordinates[1]), int.Parse(coordinates[2]));
										break;
									}
									case "initialTime":
									{
										initialTime = int.Parse(zoneNode.getTextContent()) * 1000;
										break;
									}
									case "maxAddedTime":
									{
										maxAddedTime = int.Parse(zoneNode.getTextContent()) * 1000;
										break;
									}
									case "resetDelay":
									{
										resetDelay = int.Parse(zoneNode.getTextContent()) * 1000;
										break;
									}
									case "entryItemId":
									{
										entryItemId = int.Parse(zoneNode.getTextContent());
										break;
									}
									case "entryFee":
									{
										entryFee = int.Parse(zoneNode.getTextContent());
										break;
									}
									case "minLevel":
									{
										minLevel = int.Parse(zoneNode.getTextContent());
										break;
									}
									case "maxLevel":
									{
										maxLevel = int.Parse(zoneNode.getTextContent());
										break;
									}
									case "remainRefillTime":
									{
										remainRefillTime = int.Parse(zoneNode.getTextContent());
										break;
									}
									case "refillTimeMax":
									{
										refillTimeMax = int.Parse(zoneNode.getTextContent());
										break;
									}
									case "pvpZone":
									{
										pvpZone = Boolean.parseBoolean(zoneNode.getTextContent());
										break;
									}
									case "noPvpZone":
									{
										noPvpZone = Boolean.parseBoolean(zoneNode.getTextContent());
										break;
									}
									case "instanceId":
									{
										instanceId = int.Parse(zoneNode.getTextContent());
										break;
									}
									case "soloInstance":
									{
										soloInstance = Boolean.parseBoolean(zoneNode.getTextContent());
										break;
									}
									case "weekly":
									{
										weekly = Boolean.parseBoolean(zoneNode.getTextContent());
										break;
									}
									case "useWorldPrefix":
									{
										useWorldPrefix = Boolean.parseBoolean(zoneNode.getTextContent());
										break;
									}
									case "zonePremiumUserOnly":
									{
										zonePremiumUserOnly = Boolean.parseBoolean(zoneNode.getTextContent());
										break;
									}
								}
							}
							_timedHuntingZoneData.put(id, new TimedHuntingZoneHolder(id, name, initialTime, maxAddedTime, resetDelay, entryItemId, entryFee, minLevel, maxLevel, remainRefillTime, refillTimeMax, pvpZone, noPvpZone, instanceId, soloInstance, weekly, useWorldPrefix, zonePremiumUserOnly, enterLocation, subEnterLocation1, subEnterLocation2, subEnterLocation3, exitLocation));
						}
					}
				}
			}
		}
	}
	
	public TimedHuntingZoneHolder getHuntingZone(int zoneId)
	{
		return _timedHuntingZoneData.get(zoneId);
	}
	
	public ICollection<TimedHuntingZoneHolder> getAllHuntingZones()
	{
		return _timedHuntingZoneData.values();
	}
	
	public int getSize()
	{
		return _timedHuntingZoneData.size();
	}
	
	public static TimedHuntingZoneData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly TimedHuntingZoneData INSTANCE = new();
	}
}