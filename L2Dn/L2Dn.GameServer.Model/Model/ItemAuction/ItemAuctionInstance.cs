using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.ItemAuction;

public class ItemAuctionInstance
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemAuctionInstance));

	private static readonly TimeSpan START_TIME_SPACE = TimeSpan.FromMinutes(1);
	private static readonly TimeSpan FINISH_TIME_SPACE = TimeSpan.FromMinutes(10);

	private readonly int _instanceId;
	private readonly AtomicInteger _auctionIds;
	private readonly Map<int, ItemAuction> _auctions;
	private readonly List<AuctionItem> _items;
	private readonly AuctionDateGenerator _dateGenerator;

	private ItemAuction? _currentAuction;
	private ItemAuction? _nextAuction;
	private ScheduledFuture? _stateTask;

	public ItemAuctionInstance(int instanceId, AtomicInteger auctionIds, XElement element)
	{
		_instanceId = instanceId;
		_auctionIds = auctionIds;
		_auctions = new();
		_items = new();

		StatSet generatorConfig = new StatSet(element);
		_dateGenerator = new AuctionDateGenerator(generatorConfig);

		element.Elements("item").ForEach(el =>
		{
			try
			{
				int auctionItemId = el.GetAttributeValueAsInt32("auctionItemId");
				int auctionLength = el.GetAttributeValueAsInt32("auctionLength");
				long auctionInitBid = el.GetAttributeValueAsInt32("auctionInitBid");

				int itemId = el.GetAttributeValueAsInt32("itemId");
				int itemCount = el.GetAttributeValueAsInt32("itemCount");

				if (auctionLength < 1)
				{
					throw new ArgumentException(nameof(auctionLength),
						"auctionLength < 1 for instanceId: " + _instanceId + ", itemId " + itemId);
				}

				StatSet itemExtra = new StatSet();
				AuctionItem item = new AuctionItem(auctionItemId, auctionLength, auctionInitBid, itemId, itemCount,
					itemExtra);

				if (!item.checkItemExists())
				{
					throw new ArgumentException(nameof(itemId), "Item with id " + itemId + " not found");
				}

				foreach (AuctionItem tmp in _items)
				{
					if (tmp.getAuctionItemId() == auctionItemId)
					{
						throw new ArgumentException(nameof(auctionItemId),
							"Dublicated auction item id " + auctionItemId);
					}
				}

				_items.Add(item);

				el.Elements("extra").Attributes().ForEach(a =>
				{
					itemExtra.set(a.Name.LocalName, a.Value);
				});
			}
			catch (ArgumentException e)
			{
				LOGGER.Error(GetType().Name + ": Failed loading auction item: " + e);
			}
		});

		if (_items.Count == 0)
		{
			throw new ArgumentException(nameof(_items), "No items defined");
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query = ctx.ItemAuctions.Where(r => r.InstanceId == _instanceId);
			foreach (var record in query)
			{
				int auctionId = record.AuctionId;
				try
				{
					ItemAuction? auction = loadAuction(auctionId);
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
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Failed loading auctions.", e);
			return;
		}

		LOGGER.Info(GetType().Name + ": Loaded " + _items.Count + " item(s) and registered " + _auctions.Count + " auction(s) for instance " + _instanceId);
		checkAndSetCurrentAndNextAuction();
	}

	public ItemAuction? getCurrentAuction()
	{
		return _currentAuction;
	}

	public ItemAuction? getNextAuction()
	{
		return _nextAuction;
	}

	public void shutdown()
	{
		ScheduledFuture? stateTask = _stateTask;
		if (stateTask != null)
		{
			stateTask.cancel(false);
		}
	}

	private AuctionItem? getAuctionItem(int auctionItemId)
	{
		for (int i = _items.Count; i-- > 0;)
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
		ItemAuction[] auctions = _auctions.Values.ToArray();

		ItemAuction? currentAuction = null;
		ItemAuction? nextAuction = null;

		switch (auctions.Length)
		{
			case 0:
			{
				nextAuction = createAuction(DateTime.UtcNow + START_TIME_SPACE);
				break;
			}
			case 1:
			{
				switch (auctions[0].getAuctionState())
				{
					case ItemAuctionState.CREATED:
					{
						if (auctions[0].getStartingTime() < DateTime.UtcNow + START_TIME_SPACE)
						{
							currentAuction = auctions[0];
							nextAuction = createAuction(DateTime.UtcNow + START_TIME_SPACE);
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
						nextAuction = createAuction(Algorithms.Max(currentAuction.getEndingTime() + FINISH_TIME_SPACE, DateTime.UtcNow + START_TIME_SPACE));
						break;
					}
					case ItemAuctionState.FINISHED:
					{
						currentAuction = auctions[0];
						nextAuction = createAuction(DateTime.UtcNow + START_TIME_SPACE);
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
				Array.Sort(auctions, (a, b) => -a.getStartingTime().CompareTo(b.getStartingTime()));
				// just to make sure we won't skip any auction because of little different times
				DateTime currentTime = DateTime.UtcNow;
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
					if (auction.getStartingTime() > currentTime && currentAuction != auction)
					{
						nextAuction = auction;
						break;
					}
				}
				if (nextAuction == null)
				{
					nextAuction = createAuction(DateTime.UtcNow + START_TIME_SPACE);
				}
				break;
			}
		}

		_auctions.put(nextAuction.getAuctionId(), nextAuction);

		_currentAuction = currentAuction;
		_nextAuction = nextAuction;

		if (currentAuction != null && currentAuction.getAuctionState() != ItemAuctionState.FINISHED)
		{
			if (currentAuction.getAuctionState() == ItemAuctionState.STARTED)
			{
				TimeSpan delay = nextAuction.getEndingTime() - DateTime.UtcNow;
				if (delay < TimeSpan.Zero)
					delay = TimeSpan.Zero;

				setStateTask(ThreadPool.schedule(new ScheduleAuctionTask(this, currentAuction), delay));
			}
			else
			{
				TimeSpan delay = nextAuction.getStartingTime() - DateTime.UtcNow;
				if (delay < TimeSpan.Zero)
					delay = TimeSpan.Zero;

				setStateTask(ThreadPool.schedule(new ScheduleAuctionTask(this, currentAuction), delay));
			}

			LOGGER.Info(GetType().Name + ": Schedule current auction " + currentAuction.getAuctionId() +
			            " for instance " + _instanceId);
		}
		else
		{
			TimeSpan delay = nextAuction.getStartingTime() - DateTime.UtcNow;
			if (delay < TimeSpan.Zero)
				delay = TimeSpan.Zero;

			setStateTask(ThreadPool.schedule(new ScheduleAuctionTask(this, nextAuction), delay));

			LOGGER.Info(GetType().Name + ": Schedule next auction " + nextAuction.getAuctionId() + " on " +
			            nextAuction.getStartingTime().ToString("HH:mm:ss dd.MM.yy") + " for instance " +
			            _instanceId);
		}
	}

	public ItemAuction? getAuction(int auctionId)
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
				ItemAuctionBid? bid = auction.getBidFor(bidderObjId);
				if (bid != null)
				{
					stack.Add(auction);
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
			auctions = _auctions.Values;
		}

		return auctions;
	}

	private class ScheduleAuctionTask: Runnable
	{
		private readonly ItemAuctionInstance _instance;
		private readonly ItemAuction _auction;

		public ScheduleAuctionTask(ItemAuctionInstance instance, ItemAuction auction)
		{
			_instance = instance;
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
					_instance.checkAndSetCurrentAndNextAuction();
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
								_instance.setStateTask(ThreadPool.schedule(this, Algorithms.Max(_auction.getEndingTime() - DateTime.UtcNow, TimeSpan.Zero)));
								return;
							}
							break;
						}
						case ItemAuctionExtendState.EXTEND_BY_3_MIN:
						{
							if (_auction.getScheduledAuctionEndingExtendState() != ItemAuctionExtendState.EXTEND_BY_3_MIN)
							{
								_auction.setScheduledAuctionEndingExtendState(ItemAuctionExtendState.EXTEND_BY_3_MIN);
								_instance.setStateTask(ThreadPool.schedule(this, Algorithms.Max(_auction.getEndingTime() - DateTime.UtcNow, TimeSpan.Zero)));
								return;
							}
							break;
						}
						case ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A:
						{
							if (_auction.getScheduledAuctionEndingExtendState() != ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B)
							{
								_auction.setScheduledAuctionEndingExtendState(ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B);
								_instance.setStateTask(ThreadPool.schedule(this, Algorithms.Max(_auction.getEndingTime() - DateTime.UtcNow, TimeSpan.Zero)));
								return;
							}
							break;
						}
						case ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_B:
						{
							if (_auction.getScheduledAuctionEndingExtendState() != ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A)
							{
								_auction.setScheduledAuctionEndingExtendState(ItemAuctionExtendState.EXTEND_BY_CONFIG_PHASE_A);
								_instance.setStateTask(ThreadPool.schedule(this, Algorithms.Max(_auction.getEndingTime() - DateTime.UtcNow, TimeSpan.Zero)));
								return;
							}

							break;
						}
					}

					if (!_auction.setAuctionState(state, ItemAuctionState.FINISHED))
					{
						throw new InvalidOperationException("Could not set auction state: " +
						                                    ItemAuctionState.FINISHED + ", expected: " + state);
					}

					_instance.onAuctionFinished(_auction);
					_instance.checkAndSetCurrentAndNextAuction();
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
		var sm = new SystemMessagePacket(SystemMessageId.S1_S_AUCTION_HAS_ENDED);
		sm.Params.addInt(auction.getAuctionId());
		auction.broadcastToAllBiddersInternal(sm);

		ItemAuctionBid? bid = auction.getHighestBid();
		if (bid != null)
		{
			Item item = auction.createNewItemInstance();
			Player? player = bid.getPlayer();
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

	protected void setStateTask(ScheduledFuture future)
	{
		ScheduledFuture? stateTask = _stateTask;
		if (stateTask != null)
		{
			stateTask.cancel(false);
		}

		_stateTask = future;
	}

	private ItemAuction createAuction(DateTime after)
	{
		AuctionItem auctionItem = _items.GetRandomElement();
		DateTime startingTime = _dateGenerator.nextDate(after);
		DateTime endingTime = startingTime + TimeSpan.FromMinutes(auctionItem.getAuctionLength());
		ItemAuction auction = new ItemAuction(_auctionIds.getAndIncrement(), _instanceId, startingTime, endingTime, auctionItem);
		auction.storeMe();
		return auction;
	}

	private ItemAuction? loadAuction(int auctionId)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			Db.ItemAuction? record = ctx.ItemAuctions.SingleOrDefault(r => r.AuctionId == auctionId);
			if (record is null)
			{
				LOGGER.Warn(GetType().Name + ": Auction data not found for auction: " + auctionId);
				return null;
			}

			int auctionItemId = record.AuctionItemId;
			DateTime startingTime = record.StartingTime;
			DateTime endingTime = record.EndingTime;
			byte auctionStateId = record.AuctionStateId;

			if (startingTime >= endingTime)
			{
				LOGGER.Warn(GetType().Name + ": Invalid starting/ending paramaters for auction: " + auctionId);
				return null;
			}

			AuctionItem? auctionItem = getAuctionItem(auctionItemId);
			if (auctionItem == null)
			{
				LOGGER.Warn(
					GetType().Name + ": AuctionItem: " + auctionItemId + ", not found for auction: " + auctionId);
				return null;
			}

			ItemAuctionState auctionState = (ItemAuctionState)auctionStateId;
			if (!Enum.IsDefined(auctionState))
			{
				LOGGER.Warn(GetType().Name + ": Invalid auctionStateId: " + auctionStateId + ", for auction: " +
				            auctionId);
				return null;
			}

			if (auctionState == ItemAuctionState.FINISHED && startingTime <
                DateTime.UtcNow -
                TimeSpan.FromDays(
                    Config.ALT_ITEM_AUCTION_EXPIRED_AFTER))
			{
				LOGGER.Info(GetType().Name + ": Clearing expired auction: " + auctionId);
				ctx.ItemAuctions.Where(r => r.AuctionId == auctionId).ExecuteDelete();
				ctx.ItemAuctionBids.Where(r => r.AuctionId == auctionId).ExecuteDelete();
				return null;
			}

			List<ItemAuctionBid> auctionBids = new();
			const string SELECT_PLAYERS_ID_BY_AUCTION_ID =
				"SELECT playerObjId, playerBid FROM item_auction_bid WHERE auctionId = ?";
			var query = ctx.ItemAuctionBids.Where(r => r.AuctionId == auctionId)
				.Select(r => new { r.CharacterId, r.Bid });

			foreach (var record1 in query)
			{
				int playerObjId = record1.CharacterId;
				long playerBid = record1.Bid;
				ItemAuctionBid bid = new ItemAuctionBid(playerObjId, playerBid);
				auctionBids.Add(bid);
			}

			return new ItemAuction(auctionId, _instanceId, startingTime, endingTime, auctionItem, auctionBids,
				auctionState);
		}
		catch (Exception exception)
		{
			LOGGER.Error(exception);
			return null;
		}
	}
}