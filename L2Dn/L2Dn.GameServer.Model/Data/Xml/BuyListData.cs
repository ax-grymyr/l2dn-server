using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.DataPack;
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

		LoadXmlDocuments<XmlBuyList>(DataFileLocation.Data, "buylists").ForEach(t => loadFile(t.FilePath, t.Document));
		if (Config.CUSTOM_BUYLIST_LOAD)
		{
			LoadXmlDocuments<XmlBuyList>(DataFileLocation.Data, "buylists/custom").ForEach(t => loadFile(t.FilePath, t.Document));
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
				
				if (count < product.getRestock()?.Count)
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

	private void loadFile(string filePath, XmlBuyList document)
	{
		//int defaultBaseTax = parseInteger(list.getAttributes(), "baseTax", 0);
		int buyListId = int.Parse(Path.GetFileNameWithoutExtension(filePath));

		List<int> allowedNpc = document.Npcs;
		List<Product> products = document.Items.Select(buyListItem =>
		{
			int itemId = buyListItem.Id;
			ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
			if (item == null)
			{
				LOGGER.Warn("Item not found. BuyList:" + buyListId + " ItemID:" + itemId + " File:" + filePath);
				return null;
			}

			long price = buyListItem.Price;

			ProductRestock? restock = null;
			if (buyListItem.CountSpecified && buyListItem.RestockDelaySpecified)
				restock = new ProductRestock(buyListItem.Count, TimeSpan.FromMinutes(buyListItem.RestockDelay));

			int baseTax = buyListItem.BaseTaxSpecified ? buyListItem.BaseTax : 0;
			int sellPrice = item.getReferencePrice() / 2;
			if (Config.CORRECT_PRICES && allowedNpc.Count != 0 && price >= 0 && sellPrice > price)
			{
				LOGGER.Warn(
					$"Buy price {price} is less than sell price {sellPrice} for ItemID:{itemId} of buylist {buyListId}.");

				price = sellPrice;
			}

			return new Product(buyListId, item, price, restock, baseTax);
		}).Where(x => x != null).ToList();

		List<int> duplicateItemIds =
			products.GroupBy(x => x.getItemId()).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

		if (duplicateItemIds.Count > 0)
		{
			LOGGER.Warn($"Buylist={buyListId} contains duplicated item ids {string.Join(", ", duplicateItemIds)}.");
		}
		
		ProductList buyList = new(buyListId, products.ToImmutableArray(), allowedNpc.ToImmutableSortedSet());
		_buyLists.put(buyListId, buyList);
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