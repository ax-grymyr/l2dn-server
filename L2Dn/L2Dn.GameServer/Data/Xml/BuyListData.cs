using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads buy lists for NPCs.
 * @author NosBit
 */
public class BuyListData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(BuyListData));
	
	private readonly Map<int, ProductList> _buyLists = new();
	private static readonly FileFilter NUMERIC_FILTER = new NumericNameFilter();
	
	protected BuyListData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_buyLists.clear();
		parseDatapackDirectory("data/buylists", false);
		if (Config.CUSTOM_BUYLIST_LOAD)
		{
			parseDatapackDirectory("data/buylists/custom", false);
		}
		
		LOGGER.Info(GetType().Name + ": Loaded " + _buyLists.size() + " buyLists.");
		
		try 
		{
			using GameServerDbContext ctx = new();
			Statement statement = con.createStatement();
			ResultSet rs = statement.executeQuery("SELECT * FROM `buylists`");
			while (rs.next())
			{
				int buyListId = rs.getInt("buylist_id");
				int itemId = rs.getInt("item_id");
				long count = rs.getLong("count");
				long nextRestockTime = rs.getLong("next_restock_time");
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
	
	public void parseDocument(Document doc, File f)
	{
		try
		{
			int buyListId = int.Parse(f.getName().replaceAll(".xml", ""));
			forEach(doc, "list", list =>
			{
				int defaultBaseTax = parseInteger(list.getAttributes(), "baseTax", 0);
				ProductList buyList = new ProductList(buyListId);
				forEach(list, node =>
				{
					switch (node.getNodeName())
					{
						case "item":
						{
							NamedNodeMap attrs = node.getAttributes();
							int itemId = parseInteger(attrs, "id");
							ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
							if (item != null)
							{
								long price = Parse(attrs, "price", -1L);
								long restockDelay = Parse(attrs, "restock_delay", -1L);
								long count = Parse(attrs, "count", -1L);
								int baseTax = parseInteger(attrs, "baseTax", defaultBaseTax);
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
								LOGGER.Warn("Item not found. BuyList:" + buyListId + " ItemID:" + itemId + " File:" + f);
							}
							break;
						}
						case "npcs":
						{
							forEach(node, "npc", npcNode => buyList.addAllowedNpc(int.Parse(npcNode.getTextContent())));
							break;
						}
					}
				});
				_buyLists.put(buyListId, buyList);
			});
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed to load buyList data from xml File:" + f.getName(), e);
		}
	}
	
	public FileFilter getCurrentFileFilter()
	{
		return NUMERIC_FILTER;
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