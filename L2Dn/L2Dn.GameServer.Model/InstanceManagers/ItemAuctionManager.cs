using System.Reflection.Metadata;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.ItemAuction;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Forsaiken
 */
public class ItemAuctionManager: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemAuctionManager));

	private readonly Map<int, ItemAuctionInstance> _managerInstances = new();
	private AtomicInteger _auctionIds = new AtomicInteger();

	protected ItemAuctionManager()
	{
		if (!Config.ALT_ITEM_AUCTION_ENABLED)
		{
			LOGGER.Info(GetType().Name +": Disabled.");
			return;
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			_auctionIds.set(ctx.ItemAuctions.Select(a => a.AuctionId).OrderByDescending(a => a).FirstOrDefault());
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed loading auctions." + e);
		}

		load();
	}

	public void load()
	{
		_managerInstances.Clear();

		LoadXmlDocument(DataFileLocation.Data, "ItemAuctions.xml").Elements("list").Elements("instance")
			.ForEach(parseElement);

		LOGGER.Info(GetType().Name +": Loaded " + _managerInstances.Count + " instances.");
	}

	private void parseElement(XElement element)
	{
		try
		{
			int instanceId = element.GetAttributeValueAsInt32("id");
			if (_managerInstances.ContainsKey(instanceId))
				throw new Exception("Dublicated instanceId " + instanceId);

			ItemAuctionInstance instance = new ItemAuctionInstance(instanceId, _auctionIds, element);
			_managerInstances.put(instanceId, instance);
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Failed loading auctions from xml." + e);
		}
	}

	public void shutdown()
	{
		foreach (ItemAuctionInstance instance in _managerInstances.Values)
		{
			instance.shutdown();
		}
	}

	public ItemAuctionInstance? getManagerInstance(int instanceId)
	{
		return _managerInstances.get(instanceId);
	}

	public int getNextAuctionId()
	{
		return _auctionIds.incrementAndGet();
	}

	public static void deleteAuction(int auctionId)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.ItemAuctionBids.Where(a => a.AuctionId == auctionId).ExecuteDelete();
			ctx.ItemAuctions.Where(a => a.AuctionId == auctionId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("ItemAuctionManagerInstance: Failed deleting auction: " + auctionId, e);
		}
	}

	/**
	 * Gets the single instance of {@code ItemAuctionManager}.
	 * @return single instance of {@code ItemAuctionManager}
	 */
	public static ItemAuctionManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly ItemAuctionManager INSTANCE = new ItemAuctionManager();
	}
}