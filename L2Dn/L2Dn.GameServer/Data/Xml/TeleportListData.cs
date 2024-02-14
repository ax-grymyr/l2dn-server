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
public class TeleportListData
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
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/TeleportListData.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("teleport").ForEach(parseElement);
		
		_teleportCount = _teleports.size();
		LOGGER.Info(GetType().Name + ": Loaded " + _teleportCount + " teleports.");
	}

	private void parseElement(XElement element)
	{
		int tpId = element.Attribute("id").GetInt32();
		int tpPrice = element.Attribute("price").GetInt32();
		bool special = element.Attribute("special").GetBoolean(false);
		List<Location> locations = new();
		element.Elements("location").ForEach(el =>
		{
			int x = el.Attribute("x").GetInt32();
			int y = el.Attribute("y").GetInt32();
			int z = el.Attribute("z").GetInt32();
			locations.add(new Location(x, y, z));
		});

		if (locations.isEmpty())
		{
			int x = element.Attribute("x").GetInt32();
			int y = element.Attribute("y").GetInt32();
			int z = element.Attribute("z").GetInt32();
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