using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Gustavo Fonseca
 */
public class RaidTeleportListData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RaidTeleportListData));

	private readonly Map<int, TeleportListHolder> _teleports = new();

	protected RaidTeleportListData()
	{
		load();
	}

	public void load()
	{
		_teleports.Clear();

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "RaidTeleportListData.xml");
		document.Elements("list").Elements("teleport").ForEach(parseElement);

		LOGGER.Info(GetType().Name + ": Loaded " + _teleports.Count + " teleports.");
	}

	private void parseElement(XElement element)
	{
		int tpId = element.GetAttributeValueAsInt32("id");
		int x = element.GetAttributeValueAsInt32("x");
		int y = element.GetAttributeValueAsInt32("y");
		int z = element.GetAttributeValueAsInt32("z");
		int tpPrice = element.GetAttributeValueAsInt32("price");
		_teleports.put(tpId, new TeleportListHolder(tpId, new Location3D(x, y, z), tpPrice, false));
	}

	public TeleportListHolder? getTeleport(int teleportId)
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