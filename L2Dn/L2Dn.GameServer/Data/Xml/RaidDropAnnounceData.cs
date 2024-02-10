using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author MacuK
 */
public class RaidDropAnnounceData
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
		parseDatapackFile("data/RaidDropAnnounceData.xml");
		if (!_itemIds.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _itemIds.size() + " raid drop announce data.");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("item".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						Node att;
						StatSet set = new StatSet();
						for (int i = 0; i < attrs.getLength(); i++)
						{
							att = attrs.item(i);
							set.set(att.getNodeName(), att.getNodeValue());
						}
						
						int id = parseInteger(attrs, "id");
						ItemTemplate item = ItemData.getInstance().getTemplate(id);
						if (item != null)
						{
							_itemIds.add(id);
						}
						else
						{
							LOGGER.Warn(GetType().Name + ": Could not find item with id: " + id);
						}
					}
				}
			}
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