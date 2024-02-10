using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
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
		parseDatapackFile("data/TeleportListData.xml");
		_teleportCount = _teleports.size();
		LOGGER.Info(GetType().Name + ": Loaded " + _teleportCount + " teleports.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "teleport", teleportNode =>
		{
			StatSet set = new StatSet(parseAttributes(teleportNode));
			int tpId = set.getInt("id");
			int tpPrice = set.getInt("price");
			bool special = set.getBoolean("special", false);
			List<Location> locations = new();
			forEach(teleportNode, "location", locationsNode =>
			{
				StatSet locationSet = new StatSet(parseAttributes(locationsNode));
				locations.add(new Location(locationSet.getInt("x"), locationSet.getInt("y"), locationSet.getInt("z")));
			});
			if (locations.isEmpty())
			{
				locations.add(new Location(set.getInt("x"), set.getInt("y"), set.getInt("z")));
			}
			_teleports.put(tpId, new TeleportListHolder(tpId, locations, tpPrice, special));
		}));
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