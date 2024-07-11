using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.PrimeShop;
using L2Dn.GameServer.Network.OutgoingPackets.PrimeShop;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Gnacik, UnAfraid
 */
public class PrimeShopData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PrimeShopData));
	
	private readonly Map<int, PrimeShopGroup> _primeItems = new();
	
	protected PrimeShopData()
	{
		load();
	}
	
	public void load()
	{
		_primeItems.Clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "PrimeShop.xml");
		document.Elements("list").Where(el => el.Attribute("enabled").GetBoolean(false)).Elements("item")
			.ForEach(parseElement);
		
		if (!_primeItems.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _primeItems.size() + " items.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": System is disabled.");
		}
	}

	private void parseElement(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		List<PrimeShopItem> items = new();

		element.Elements("item").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("itemId");
			int count = el.GetAttributeValueAsInt32("count");
			ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
			if (item == null)
			{
				LOGGER.Error(GetType().Name + ": Item template null for itemId: " + itemId + " brId: " + id);
				return;
			}

			items.Add(new PrimeShopItem(itemId, count, item.getWeight(), item.isTradeable() ? 1 : 0));
		});

		_primeItems.put(id, new PrimeShopGroup(element, items));
	}

	public void showProductInfo(Player player, int brId)
	{
		PrimeShopGroup item = _primeItems.get(brId);
		if ((player == null) || (item == null))
		{
			return;
		}
		
		player.sendPacket(new ExBRProductInfoPacket(item, player));
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