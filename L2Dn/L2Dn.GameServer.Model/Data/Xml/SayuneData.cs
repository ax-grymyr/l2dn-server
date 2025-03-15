using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class SayuneData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SayuneData));
	
	private readonly Map<int, SayuneEntry> _maps = new();
	
	protected SayuneData()
	{
		load();
	}
	
	public void load()
	{
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "SayuneData.xml");
		document.Elements("list").Elements("map").ForEach(parseElement);
		
		
		LOGGER.Info(GetType().Name + ": Loaded " + _maps.Count + " maps.");
	}

	private void parseElement(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
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
				int id = el.GetAttributeValueAsInt32("id");
				int x = el.GetAttributeValueAsInt32("x");
				int y = el.GetAttributeValueAsInt32("y");
				int z = el.GetAttributeValueAsInt32("z");
				Location3D location = new(x, y, z);
				parseEntries(lastEntry.addInnerEntry(new SayuneEntry(el.Name.LocalName == "selector", id, location)), el);
			}
		});
	}
	
	public SayuneEntry? getMap(int id)
	{
		return _maps.get(id);
	}
	
	public ICollection<SayuneEntry> getMaps()
	{
		return _maps.Values;
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