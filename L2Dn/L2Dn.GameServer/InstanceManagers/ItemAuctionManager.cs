using L2Dn.GameServer.Model.ItemAuction;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Forsaiken
 */
public class ItemAuctionManager: IXmlReader
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemAuctionManager));
	
	private readonly Map<int, ItemAuctionInstance> _managerInstances = new();
	private readonly AtomicInteger _auctionIds = new AtomicInteger(1);
	
	protected ItemAuctionManager()
	{
		if (!Config.ALT_ITEM_AUCTION_ENABLED)
		{
			LOGGER.Info(GetType().Name +": Disabled.");
			return;
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			Statement statement = con.createStatement();
			ResultSet rset =
				statement.executeQuery("SELECT auctionId FROM item_auction ORDER BY auctionId DESC LIMIT 0, 1");
			if (rset.next())
			{
				_auctionIds.set(rset.getInt(1) + 1);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed loading auctions." + e);
		}
		
		load();
	}
	
	public void load()
	{
		_managerInstances.clear();
		parseDatapackFile("data/ItemAuctions.xml");
		LOGGER.Info(GetType().Name +": Loaded " + _managerInstances.size() + " instances.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		try
		{
			for (Node na = doc.getFirstChild(); na != null; na = na.getNextSibling())
			{
				if ("list".equalsIgnoreCase(na.getNodeName()))
				{
					for (Node nb = na.getFirstChild(); nb != null; nb = nb.getNextSibling())
					{
						if ("instance".equalsIgnoreCase(nb.getNodeName()))
						{
							NamedNodeMap nab = nb.getAttributes();
							int instanceId = int.Parse(nab.getNamedItem("id").getNodeValue());
							if (_managerInstances.containsKey(instanceId))
							{
								throw new Exception("Dublicated instanceId " + instanceId);
							}
							
							ItemAuctionInstance instance = new ItemAuctionInstance(instanceId, _auctionIds, nb);
							_managerInstances.put(instanceId, instance);
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Failed loading auctions from xml." + e);
		}
	}
	
	public void shutdown()
	{
		foreach (ItemAuctionInstance instance in _managerInstances.values())
		{
			instance.shutdown();
		}
	}
	
	public ItemAuctionInstance getManagerInstance(int instanceId)
	{
		return _managerInstances.get(instanceId);
	}
	
	public int getNextAuctionId()
	{
		return _auctionIds.getAndIncrement();
	}
	
	public static void deleteAuction(int auctionId)
	{
		try
		{
			using GameServerDbContext ctx = new();

			{
				PreparedStatement statement = con.prepareStatement("DELETE FROM item_auction WHERE auctionId=?");
				statement.setInt(1, auctionId);
				statement.execute();
			}


			{
				PreparedStatement statement = con.prepareStatement("DELETE FROM item_auction_bid WHERE auctionId=?");
				statement.setInt(1, auctionId);
				statement.execute();
			}
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