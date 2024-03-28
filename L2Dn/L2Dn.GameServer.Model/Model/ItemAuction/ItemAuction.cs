using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Packets;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.ItemAuction;

public class ItemAuction
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemAuction));
	private static readonly TimeSpan ENDING_TIME_EXTEND_5 = TimeSpan.FromMinutes(5);
	private static readonly TimeSpan ENDING_TIME_EXTEND_3 = TimeSpan.FromMinutes(3);
	
	private readonly int _auctionId;
	private readonly int _instanceId;
	private readonly DateTime _startingTime;
	private DateTime _endingTime;
	private readonly AuctionItem _auctionItem;
	private readonly List<ItemAuctionBid> _auctionBids;
	private readonly object _auctionStateLock;
	
	private ItemAuctionState _auctionState;
	private ItemAuctionExtendState _scheduledAuctionEndingExtendState;
	private ItemAuctionExtendState _auctionEndingExtendState;
	
	private readonly ItemInfo _itemInfo;
	
	private ItemAuctionBid _highestBid;
	private int _lastBidPlayerObjId;
	
	public ItemAuction(int auctionId, int instanceId, DateTime startingTime, DateTime endingTime, AuctionItem auctionItem):this(auctionId, instanceId, startingTime, endingTime, auctionItem, new(), ItemAuctionState.CREATED) 
	{
	}
	
	public ItemAuction(int auctionId, int instanceId, DateTime startingTime, DateTime endingTime, AuctionItem auctionItem, List<ItemAuctionBid> auctionBids, ItemAuctionState auctionState)
	{
		_auctionId = auctionId;
		_instanceId = instanceId;
		_startingTime = startingTime;
		_endingTime = endingTime;
		_auctionItem = auctionItem;
		_auctionBids = auctionBids;
		_auctionState = auctionState;
		_auctionStateLock = new object();
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
	
	public DateTime getStartingTime()
	{
		return _startingTime;
	}
	
	public DateTime getEndingTime()
	{
		return _endingTime;
	}

	public TimeSpan getStartingTimeRemaining()
	{
		TimeSpan span = _endingTime - DateTime.UtcNow; // TODO maybe must be _startingTime here
		return span < TimeSpan.Zero ? TimeSpan.Zero : span;
	}

	public TimeSpan getFinishingTimeRemaining()
	{
		TimeSpan span = _endingTime - DateTime.UtcNow;
		return span < TimeSpan.Zero ? TimeSpan.Zero : span;
	}
	
	public void storeMe()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			Db.ItemAuction? record = ctx.ItemAuctions.SingleOrDefault(r => r.AuctionId == _auctionId);
			if (record is null)
			{
				record = new Db.ItemAuction();
				record.AuctionId = _auctionId;
				ctx.ItemAuctions.Add(record);
			}

			record.InstanceId = _instanceId;
			record.AuctionItemId = _auctionItem.getAuctionItemId();
			record.StartingTime = _startingTime;
			record.EndingTime = _endingTime;
			record.AuctionStateId = (byte)_auctionState;
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
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
		try
		{
			using GameServerDbContext ctx = new();
			int playerObjId = bid.getPlayerObjId();

			if (delete)
			{
				ctx.ItemAuctionBids.Where(r => r.AuctionId == _auctionId && r.CharacterId == playerObjId)
					.ExecuteDelete();
			}
			else
			{
				Db.ItemAuctionBid? record =
					ctx.ItemAuctionBids.SingleOrDefault(r => r.AuctionId == _auctionId && r.CharacterId == playerObjId);
				if (record is null)
				{
					record = new Db.ItemAuctionBid
					{
						AuctionId = _auctionId,
						CharacterId = playerObjId
					};
					
					ctx.ItemAuctionBids.Add(record);
				}

				record.Bid = bid.getLastBid();
				ctx.SaveChanges();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
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
			
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_SUBMITTED_A_BID_FOR_THE_AUCTION_OF_S1);
			sm.Params.addLong(newBid);
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
		
		if ((_endingTime - DateTime.UtcNow) <= TimeSpan.FromMinutes(10)) // 10 minutes
		{
			switch (_auctionEndingExtendState)
			{
				case ItemAuctionExtendState.INITIAL:
				{
					_auctionEndingExtendState = ItemAuctionExtendState.EXTEND_BY_5_MIN;
					_endingTime += ENDING_TIME_EXTEND_5;
					broadcastToAllBidders(new SystemMessagePacket(SystemMessageId.BIDDER_EXISTS_THE_AUCTION_TIME_HAS_BEEN_EXTENDED_FOR_5_MIN));
					break;
				}
				case ItemAuctionExtendState.EXTEND_BY_5_MIN:
				{
					if (getAndSetLastBidPlayerObjectId(player.getObjectId()) != player.getObjectId())
					{
						_auctionEndingExtendState = ItemAuctionExtendState.EXTEND_BY_3_MIN;
						_endingTime += ENDING_TIME_EXTEND_3;
						broadcastToAllBidders(new SystemMessagePacket(SystemMessageId.BIDDER_EXISTS_AUCTION_TIME_HAS_BEEN_EXTENDED_FOR_3_MIN));
					}
					break;
				}
				case ItemAuctionExtendState.EXTEND_BY_3_MIN:
				{
					if ((Config.ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID > 0) && (getAndSetLastBidPlayerObjectId(player.getObjectId()) != player.getObjectId()))
					{
						_auctionEndingExtendState = ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A;
						_endingTime += TimeSpan.FromMilliseconds(Config.ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID);
					}
					break;
				}
				case ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A:
				{
					if ((getAndSetLastBidPlayerObjectId(player.getObjectId()) != player.getObjectId()) && (_scheduledAuctionEndingExtendState == ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B))
					{
						_auctionEndingExtendState = ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B;
						_endingTime += TimeSpan.FromMilliseconds(Config.ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID);
					}
					break;
				}
				case ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B:
				{
					if ((getAndSetLastBidPlayerObjectId(player.getObjectId()) != player.getObjectId()) && (_scheduledAuctionEndingExtendState == ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A))
					{
						_endingTime += TimeSpan.FromMilliseconds(Config.ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID);
						_auctionEndingExtendState = ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A;
					}

					break;
				}
			}
		}
	}
	
	public void broadcastToAllBidders<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		ThreadPool.execute(() => broadcastToAllBiddersInternal(packet));
	}
	
	public void broadcastToAllBiddersInternal<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
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
				if (_startingTime < (DateTime.UtcNow - TimeSpan.FromDays(Config.ALT_ITEM_AUCTION_EXPIRED_AFTER)))
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