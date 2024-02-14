using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
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
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/SayuneData.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("map").ForEach(parseElement);
		
		
		LOGGER.Info(GetType().Name + ": Loaded " + _maps.size() + " maps.");
	}

	private void parseElement(XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		SayuneEntry map = new SayuneEntry(id);
		parseEntries(map, element);
		_maps.put(map.getId(), map);
	}

	private void parseEntries(SayuneEntry lastEntry, XElement element)
	{
		element.Elements().ForEach(el =>
		{
			if (el.Name.LocalName == "selector" || el.Name.LocalName == "choice" || el.Name.LocalName == "loc")
			{
				int id = el.Attribute("id").GetInt32();
				int x = el.Attribute("x").GetInt32();
				int y = el.Attribute("y").GetInt32();
				int z = el.Attribute("z").GetInt32();
				parseEntries(lastEntry.addInnerEntry(new SayuneEntry(el.Name.LocalName == "selector", id, x, y, z)), el);
			}
		});
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