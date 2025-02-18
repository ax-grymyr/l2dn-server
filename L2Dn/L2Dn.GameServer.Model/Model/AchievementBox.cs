using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Network.OutgoingPackets.SteadyBoxes;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

public class AchievementBox
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AchievementBox));

	private static readonly TimeSpan ACHIEVEMENT_BOX_2H = TimeSpan.FromHours(2);
	private static readonly TimeSpan ACHIEVEMENT_BOX_6H = TimeSpan.FromHours(6);
	private static readonly TimeSpan ACHIEVEMENT_BOX_12H = TimeSpan.FromHours(12);

	private readonly Player _owner;
	private int _boxOwned = 1;
	private int _monsterPoints = 0;
	private int _pvpPoints = 0;
	private int _pendingBoxSlotId = 0;
	private DateTime _pvpEndDate;
	private DateTime? _boxTimeForOpen;
	private readonly List<AchievementBoxHolder> _achievementBox = new();
	private ScheduledFuture? _boxOpenTask;

	public AchievementBox(Player owner)
	{
		_owner = owner;
	}

	public DateTime pvpEndDate()
	{
		return _pvpEndDate;
	}

	public void addPoints(int value)
	{
		int newPoints = Math.Min(Config.ACHIEVEMENT_BOX_POINTS_FOR_REWARD, _monsterPoints + value);
		if (newPoints >= Config.ACHIEVEMENT_BOX_POINTS_FOR_REWARD)
		{
			if (addNewBox())
			{
				_monsterPoints = 0;
			}
			else
			{
				_monsterPoints = Config.ACHIEVEMENT_BOX_POINTS_FOR_REWARD;
			}
			return;
		}
		_monsterPoints += value;
	}

	public void addPvpPoints(int value)
	{
		int newPoints = Math.Min(Config.ACHIEVEMENT_BOX_PVP_POINTS_FOR_REWARD, _pvpPoints);
		while (newPoints >= Config.ACHIEVEMENT_BOX_PVP_POINTS_FOR_REWARD)
		{
			if (addNewBox())
			{
				_pvpPoints = 0;
			}
			else
			{
				_pvpPoints = Config.ACHIEVEMENT_BOX_PVP_POINTS_FOR_REWARD;
			}
			return;
		}
		_pvpPoints += value;
	}

	public void restore()
	{
		tryFinishBox();
		refreshPvpEndDate();
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _owner.ObjectId;
			var record = ctx.AchievementBoxes.SingleOrDefault(r => r.CharacterId == characterId);
			if (record != null)
			{
				try
				{
					_boxOwned = record.BoxOwned;
					_monsterPoints = record.MonsterPoint;
					_pvpPoints = record.PvpPoint;
					_pendingBoxSlotId = record.PendingBox;
					_boxTimeForOpen = record.OpenTime;

					AchievementBoxState state = (AchievementBoxState)record.BoxStateSlot1;
					AchievementBoxType type = (AchievementBoxType)record.BoxTypeSlot1;
					if (state == AchievementBoxState.LOCKED)
						state = AchievementBoxState.AVAILABLE;

					AchievementBoxHolder holder = new AchievementBoxHolder(0, state, type);
					_achievementBox.Insert(0, holder);

					state = (AchievementBoxState)record.BoxStateSlot2;
					type = (AchievementBoxType)record.BoxTypeSlot2;
					holder = new AchievementBoxHolder(1, state, type);
					_achievementBox.Insert(1, holder);

					state = (AchievementBoxState)record.BoxStateSlot3;
					type = (AchievementBoxType)record.BoxTypeSlot3;
					holder = new AchievementBoxHolder(2, state, type);
					_achievementBox.Insert(2, holder);

					state = (AchievementBoxState)record.BoxStateSlot4;
					type = (AchievementBoxType)record.BoxTypeSlot4;
					holder = new AchievementBoxHolder(3, state, type);
					_achievementBox.Insert(3, holder);
				}
				catch (Exception e)
                {
                    LOGGER.Error("Could not restore Achievement box for " + _owner + ": " + e);
                }
			}
			else
			{
				storeNew();
				_achievementBox.Insert(0, new AchievementBoxHolder(1, AchievementBoxState.AVAILABLE, AchievementBoxType.LOCKED));
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore achievement box for " + _owner + ": " + e);
		}
	}

	public void storeNew()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.AchievementBoxes.Add(new DbAchievementBox()
			{
				CharacterId = _owner.ObjectId,
				BoxOwned = _boxOwned,
				MonsterPoint = _monsterPoints,
				PvpPoint = _pvpPoints,
				PendingBox = _pendingBoxSlotId,
				OpenTime = _boxTimeForOpen
			});

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store new Archivement Box for: " + _owner + ": " + e);
		}
	}

	public void store()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _owner.ObjectId;
			var record = ctx.AchievementBoxes.SingleOrDefault(r => r.CharacterId == characterId);
			if (record is null)
			{
				record = new DbAchievementBox();
				record.CharacterId = characterId;
				ctx.AchievementBoxes.Add(record);
			}

			record.BoxOwned = getBoxOwned();
			record.MonsterPoint = getMonsterPoints();
			record.PvpPoint = getPvpPoints();
			record.PendingBox = getPendingBoxSlotId();
			record.OpenTime = getBoxOpenTime();

			record.BoxStateSlot1 = _achievementBox.Count > 0 ? (int)_achievementBox[0].getState() : 0;
			record.BoxTypeSlot1 = _achievementBox.Count > 0 ? (int)_achievementBox[0].getType() : 0;

			record.BoxStateSlot2 = _achievementBox.Count > 1 ? (int)_achievementBox[1].getState() : 0;
			record.BoxTypeSlot2 = _achievementBox.Count > 1 ? (int)_achievementBox[1].getType() : 0;

			record.BoxStateSlot3 = _achievementBox.Count > 2 ? (int)_achievementBox[2].getState() : 0;
			record.BoxTypeSlot3 = _achievementBox.Count > 2 ? (int)_achievementBox[2].getType() : 0;

			record.BoxStateSlot4 = _achievementBox.Count > 3 ? (int)_achievementBox[3].getState() : 0;
			record.BoxTypeSlot4 = _achievementBox.Count > 3 ? (int)_achievementBox[3].getType() : 0;

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store Achievement Box for: " + _owner + ": " + e);
		}
	}

	public List<AchievementBoxHolder> getAchievementBox()
	{
		return _achievementBox;
	}

	public bool addNewBox()
	{
		AchievementBoxHolder? free = null;
		int id = -1;
		for (int i = 1; i <= getBoxOwned(); i++)
		{
			AchievementBoxHolder holder = getAchievementBox()[i - 1];
			if (holder.getState() == AchievementBoxState.AVAILABLE)
			{
				free = holder;
				id = i;
				break;
			}
		}
		if (free != null)
		{
			int rnd = Rnd.get(0, 100);
			free.setType(rnd < 12 ? AchievementBoxType.BOX_12H : rnd < 40 ? AchievementBoxType.BOX_6H : AchievementBoxType.BOX_2H);
			switch (free.getType())
			{
				case AchievementBoxType.BOX_2H:
				case AchievementBoxType.BOX_6H:
				case AchievementBoxType.BOX_12H:
				{
					free.setState(AchievementBoxState.OPEN);
					getAchievementBox().RemoveAt(id - 1);
					getAchievementBox().Insert(id - 1, free);
					sendBoxUpdate();
					break;
				}
			}
			return true;
		}
		return false;
	}

	public void openBox(int slotId)
	{
		if (slotId > getBoxOwned())
		{
			return;
		}

		AchievementBoxHolder holder = getAchievementBox()[slotId - 1];
		if ((holder == null) || (_boxTimeForOpen != null))
		{
			return;
		}

		_pendingBoxSlotId = slotId;

		switch (holder.getType())
		{
			case AchievementBoxType.BOX_2H:
			{
				setBoxTimeForOpen(ACHIEVEMENT_BOX_2H);
				holder.setState(AchievementBoxState.UNLOCK_IN_PROGRESS);
				getAchievementBox().RemoveAt(slotId - 1);
				getAchievementBox().Insert(slotId - 1, holder);
				sendBoxUpdate();
				break;
			}
			case AchievementBoxType.BOX_6H:
			{
				setBoxTimeForOpen(ACHIEVEMENT_BOX_6H);
				holder.setState(AchievementBoxState.UNLOCK_IN_PROGRESS);
				getAchievementBox().RemoveAt(slotId - 1);
				getAchievementBox().Insert(slotId - 1, holder);
				sendBoxUpdate();
				break;
			}
			case AchievementBoxType.BOX_12H:
			{
				setBoxTimeForOpen(ACHIEVEMENT_BOX_12H);
				holder.setState(AchievementBoxState.UNLOCK_IN_PROGRESS);
				getAchievementBox().RemoveAt(slotId - 1);
				getAchievementBox().Insert(slotId - 1, holder);
				sendBoxUpdate();
				break;
			}
		}
	}

	public void skipBoxOpenTime(int slotId, long fee)
	{
		if (slotId > getBoxOwned())
		{
			return;
		}

		AchievementBoxHolder holder = getAchievementBox()[slotId - 1];
		if ((holder != null) && _owner.destroyItemByItemId("Take Achievement Box", Inventory.LCOIN_ID, fee, _owner, true))
		{
			if (_pendingBoxSlotId == slotId)
			{
				cancelTask();
			}
			finishAndUnlockChest(slotId);
		}
	}

	public bool setBoxTimeForOpen(TimeSpan time)
	{
		if ((_boxOpenTask != null) && !(_boxOpenTask.isDone() || _boxOpenTask.isCancelled()))
		{
			return false;
		}

		_boxTimeForOpen = DateTime.UtcNow + time;
		return true;
	}

	public void tryFinishBox()
	{
		if ((_boxTimeForOpen == null) || (_boxTimeForOpen >= DateTime.UtcNow))
		{
			return;
		}

		if ((_owner == null) || !_owner.isOnline())
		{
			return;
		}

		AchievementBoxHolder holder = getAchievementBox()[_pendingBoxSlotId - 1];
		if (holder != null)
		{
			finishAndUnlockChest(_pendingBoxSlotId);
		}
	}

	public int getBoxOwned()
	{
		return _boxOwned;
	}

	public int getMonsterPoints()
	{
		return _monsterPoints;
	}

	public int getPvpPoints()
	{
		return _pvpPoints;
	}

	public int getPendingBoxSlotId()
	{
		return _pendingBoxSlotId;
	}

	public DateTime? getBoxOpenTime()
	{
		return _boxTimeForOpen;
	}

	public void finishAndUnlockChest(int id)
	{
		if (id > getBoxOwned())
		{
			return;
		}

		if (_pendingBoxSlotId == id)
		{
			_boxTimeForOpen = null;
			_pendingBoxSlotId = 0;
		}

		getAchievementBox()[id - 1].setState(AchievementBoxState.RECEIVE_REWARD);
		sendBoxUpdate();
	}

	public void sendBoxUpdate()
	{
		_owner.sendPacket(new ExSteadyAllBoxUpdatePacket(_owner));
	}

	public void cancelTask()
	{
		if (_boxOpenTask == null)
		{
			return;
		}

		_boxOpenTask.cancel(false);
		_boxOpenTask = null;
	}

	public void unlockSlot(int slotId)
	{
		if (((slotId - 1) != getBoxOwned()) || (slotId > 4))
		{
			return;
		}

		bool paidSlot = false;
		switch (slotId)
		{
			case 2:
			{
				if (_owner.reduceAdena("Adena " + slotId, 100000000, _owner, true))
				{
					paidSlot = true;
				}
				break;
			}
			case 3:
			{
				if (_owner.destroyItemByItemId("L coin " + slotId, Inventory.LCOIN_ID, 2000, _owner, true))
				{
					paidSlot = true;
				}
				break;
			}
			case 4:
			{
				if (_owner.destroyItemByItemId("L coin " + slotId, Inventory.LCOIN_ID, 8000, _owner, true))
				{
					paidSlot = true;
				}
				break;
			}
		}

		if (paidSlot)
		{
			_boxOwned = slotId;
			AchievementBoxHolder holder = new AchievementBoxHolder(slotId, AchievementBoxState.AVAILABLE, AchievementBoxType.LOCKED);
			holder.setState(AchievementBoxState.AVAILABLE);
			holder.setType(AchievementBoxType.LOCKED);
			getAchievementBox().Insert(slotId - 1, holder);
			sendBoxUpdate();
		}
	}

	public void getReward(int slotId)
	{
		AchievementBoxHolder holder = getAchievementBox()[slotId - 1];
		if (holder.getState() != AchievementBoxState.RECEIVE_REWARD)
		{
			return;
		}

		int rnd = Rnd.get(100);
		ItemHolder? reward = null;
		switch (holder.getType())
		{
			case AchievementBoxType.BOX_2H:
			{
				if (rnd < 3)
				{
					reward = new ItemHolder(Rnd.get(72084, 72102), 1);
				}
				else if (rnd < 30)
				{
					reward = new ItemHolder(93274, 5); // Sayha Cookie
				}
				else if (rnd < 70)
				{
					reward = new ItemHolder(90907, 250); // Soulshot Ticket
				}
				else
				{
					reward = new ItemHolder(3031, 50); // Spirit Ore
				}
				break;
			}
			case AchievementBoxType.BOX_6H:
			{
				if (rnd < 10)
				{
					reward = new ItemHolder(Rnd.get(72084, 72102), 1);
				}
				else if (rnd < 30)
				{
					reward = new ItemHolder(93274, 10); // Sayha Cookie
				}
				else if (rnd < 70)
				{
					reward = new ItemHolder(90907, 500); // Soulshot Ticket
				}
				else
				{
					reward = new ItemHolder(3031, 100); // Spirit Ore
				}
				break;
			}
			case AchievementBoxType.BOX_12H:
			{
				if (rnd < 20)
				{
					reward = new ItemHolder(Rnd.get(72084, 72102), 1);
				}
				else if (rnd < 30)
				{
					reward = new ItemHolder(93274, 20); // Sayha Cookie
				}
				else if (rnd < 70)
				{
					reward = new ItemHolder(90907, 1000); // Soulshot Ticket
				}
				else
				{
					reward = new ItemHolder(3031, 200); // Spirit Ore
				}
				break;
			}
		}

		holder.setState(AchievementBoxState.AVAILABLE);
		holder.setType(AchievementBoxType.LOCKED);
		sendBoxUpdate();
		if (reward != null)
		{
			_owner.addItem("Chest unlock", reward, _owner, true);
			_owner.sendPacket(new ExSteadyBoxRewardPacket(slotId, reward.getId(), reward.getCount()));
		}
	}

	public void refreshPvpEndDate()
	{
		DateTime currentTime = DateTime.Now;
		DateTime calendar = new DateTime(currentTime.Year, currentTime.Month, 1, 6, 0, 0, 0);
		if (calendar < DateTime.Now)
		{
			calendar = calendar.AddMonths(1);
		}

		_pvpEndDate = calendar;
	}
}