using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.PrimeShop;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Gnacik, UnAfraid
 */
public class PrimeShopData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PrimeShopData));
	
	private readonly Map<int, PrimeShopGroup> _primeItems = new();
	
	protected PrimeShopData()
	{
		load();
	}
	
	public void load()
	{
		_primeItems.clear();
		parseDatapackFile("data/PrimeShop.xml");
		
		if (!_primeItems.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _primeItems.size() + " items.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": System is disabled.");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				NamedNodeMap at = n.getAttributes();
				Node attribute = at.getNamedItem("enabled");
				if ((attribute != null) && Boolean.parseBoolean(attribute.getNodeValue()))
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
							
							List<PrimeShopItem> items = new();
							for (Node b = d.getFirstChild(); b != null; b = b.getNextSibling())
							{
								if ("item".equalsIgnoreCase(b.getNodeName()))
								{
									attrs = b.getAttributes();
									
									int itemId = parseInteger(attrs, "itemId");
									int count = parseInteger(attrs, "count");
									ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
									if (item == null)
									{
										LOGGER.Error(GetType().Name + ": Item template null for itemId: " + itemId + " brId: " + set.getInt("id"));
										return;
									}
									
									items.add(new PrimeShopItem(itemId, count, item.getWeight(), item.isTradeable() ? 1 : 0));
								}
							}
							
							_primeItems.put(set.getInt("id"), new PrimeShopGroup(set, items));
						}
					}
				}
			}
		}
	}
	
	public void showProductInfo(Player player, int brId)
	{
		PrimeShopGroup item = _primeItems.get(brId);
		if ((player == null) || (item == null))
		{
			return;
		}
		
		player.sendPacket(new ExBRProductInfo(item, player));
	}
	
	public PrimeShopGroup getItem(int brId)
	{
		return _primeItems.get(brId);
	}
	
	public Map<int, PrimeShopGroup> getPrimeItems()
	{
		return _primeItems;
	}
	
	public static PrimeShopData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PrimeShopData INSTANCE = new();
	}
}