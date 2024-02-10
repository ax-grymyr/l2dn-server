using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Gustavo Fonseca
 */
public class RaidTeleportListData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RaidTeleportListData));
	
	private readonly Map<int, TeleportListHolder> _teleports = new();
	
	protected RaidTeleportListData()
	{
		load();
	}
	
	public void load()
	{
		_teleports.clear();
		parseDatapackFile("data/RaidTeleportListData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _teleports.size() + " teleports.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "teleport", teleportNode =>
		{
			StatSet set = new StatSet(parseAttributes(teleportNode));
			int tpId = set.getInt("id");
			int x = set.getInt("x");
			int y = set.getInt("y");
			int z = set.getInt("z");
			int tpPrice = set.getInt("price");
			_teleports.put(tpId, new TeleportListHolder(tpId, x, y, z, tpPrice, false));
		}));
	}
	
	public TeleportListHolder getTeleport(int teleportId)
	{
		return _teleports.get(teleportId);
	}
	
	public static RaidTeleportListData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly RaidTeleportListData INSTANCE = new();
	}
}