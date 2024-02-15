using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author MacuK
 */
public class RaidDropAnnounceData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RaidDropAnnounceData));
	
	private readonly Set<int> _itemIds = new();
	
	protected RaidDropAnnounceData()
	{
		load();
	}
	
	public void load()
	{
		_itemIds.clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "RaidDropAnnounceData.xml");
		document.Elements("list").Elements("item").ForEach(parseElement);
		
		if (!_itemIds.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _itemIds.size() + " raid drop announce data.");
		}
	}

	private void parseElement(XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		ItemTemplate item = ItemData.getInstance().getTemplate(id);
		if (item != null)
		{
			_itemIds.add(id);
		}
		else
		{
			LOGGER.Error(GetType().Name + ": Could not find item with id: " + id);
		}
	}

	public bool isAnnounce(int itemId)
	{
		return _itemIds.Contains(itemId);
	}
	
	public static RaidDropAnnounceData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly RaidDropAnnounceData INSTANCE = new();
	}
}