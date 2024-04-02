using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads buy lists for NPCs.
 * @author NosBit
 */
public class BuyListData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(BuyListData));
	
	private readonly Map<int, ProductList> _buyLists = new();
	
	protected BuyListData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_buyLists.clear();

		LoadXmlDocuments(DataFileLocation.Data, "buylists").ForEach(t => loadFile(t.FilePath, t.Document));
		if (Config.CUSTOM_BUYLIST_LOAD)
		{
			LoadXmlDocuments(DataFileLocation.Data, "buylists/custom").ForEach(t => loadFile(t.FilePath, t.Document));
		}

		LOGGER.Info(GetType().Name + ": Loaded " + _buyLists.size() + " buyLists.");
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var buyLists = ctx.BuyLists;
			foreach (DbBuyList dbBuyList in buyLists)
			{
				int buyListId = dbBuyList.BuyListId;
				int itemId = dbBuyList.ItemId;
				long count = dbBuyList.Count;
				DateTime nextRestockTime = dbBuyList.NextRestockTime;
				ProductList buyList = getBuyList(buyListId);
				if (buyList == null)
				{
					LOGGER.Warn("BuyList found in database but not loaded from xml! BuyListId: " + buyListId);
					continue;
				}
				Product product = buyList.getProductByItemId(itemId);
				if (product == null)
				{
					LOGGER.Warn("ItemId found in database but not loaded from xml! BuyListId: " + buyListId + " ItemId: " + itemId);
					continue;
				}
				if (count < product.getMaxCount())
				{
					product.setCount(count);
					product.restartRestockTask(nextRestockTime);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed to load buyList data from database.", e);
		}
	}

	private void loadFile(string filePath, XDocument document)
	{
		//int defaultBaseTax = parseInteger(list.getAttributes(), "baseTax", 0);
		int buyListId = int.Parse(Path.GetFileNameWithoutExtension(filePath)); // TODO: is it required somewhere to be a number?
		ProductList buyList = new ProductList(buyListId);

		document.Elements("list").Elements("npcs").Elements("npc").Select(n => (int)n)
			.ForEach(x => buyList.addAllowedNpc(x));

		document.Elements("list").Elements("item").ForEach(node =>
		{
			int itemId = node.GetAttributeValueAsInt32("id");
			ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
			if (item != null)
			{
				long price = node.Attribute("price").GetInt64(-1);
				TimeSpan restockDelay = TimeSpan.FromMinutes(node.Attribute("restock_delay").GetInt64(-1));
				long count = node.Attribute("count").GetInt64(-1);
				int baseTax = node.Attribute("baseTax").GetInt32(0);
				int sellPrice = item.getReferencePrice() / 2;
				if (Config.CORRECT_PRICES && (price > -1) && (sellPrice > price) && (buyList.getNpcsAllowed() != null))
				{
					LOGGER.Warn("Buy price " + price + " is less than sell price " + sellPrice + " for ItemID:" + itemId + " of buylist " + buyList.getListId() + ".");
					buyList.addProduct(new Product(buyListId, item, sellPrice, restockDelay, count, baseTax));
				}
				else
				{
					buyList.addProduct(new Product(buyListId, item, price, restockDelay, count, baseTax));
				}
			}
			else
			{
				LOGGER.Warn("Item not found. BuyList:" + buyListId + " ItemID:" + itemId + " File:" + filePath);
			}

			_buyLists.put(buyListId, buyList);
		});
	}
	
	public ProductList getBuyList(int listId)
	{
		return _buyLists.get(listId);
	}
	
	public static BuyListData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly BuyListData INSTANCE = new();
	}
}