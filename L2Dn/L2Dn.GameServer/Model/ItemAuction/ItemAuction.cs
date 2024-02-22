using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using NLog;

namespace L2Dn.GameServer.Model.ItemAuction;

public class ItemAuction
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemAuction));
	private static readonly TimeSpan ENDING_TIME_EXTEND_5 = TimeSpan.FromMinutes(5);
	private static readonly TimeSpan ENDING_TIME_EXTEND_3 = TimeSpan.FromMinutes(3);
	
	private readonly int _auctionId;
	private readonly int _instanceId;
	private readonly long _startingTime;
	private long _endingTime;
	private readonly AuctionItem _auctionItem;
	private readonly List<ItemAuctionBid> _auctionBids;
	private readonly Object _auctionStateLock;
	
	private ItemAuctionState _auctionState;
	private ItemAuctionExtendState _scheduledAuctionEndingExtendState;
	private ItemAuctionExtendState _auctionEndingExtendState;
	
	private readonly ItemInfo _itemInfo;
	
	private ItemAuctionBid _highestBid;
	private int _lastBidPlayerObjId;
	
	// SQL
	private const String DELETE_ITEM_AUCTION_BID = "DELETE FROM item_auction_bid WHERE auctionId = ? AND playerObjId = ?";
	private const String INSERT_ITEM_AUCTION_BID = "INSERT INTO item_auction_bid (auctionId, playerObjId, playerBid) VALUES (?, ?, ?) ON DUPLICATE KEY UPDATE playerBid = ?";
	
	public ItemAuction(int auctionId, int instanceId, long startingTime, long endingTime, AuctionItem auctionItem):this(auctionId, instanceId, startingTime, endingTime, auctionItem, new(), ItemAuctionState.CREATED) 
	{
	}
	
	public ItemAuction(int auctionId, int instanceId, long startingTime, long endingTime, AuctionItem auctionItem, List<ItemAuctionBid> auctionBids, ItemAuctionState auctionState)
	{
		_auctionId = auctionId;
		_instanceId = instanceId;
		_startingTime = startingTime;
		_endingTime = endingTime;
		_auctionItem = auctionItem;
		_auctionBids = auctionBids;
		_auctionState = auctionState;
		_auctionStateLock = new Object();
		_scheduledAuctionEndingExtendState = ItemAuctionExtendState.INITIAL;
		_auctionEndingExtendState = ItemAuctionExtendState.INITIAL;
		
		Item item = _auctionItem.createNewItemInstance();
		_itemInfo = new ItemInfo(item);
		World.getInstance().removeObject(item);
		
		foreach (ItemAuctionBid bid in _auctionBids)
		{
			if ((_highestBid == null) || (_highestBid.getLastBid() < bid.getLastBid()))
			{
				_highestBid = bid;
			}
		}
	}
	
	public ItemAuctionState getAuctionState()
	{
		ItemAuctionState auctionState;
		
		lock (_auctionStateLock)
		{
			auctionState = _auctionState;
		}
		
		return auctionState;
	}
	
	public bool setAuctionState(ItemAuctionState expected, ItemAuctionState wanted)
	{
		lock (_auctionStateLock)
		{
			if (_auctionState != expected)
			{
				return false;
			}
			
			_auctionState = wanted;
			storeMe();
			return true;
		}
	}
	
	public int getAuctionId()
	{
		return _auctionId;
	}
	
	public int getInstanceId()
	{
		return _instanceId;
	}
	
	public ItemInfo getItemInfo()
	{
		return _itemInfo;
	}
	
	public Item createNewItemInstance()
	{
		return _auctionItem.createNewItemInstance();
	}
	
	public long getAuctionInitBid()
	{
		return _auctionItem.getAuctionInitBid();
	}
	
	public ItemAuctionBid getHighestBid()
	{
		return _highestBid;
	}
	
	public ItemAuctionExtendState getAuctionEndingExtendState()
	{
		return _auctionEndingExtendState;
	}
	
	public ItemAuctionExtendState getScheduledAuctionEndingExtendState()
	{
		return _scheduledAuctionEndingExtendState;
	}
	
	public void setScheduledAuctionEndingExtendState(ItemAuctionExtendState state)
	{
		_scheduledAuctionEndingExtendState = state;
	}
	
	public long getStartingTime()
	{
		return _startingTime;
	}
	
	public long getEndingTime()
	{
		return _endingTime;
	}
	
	public long getStartingTimeRemaining()
	{
		return Math.Max(_endingTime - System.currentTimeMillis(), 0);
	}
	
	public long getFinishingTimeRemaining()
	{
		return Math.Max(_endingTime - System.currentTimeMillis(), 0);
	}
	
	public void storeMe()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(
				"INSERT INTO item_auction (auctionId,instanceId,auctionItemId,startingTime,endingTime,auctionStateId) VALUES (?,?,?,?,?,?) ON DUPLICATE KEY UPDATE auctionStateId=?");
			statement.setInt(1, _auctionId);
			statement.setInt(2, _instanceId);
			statement.setInt(3, _auctionItem.getAuctionItemId());
			statement.setLong(4, _startingTime);
			statement.setLong(5, _endingTime);
			statement.setByte(6, _auctionState.getStateId());
			statement.setByte(7, _auctionState.getStateId());
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}
	
	public int getAndSetLastBidPlayerObjectId(int playerObjId)
	{
		int lastBid = _lastBidPlayerObjId;
		_lastBidPlayerObjId = playerObjId;
		return lastBid;
	}
	
	private void updatePlayerBid(ItemAuctionBid bid, bool delete)
	{
		updatePlayerBidInternal(bid, delete);
	}
	
	private void updatePlayerBidInternal(ItemAuctionBid bid, bool delete)
	{
		String query = delete ? DELETE_ITEM_AUCTION_BID : INSERT_ITEM_AUCTION_BID;
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(query);
			ps.setInt(1, _auctionId);
			ps.setInt(2, bid.getPlayerObjId());
			if (!delete)
			{
				ps.setLong(3, bid.getLastBid());
				ps.setLong(4, bid.getLastBid());
			}
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}
	
	public void registerBid(Player player, long newBid)
	{
		if (player == null)
		{
			throw new ArgumentNullException();
		}
		
		if (newBid < _auctionItem.getAuctionInitBid())
		{
			player.sendPacket(SystemMessageId.YOUR_BID_PRICE_MUST_BE_HIGHER_THAN_THE_MINIMUM_PRICE_CURRENTLY_BEING_BID);
			return;
		}
		
		if (newBid > 100000000000L)
		{
			player.sendPacket(SystemMessageId.THE_HIGHEST_BID_IS_OVER_999_9_BILLION_THEREFORE_YOU_CANNOT_PLACE_A_BID);
			return;
		}
		
		if (getAuctionState() != ItemAuctionState.STARTED)
		{
			return;
		}
		
		int playerObjId = player.getObjectId();
		
		lock (_auctionBids)
		{
			if ((_highestBid != null) && (newBid < _highestBid.getLastBid()))
			{
				player.sendPacket(SystemMessageId.YOUR_BID_MUST_BE_HIGHER_THAN_THE_CURRENT_HIGHEST_BID);
				return;
			}
			
			ItemAuctionBid bid = getBidFor(playerObjId);
			if (bid == null)
			{
				if (!reduceItemCount(player, newBid))
				{
					player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_ADENA_FOR_THIS_BID);
					return;
				}
				
				bid = new ItemAuctionBid(playerObjId, newBid);
				_auctionBids.Add(bid);
			}
			else
			{
				if (!bid.isCanceled())
				{
					if (newBid < bid.getLastBid()) // just another check
					{
						player.sendPacket(SystemMessageId.YOUR_BID_MUST_BE_HIGHER_THAN_THE_CURRENT_HIGHEST_BID);
						return;
					}
					
					if (!reduceItemCount(player, newBid - bid.getLastBid()))
					{
						player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_ADENA_FOR_THIS_BID);
						return;
					}
				}
				else if (!reduceItemCount(player, newBid))
				{
					player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_ADENA_FOR_THIS_BID);
					return;
				}
				
				bid.setLastBid(newBid);
			}
			
			onPlayerBid(player, bid);
			updatePlayerBid(bid, false);
			
			SystemMessage sm = new SystemMessage(SystemMessageId.YOU_HAVE_SUBMITTED_A_BID_FOR_THE_AUCTION_OF_S1);
			sm.addLong(newBid);
			player.sendPacket(sm);
		}
	}
	
	private void onPlayerBid(Player player, ItemAuctionBid bid)
	{
		if (_highestBid == null)
		{
			_highestBid = bid;
		}
		else if (_highestBid.getLastBid() < bid.getLastBid())
		{
			Player old = _highestBid.getPlayer();
			if (old != null)
			{
				old.sendPacket(SystemMessageId.YOU_WERE_OUTBID_THE_NEW_HIGHEST_BID_IS_S1_ADENA);
			}
			
			_highestBid = bid;
		}
		
		if ((_endingTime - System.currentTimeMillis()) <= (1000 * 60 * 10)) // 10 minutes
		{
			switch (_auctionEndingExtendState)
			{
				case ItemAuctionExtendState.INITIAL:
				{
					_auctionEndingExtendState = ItemAuctionExtendState.EXTEND_BY_5_MIN;
					_endingTime += ENDING_TIME_EXTEND_5;
					broadcastToAllBidders(new SystemMessage(SystemMessageId.BIDDER_EXISTS_THE_AUCTION_TIME_HAS_BEEN_EXTENDED_FOR_5_MIN));
					break;
				}
				case ItemAuctionExtendState.EXTEND_BY_5_MIN:
				{
					if (getAndSetLastBidPlayerObjectId(player.getObjectId()) != player.getObjectId())
					{
						_auctionEndingExtendState = ItemAuctionExtendState.EXTEND_BY_3_MIN;
						_endingTime += ENDING_TIME_EXTEND_3;
						broadcastToAllBidders(new SystemMessage(SystemMessageId.BIDDER_EXISTS_AUCTION_TIME_HAS_BEEN_EXTENDED_FOR_3_MIN));
					}
					break;
				}
				case ItemAuctionExtendState.EXTEND_BY_3_MIN:
				{
					if ((Config.ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID > 0) && (getAndSetLastBidPlayerObjectId(player.getObjectId()) != player.getObjectId()))
					{
						_auctionEndingExtendState = ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A;
						_endingTime += Config.ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID;
					}
					break;
				}
				case ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A:
				{
					if ((getAndSetLastBidPlayerObjectId(player.getObjectId()) != player.getObjectId()) && (_scheduledAuctionEndingExtendState == ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B))
					{
						_auctionEndingExtendState = ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B;
						_endingTime += Config.ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID;
					}
					break;
				}
				case ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B:
				{
					if ((getAndSetLastBidPlayerObjectId(player.getObjectId()) != player.getObjectId()) && (_scheduledAuctionEndingExtendState == ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A))
					{
						_endingTime += Config.ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID;
						_auctionEndingExtendState = ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A;
					}
				}
			}
		}
	}
	
	public void broadcastToAllBidders(ServerPacket packet)
	{
		ThreadPool.execute(() => broadcastToAllBiddersInternal(packet));
	}
	
	public void broadcastToAllBiddersInternal(ServerPacket packet)
	{
		for (int i = _auctionBids.Count; i-- > 0;)
		{
			ItemAuctionBid bid = _auctionBids[i];
			if (bid != null)
			{
				Player player = bid.getPlayer();
				if (player != null)
				{
					player.sendPacket(packet);
				}
			}
		}
	}
	
	public bool cancelBid(Player player)
	{
		if (player == null)
		{
			throw new ArgumentNullException();
		}
		
		switch (getAuctionState())
		{
			case ItemAuctionState.CREATED:
			{
				return false;
			}
			case ItemAuctionState.FINISHED:
			{
				if (_startingTime < (System.currentTimeMillis() - TimeUnit.MILLISECONDS.convert(Config.ALT_ITEM_AUCTION_EXPIRED_AFTER, TimeUnit.DAYS)))
				{
					return false;
				}
				break;
			}
		}
		
		int playerObjId = player.getObjectId();
		
		lock (_auctionBids)
		{
			if (_highestBid == null)
			{
				return false;
			}
			
			int bidIndex = getBidIndexFor(playerObjId);
			if (bidIndex == -1)
			{
				return false;
			}
			
			ItemAuctionBid bid = _auctionBids[bidIndex];
			if (bid.getPlayerObjId() == _highestBid.getPlayerObjId())
			{
				// can't return winning bid
				if (getAuctionState() == ItemAuctionState.FINISHED)
				{
					return false;
				}
				
				player.sendPacket(SystemMessageId.YOU_CURRENTLY_HAVE_THE_HIGHEST_BID);
				return true;
			}
			
			if (bid.isCanceled())
			{
				return false;
			}
			
			increaseItemCount(player, bid.getLastBid());
			bid.cancelBid();
			
			// delete bid from database if auction already finished
			updatePlayerBid(bid, getAuctionState() == ItemAuctionState.FINISHED);
			
			player.sendPacket(SystemMessageId.YOU_HAVE_CANCELED_YOUR_BID);
		}
		return true;
	}
	
	public void clearCanceledBids()
	{
		if (getAuctionState() != ItemAuctionState.FINISHED)
		{
			throw new InvalidOperationException("Attempt to clear canceled bids for non-finished auction");
		}
		
		lock (_auctionBids)
		{
			foreach (ItemAuctionBid bid in _auctionBids)
			{
				if ((bid == null) || !bid.isCanceled())
				{
					continue;
				}
				updatePlayerBid(bid, true);
			}
		}
	}
	
	private bool reduceItemCount(Player player, long count)
	{
		if (!player.reduceAdena("ItemAuction", count, player, true))
		{
			player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_ADENA_FOR_THIS_BID);
			return false;
		}
		return true;
	}
	
	private void increaseItemCount(Player player, long count)
	{
		player.addAdena("ItemAuction", count, player, true);
	}
	
	/**
	 * Returns the last bid for the given player or -1 if he did not made one yet.
	 * @param player The player that made the bid
	 * @return The last bid the player made or -1
	 */
	public long getLastBid(Player player)
	{
		ItemAuctionBid bid = getBidFor(player.getObjectId());
		return bid != null ? bid.getLastBid() : -1L;
	}
	
	public ItemAuctionBid getBidFor(int playerObjId)
	{
		int index = getBidIndexFor(playerObjId);
		return index != -1 ? _auctionBids[index] : null;
	}
	
	private int getBidIndexFor(int playerObjId)
	{
		for (int i = _auctionBids.Count; i-- > 0;)
		{
			ItemAuctionBid bid = _auctionBids[i];
			if ((bid != null) && (bid.getPlayerObjId() == playerObjId))
			{
				return i;
			}
		}
		return -1;
	}
}
