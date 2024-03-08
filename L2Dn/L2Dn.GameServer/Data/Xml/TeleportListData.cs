using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author NviX, Mobius
 */
public class TeleportListData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(TeleportListData));
	private readonly Map<int, TeleportListHolder> _teleports = new();
	private int _teleportCount = 0;
	
	protected TeleportListData()
	{
		load();
	}
	
	public void load()
	{
		_teleports.clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "TeleportListData.xml");
		document.Elements("list").Elements("teleport").ForEach(parseElement);
		
		_teleportCount = _teleports.size();
		LOGGER.Info(GetType().Name + ": Loaded " + _teleportCount + " teleports.");
	}

	private void parseElement(XElement element)
	{
		int tpId = element.GetAttributeValueAsInt32("id");
		int tpPrice = element.GetAttributeValueAsInt32("price");
		bool special = element.Attribute("special").GetBoolean(false);
		List<Location> locations = new();
		element.Elements("location").ForEach(el =>
		{
			int x = el.GetAttributeValueAsInt32("x");
			int y = el.GetAttributeValueAsInt32("y");
			int z = el.GetAttributeValueAsInt32("z");
			locations.add(new Location(x, y, z));
		});

		if (locations.isEmpty())
		{
			int x = element.GetAttributeValueAsInt32("x");
			int y = element.GetAttributeValueAsInt32("y");
			int z = element.GetAttributeValueAsInt32("z");
			locations.add(new Location(x, y, z));
		}

		_teleports.put(tpId, new TeleportListHolder(tpId, locations, tpPrice, special));
	}

	public TeleportListHolder getTeleport(int teleportId)
	{
		return _teleports.get(teleportId);
	}
	
	public int getTeleportCount()
	{
		return _teleportCount;
	}
	
	public static TeleportListData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly TeleportListData INSTANCE = new();
	}
}