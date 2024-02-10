using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class SayuneData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SayuneData));
	
	private readonly Map<int, SayuneEntry> _maps = new();
	
	protected SayuneData()
	{
		load();
	}
	
	public void load()
	{
		parseDatapackFile("data/SayuneData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _maps.size() + " maps.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("map".equalsIgnoreCase(d.getNodeName()))
					{
						int id = parseInteger(d.getAttributes(), "id");
						SayuneEntry map = new SayuneEntry(id);
						parseEntries(map, d);
						_maps.put(map.getId(), map);
					}
				}
			}
		}
	}
	
	private void parseEntries(SayuneEntry lastEntry, Node n)
	{
		NamedNodeMap attrs;
		for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
		{
			if ("selector".equals(d.getNodeName()) || "choice".equals(d.getNodeName()) || "loc".equals(d.getNodeName()))
			{
				attrs = d.getAttributes();
				int id = parseInteger(attrs, "id");
				int x = parseInteger(attrs, "x");
				int y = parseInteger(attrs, "y");
				int z = parseInteger(attrs, "z");
				parseEntries(lastEntry.addInnerEntry(new SayuneEntry("selector".equals(d.getNodeName()), id, x, y, z)), d);
			}
		}
	}
	
	public SayuneEntry getMap(int id)
	{
		return _maps.get(id);
	}
	
	public ICollection<SayuneEntry> getMaps()
	{
		return _maps.values();
	}
	
	/**
	 * Gets the single instance of SayuneData.
	 * @return single instance of SayuneData
	 */
	public static SayuneData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SayuneData INSTANCE = new();
	}
}