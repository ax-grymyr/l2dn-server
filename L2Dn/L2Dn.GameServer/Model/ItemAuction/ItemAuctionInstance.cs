using System.Xml.Linq;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.ItemAuction;

public class ItemAuctionInstance
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemAuctionInstance));
	private readonly SimpleDateFormat DATE_FORMAT = new SimpleDateFormat("HH:mm:ss dd.MM.yy");
	
	private static readonly long START_TIME_SPACE = TimeUnit.MILLISECONDS.convert(1, TimeUnit.MINUTES);
	private static readonly long FINISH_TIME_SPACE = TimeUnit.MILLISECONDS.convert(10, TimeUnit.MINUTES);
	
	// SQL queries
	private const String SELECT_AUCTION_ID_BY_INSTANCE_ID = "SELECT auctionId FROM item_auction WHERE instanceId = ?";
	private const String SELECT_AUCTION_INFO = "SELECT auctionItemId, startingTime, endingTime, auctionStateId FROM item_auction WHERE auctionId = ? ";
	private const String DELETE_AUCTION_INFO_BY_AUCTION_ID = "DELETE FROM item_auction WHERE auctionId = ?";
	private const String DELETE_AUCTION_BID_INFO_BY_AUCTION_ID = "DELETE FROM item_auction_bid WHERE auctionId = ?";
	private const String SELECT_PLAYERS_ID_BY_AUCTION_ID = "SELECT playerObjId, playerBid FROM item_auction_bid WHERE auctionId = ?";
	
	private readonly int _instanceId;
	private ref int _auctionIds;
	private readonly Map<int, ItemAuction> _auctions;
	private readonly List<AuctionItem> _items;
	private readonly AuctionDateGenerator _dateGenerator;
	
	private ItemAuction _currentAuction;
	private ItemAuction _nextAuction;
	private ScheduledFuture _stateTask;
	
	public ItemAuctionInstance(int instanceId, ref int auctionIds, XElement element)
	{
		_instanceId = instanceId;
		_auctionIds = auctionIds;
		_auctions = new();
		_items = new();
		
		NamedNodeMap nanode = node.getAttributes();
		StatSet generatorConfig = new StatSet();
		for (int i = nanode.getLength(); i-- > 0;)
		{
			Node n = nanode.item(i);
			if (n != null)
			{
				generatorConfig.set(n.getNodeName(), n.getNodeValue());
			}
		}
		
		_dateGenerator = new AuctionDateGenerator(generatorConfig);
		
		for (Node na = node.getFirstChild(); na != null; na = na.getNextSibling())
		{
			try
			{
				if ("item".equalsIgnoreCase(na.getNodeName()))
				{
					NamedNodeMap naa = na.getAttributes();
					int auctionItemId = int.parseInt(naa.getNamedItem("auctionItemId").getNodeValue());
					int auctionLength = int.parseInt(naa.getNamedItem("auctionLength").getNodeValue());
					long auctionInitBid = int.parseInt(naa.getNamedItem("auctionInitBid").getNodeValue());
					
					int itemId = int.parseInt(naa.getNamedItem("itemId").getNodeValue());
					int itemCount = int.parseInt(naa.getNamedItem("itemCount").getNodeValue());
					
					if (auctionLength < 1)
					{
						throw new ArgumentException(nameof(auctionLength), "auctionLength < 1 for instanceId: " + _instanceId + ", itemId " + itemId);
					}
					
					StatSet itemExtra = new StatSet();
					AuctionItem item = new AuctionItem(auctionItemId, auctionLength, auctionInitBid, itemId, itemCount, itemExtra);
					
					if (!item.checkItemExists())
					{
						throw new ArgumentException(nameof(itemId), "Item with id " + itemId + " not found");
					}
					
					foreach (AuctionItem tmp in _items)
					{
						if (tmp.getAuctionItemId() == auctionItemId)
						{
							throw new ArgumentException(nameof(auctionItemId), "Dublicated auction item id " + auctionItemId);
						}
					}
					
					_items.Add(item);
					
					for (Node nb = na.getFirstChild(); nb != null; nb = nb.getNextSibling())
					{
						if ("extra".equalsIgnoreCase(nb.getNodeName()))
						{
							NamedNodeMap nab = nb.getAttributes();
							for (int i = nab.getLength(); i-- > 0;)
							{
								Node n = nab.item(i);
								if (n != null)
								{
									itemExtra.set(n.getNodeName(), n.getNodeValue());
								}
							}
						}
					}
				}
			}
			catch (ArgumentException e)
			{
				LOGGER.Warn(GetType().Name + ": Failed loading auction item: " + e);
			}
		}
		
		if (_items.isEmpty())
		{
			throw new ArgumentException(nameof(_items), "No items defined");
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(SELECT_AUCTION_ID_BY_INSTANCE_ID);
			ps.setInt(1, _instanceId);
			ResultSet rset = ps.executeQuery();
			{
				while (rset.next())
				{
					int auctionId = rset.getInt(1);
					try
					{
						ItemAuction auction = loadAuction(auctionId);
						if (auction != null)
						{
							_auctions.put(auctionId, auction);
						}
						else
						{
							ItemAuctionManager.deleteAuction(auctionId);
						}
					}
					catch (Exception e)
					{
						LOGGER.Error(GetType().Name + ": Failed loading auction: " + auctionId, e);
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Failed loading auctions.", e);
			return;
		}
		
		LOGGER.Info(GetType().Name + ": Loaded " + _items.Count + " item(s) and registered " + _auctions.size() + " auction(s) for instance " + _instanceId);
		checkAndSetCurrentAndNextAuction();
	}
	
	public ItemAuction getCurrentAuction()
	{
		return _currentAuction;
	}
	
	public ItemAuction getNextAuction()
	{
		return _nextAuction;
	}
	
	public void shutdown()
	{
		ScheduledFuture<?> stateTask = _stateTask;
		if (stateTask != null)
		{
			stateTask.cancel(false);
		}
	}
	
	private AuctionItem getAuctionItem(int auctionItemId)
	{
		for (int i = _items.size(); i-- > 0;)
		{
			AuctionItem item = _items[i];
			if (item.getAuctionItemId() == auctionItemId)
			{
				return item;
			}
		}
		return null;
	}
	
	protected void checkAndSetCurrentAndNextAuction()
	{
		ItemAuction[] auctions = _auctions.values().ToArray();
		
		ItemAuction currentAuction = null;
		ItemAuction nextAuction = null;
		
		switch (auctions.Length)
		{
			case 0:
			{
				nextAuction = createAuction(System.currentTimeMillis() + START_TIME_SPACE);
				break;
			}
			case 1:
			{
				switch (auctions[0].getAuctionState())
				{
					case ItemAuctionState.CREATED:
					{
						if (auctions[0].getStartingTime() < (System.currentTimeMillis() + START_TIME_SPACE))
						{
							currentAuction = auctions[0];
							nextAuction = createAuction(System.currentTimeMillis() + START_TIME_SPACE);
						}
						else
						{
							nextAuction = auctions[0];
						}
						break;
					}
					case ItemAuctionState.STARTED:
					{
						currentAuction = auctions[0];
						nextAuction = createAuction(Math.max(currentAuction.getEndingTime() + FINISH_TIME_SPACE, System.currentTimeMillis() + START_TIME_SPACE));
						break;
					}
					case ItemAuctionState.FINISHED:
					{
						currentAuction = auctions[0];
						nextAuction = createAuction(System.currentTimeMillis() + START_TIME_SPACE);
						break;
					}
					default:
					{
						throw new ArgumentException();
					}
				}
				break;
			}
			
			default:
			{
				Array.Sort(auctions, Comparator.comparingLong(ItemAuction::getStartingTime).reversed());
				// just to make sure we won't skip any auction because of little different times
				long currentTime = System.currentTimeMillis();
				foreach (ItemAuction auction in auctions)
				{
					if (auction.getAuctionState() == ItemAuctionState.STARTED)
					{
						currentAuction = auction;
						break;
					}
					else if (auction.getStartingTime() <= currentTime)
					{
						currentAuction = auction;
						break; // only first
					}
				}
				foreach (ItemAuction auction in auctions)
				{
					if ((auction.getStartingTime() > currentTime) && (currentAuction != auction))
					{
						nextAuction = auction;
						break;
					}
				}
				if (nextAuction == null)
				{
					nextAuction = createAuction(System.currentTimeMillis() + START_TIME_SPACE);
				}
				break;
			}
		}
		
		_auctions.put(nextAuction.getAuctionId(), nextAuction);
		
		_currentAuction = currentAuction;
		_nextAuction = nextAuction;
		
		if ((currentAuction != null) && (currentAuction.getAuctionState() != ItemAuctionState.FINISHED))
		{
			if (currentAuction.getAuctionState() == ItemAuctionState.STARTED)
			{
				setStateTask(ThreadPool.schedule(new ScheduleAuctionTask(currentAuction), Math.Max(currentAuction.getEndingTime() - System.currentTimeMillis(), 0)));
			}
			else
			{
				setStateTask(ThreadPool.schedule(new ScheduleAuctionTask(currentAuction), Math.Max(currentAuction.getStartingTime() - System.currentTimeMillis(), 0)));
			}
			LOGGER.Info(GetType().Name + ": Schedule current auction " + currentAuction.getAuctionId() + " for instance " + _instanceId);
		}
		else
		{
			setStateTask(ThreadPool.schedule(new ScheduleAuctionTask(nextAuction), Math.Max(nextAuction.getStartingTime() - System.currentTimeMillis(), 0)));
			LOGGER.Info(GetType().Name + ": Schedule next auction " + nextAuction.getAuctionId() + " on " + DATE_FORMAT.format(new Date(nextAuction.getStartingTime())) + " for instance " + _instanceId);
		}
	}
	
	public ItemAuction getAuction(int auctionId)
	{
		return _auctions.get(auctionId);
	}
	
	public List<ItemAuction> getAuctionsByBidder(int bidderObjId)
	{
		ICollection<ItemAuction> auctions = getAuctions();
		List<ItemAuction> stack = new();
		foreach (ItemAuction auction in getAuctions())
		{
			if (auction.getAuctionState() != ItemAuctionState.CREATED)
			{
				ItemAuctionBid bid = auction.getBidFor(bidderObjId);
				if (bid != null)
				{
					stack.add(auction);
				}
			}
		}
		return stack;
	}
	
	public ICollection<ItemAuction> getAuctions()
	{
		ICollection<ItemAuction> auctions;
		
		lock (_auctions)
		{
			auctions = _auctions.values();
		}
		
		return auctions;
	}
	
	private class ScheduleAuctionTask: Runnable
	{
		private readonly ItemAuction _auction;
		
		public ScheduleAuctionTask(ItemAuction auction)
		{
			_auction = auction;
		}
		
		public void run()
		{
			try
			{
				runImpl();
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Failed scheduling auction " + _auction.getAuctionId(), e);
			}
		}
		
		private void runImpl()
		{
			ItemAuctionState state = _auction.getAuctionState();
			switch (state)
			{
				case ItemAuctionState.CREATED:
				{
					if (!_auction.setAuctionState(state, ItemAuctionState.STARTED))
					{
						throw new InvalidOperationException("Could not set auction state: " + ItemAuctionState.STARTED + ", expected: " + state);
					}
					LOGGER.Info(GetType().Name + ": Auction " + _auction.getAuctionId() + " has started for instance " + _auction.getInstanceId());
					checkAndSetCurrentAndNextAuction();
					break;
				}
				case ItemAuctionState.STARTED:
				{
					switch (_auction.getAuctionEndingExtendState())
					{
						case ItemAuctionExtendState.EXTEND_BY_5_MIN:
						{
							if (_auction.getScheduledAuctionEndingExtendState() == ItemAuctionExtendState.INITIAL)
							{
								_auction.setScheduledAuctionEndingExtendState(ItemAuctionExtendState.EXTEND_BY_5_MIN);
								setStateTask(ThreadPool.schedule(this, Math.Max(_auction.getEndingTime() - System.currentTimeMillis(), 0)));
								return;
							}
							break;
						}
						case ItemAuctionExtendState.EXTEND_BY_3_MIN:
						{
							if (_auction.getScheduledAuctionEndingExtendState() != ItemAuctionExtendState.EXTEND_BY_3_MIN)
							{
								_auction.setScheduledAuctionEndingExtendState(ItemAuctionExtendState.EXTEND_BY_3_MIN);
								setStateTask(ThreadPool.schedule(this, Math.Max(_auction.getEndingTime() - System.currentTimeMillis(), 0)));
								return;
							}
							break;
						}
						case ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A:
						{
							if (_auction.getScheduledAuctionEndingExtendState() != ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B)
							{
								_auction.setScheduledAuctionEndingExtendState(ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B);
								setStateTask(ThreadPool.schedule(this, Math.Max(_auction.getEndingTime() - System.currentTimeMillis(), 0)));
								return;
							}
							break;
						}
						case ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B:
						{
							if (_auction.getScheduledAuctionEndingExtendState() != ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A)
							{
								_auction.setScheduledAuctionEndingExtendState(ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A);
								setStateTask(ThreadPool.schedule(this, Math.Max(_auction.getEndingTime() - System.currentTimeMillis(), 0)));
								return;
							}
						}
					}
					
					if (!_auction.setAuctionState(state, ItemAuctionState.FINISHED))
					{
						throw new IllegalStateException("Could not set auction state: " + ItemAuctionState.FINISHED + ", expected: " + state);
					}
					
					onAuctionFinished(_auction);
					checkAndSetCurrentAndNextAuction();
					break;
				}
				
				default:
				{
					throw new InvalidOperationException("Invalid state: " + state);
				}
			}
		}
	}
	
	protected void onAuctionFinished(ItemAuction auction)
	{
		auction.broadcastToAllBiddersInternal(new SystemMessage(SystemMessageId.S1_S_AUCTION_HAS_ENDED).addInt(auction.getAuctionId()));
		
		ItemAuctionBid bid = auction.getHighestBid();
		if (bid != null)
		{
			Item item = auction.createNewItemInstance();
			Player player = bid.getPlayer();
			if (player != null)
			{
				player.getWarehouse().addItem("ItemAuction", item, null, null);
				player.sendPacket(SystemMessageId.YOU_HAVE_BID_THE_HIGHEST_PRICE_AND_HAVE_WON_THE_ITEM_THE_ITEM_CAN_BE_FOUND_IN_YOUR_PERSONAL_WAREHOUSE);
				
				LOGGER.Info(GetType().Name + ": Auction " + auction.getAuctionId() + " has finished. Highest bid by " + player.getName() + " for instance " + _instanceId);
			}
			else
			{
				item.setOwnerId(bid.getPlayerObjId());
				item.setItemLocation(ItemLocation.WAREHOUSE);
				item.updateDatabase();
				World.getInstance().removeObject(item);
				
				LOGGER.Info(GetType().Name + ": Auction " + auction.getAuctionId() + " has finished. Highest bid by " + CharInfoTable.getInstance().getNameById(bid.getPlayerObjId()) + " for instance " + _instanceId);
			}
			
			// Clean all canceled bids
			auction.clearCanceledBids();
		}
		else
		{
			LOGGER.Info(GetType().Name + ": Auction " + auction.getAuctionId() + " has finished. There have not been any bid for instance " + _instanceId);
		}
	}
	
	protected void setStateTask(ScheduledFuture<?> future)
	{
		ScheduledFuture<?> stateTask = _stateTask;
		if (stateTask != null)
		{
			stateTask.cancel(false);
		}
		
		_stateTask = future;
	}
	
	private ItemAuction createAuction(long after)
	{
		AuctionItem auctionItem = _items[Rnd.get(_items.size())];
		long startingTime = _dateGenerator.nextDate(after);
		long endingTime = startingTime + TimeUnit.MILLISECONDS.convert(auctionItem.getAuctionLength(), TimeUnit.MINUTES);
		ItemAuction auction = new ItemAuction(_auctionIds.getAndIncrement(), _instanceId, startingTime, endingTime, auctionItem);
		auction.storeMe();
		return auction;
	}
	
	private ItemAuction loadAuction(int auctionId)
	{
		try
		{
			using GameServerDbContext ctx = new();
			int auctionItemId = 0;
			long startingTime = 0;
			long endingTime = 0;
			byte auctionStateId = 0;
			PreparedStatement ps = con.prepareStatement(SELECT_AUCTION_INFO);
			{
				ps.setInt(1, auctionId);
				ResultSet rset = ps.executeQuery();
				{
					if (!rset.next())
					{
						LOGGER.Warn(GetType().Name + ": Auction data not found for auction: " + auctionId);
						return null;
					}
					auctionItemId = rset.getInt(1);
					startingTime = rset.getLong(2);
					endingTime = rset.getLong(3);
					auctionStateId = rset.getByte(4);
				}
			}
			
			if (startingTime >= endingTime)
			{
				LOGGER.Warn(GetType().Name + ": Invalid starting/ending paramaters for auction: " + auctionId);
				return null;
			}
			
			AuctionItem auctionItem = getAuctionItem(auctionItemId);
			if (auctionItem == null)
			{
				LOGGER.Warn(GetType().Name + ": AuctionItem: " + auctionItemId + ", not found for auction: " + auctionId);
				return null;
			}
			
			ItemAuctionState auctionState = ItemAuctionState.stateForStateId(auctionStateId);
			if (auctionState == null)
			{
				LOGGER.Warn(GetType().Name + ": Invalid auctionStateId: " + auctionStateId + ", for auction: " + auctionId);
				return null;
			}
			
			if ((auctionState == ItemAuctionState.FINISHED) && (startingTime < (System.currentTimeMillis() - TimeUnit.MILLISECONDS.convert(Config.ALT_ITEM_AUCTION_EXPIRED_AFTER, TimeUnit.DAYS))))
			{
				LOGGER.Info(GetType().Name + ": Clearing expired auction: " + auctionId);
				{
					PreparedStatement ps = con.prepareStatement(DELETE_AUCTION_INFO_BY_AUCTION_ID);
					ps.setInt(1, auctionId);
					ps.execute();
				}
				
				{
					PreparedStatement ps = con.prepareStatement(DELETE_AUCTION_BID_INFO_BY_AUCTION_ID);
					ps.setInt(1, auctionId);
					ps.execute();
				}
				return null;
			}
			
			List<ItemAuctionBid> auctionBids = new();
			try
			{
				PreparedStatement ps = con.prepareStatement(SELECT_PLAYERS_ID_BY_AUCTION_ID);
				ps.setInt(1, auctionId);
				{
					ResultSet rs = ps.executeQuery();
					while (rs.next())
					{
						int playerObjId = rs.getInt(1);
						long playerBid = rs.getLong(2);
						ItemAuctionBid bid = new ItemAuctionBid(playerObjId, playerBid);
						auctionBids.Add(bid);
					}
				}
			}
			return new ItemAuction(auctionId, _instanceId, startingTime, endingTime, auctionItem, auctionBids, auctionState);
		}
	}
}
