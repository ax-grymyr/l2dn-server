using System.Globalization;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

public class AchievementBox
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AchievementBox));
	
	private const int ACHIEVEMENT_BOX_2H = 7200000;
	private const int ACHIEVEMENT_BOX_6H = 21600000;
	private const int ACHIEVEMENT_BOX_12H = 43200000;
	
	private readonly Player _owner;
	private int _boxOwned = 1;
	private int _monsterPoints = 0;
	private int _pvpPoints = 0;
	private int _pendingBoxSlotId = 0;
	private int _pvpEndDate;
	private long _boxTimeForOpen;
	private readonly List<AchievementBoxHolder> _achievementBox = new();
	private ScheduledFuture<?> _boxOpenTask;
	
	public AchievementBox(Player owner)
	{
		_owner = owner;
	}
	
	public int pvpEndDate()
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT * FROM achievement_box WHERE charId=?");
			ps.setInt(1, _owner.getObjectId());
			{
				ResultSet rs = ps.executeQuery();
				if (rs.next())
				{
					try
					{
						_boxOwned = rs.getInt("box_owned");
						_monsterPoints = rs.getInt("monster_point");
						_pvpPoints = rs.getInt("pvp_point");
						_pendingBoxSlotId = rs.getInt("pending_box");
						_boxTimeForOpen = rs.getLong("open_time");
						for (int i = 1; i <= 4; i++)
						{
							int state = rs.getInt("box_state_slot_" + i);
							int type = rs.getInt("boxtype_slot_" + i);
							if ((i == 1) && (state == 0))
							{
								state = 1;
							}
							AchievementBoxHolder holder = new AchievementBoxHolder(i, state, type);
							_achievementBox.add(i - 1, holder);
						}
					}
					catch (Exception e)
					{
						LOGGER.Error("Could not restore Achievement box for " + _owner);
					}
				}
				else
				{
					storeNew();
					_achievementBox.add(0, new AchievementBoxHolder(1, 1, 0));
				}
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps =
				con.prepareStatement("INSERT INTO achievement_box VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)");
			ps.setInt(1, _owner.getObjectId());
			ps.setInt(2, _boxOwned);
			ps.setInt(3, _monsterPoints);
			ps.setInt(4, _pvpPoints);
			ps.setInt(5, _pendingBoxSlotId);
			ps.setLong(6, _boxTimeForOpen);
			for (int i = 0; i < 4; i++)
			{
				ps.setInt(7 + (i * 2), 0);
				ps.setInt(8 + (i * 2), 0);
			}
			ps.executeUpdate();
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(
				"UPDATE achievement_box SET box_owned=?,monster_point=?,pvp_point=?,pending_box=?,open_time=?,box_state_slot_1=?,boxtype_slot_1=?,box_state_slot_2=?,boxtype_slot_2=?,box_state_slot_3=?,boxtype_slot_3=?,box_state_slot_4=?,boxtype_slot_4=? WHERE charId=?");
			ps.setInt(1, getBoxOwned());
			ps.setInt(2, getMonsterPoints());
			ps.setInt(3, getPvpPoints());
			ps.setInt(4, getPendingBoxSlotId());
			ps.setLong(5, getBoxOpenTime());
			for (int i = 0; i < 4; i++)
			{
				if (_achievementBox.size() >= (i + 1))
				{
					AchievementBoxHolder holder = _achievementBox.get(i);
					ps.setInt(6 + (i * 2), holder == null ? 0 : holder.getState().ordinal());
					ps.setInt(7 + (i * 2), holder == null ? 0 : holder.getType().ordinal());
				}
				else
				{
					ps.setInt(6 + (i * 2), 0);
					ps.setInt(7 + (i * 2), 0);
				}
			}
			ps.setInt(14, _owner.getObjectId());
			ps.execute();
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
		AchievementBoxHolder free = null;
		int id = -1;
		for (int i = 1; i <= getBoxOwned(); i++)
		{
			AchievementBoxHolder holder = getAchievementBox().get(i - 1);
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
					getAchievementBox().add(id - 1, free);
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
		
		AchievementBoxHolder holder = getAchievementBox().get(slotId - 1);
		if ((holder == null) || (_boxTimeForOpen != 0))
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
				getAchievementBox().add(slotId - 1, holder);
				sendBoxUpdate();
				break;
			}
			case AchievementBoxType.BOX_6H:
			{
				setBoxTimeForOpen(ACHIEVEMENT_BOX_6H);
				holder.setState(AchievementBoxState.UNLOCK_IN_PROGRESS);
				getAchievementBox().RemoveAt(slotId - 1);
				getAchievementBox().add(slotId - 1, holder);
				sendBoxUpdate();
				break;
			}
			case AchievementBoxType.BOX_12H:
			{
				setBoxTimeForOpen(ACHIEVEMENT_BOX_12H);
				holder.setState(AchievementBoxState.UNLOCK_IN_PROGRESS);
				getAchievementBox().RemoveAt(slotId - 1);
				getAchievementBox().add(slotId - 1, holder);
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
		
		AchievementBoxHolder holder = getAchievementBox().get(slotId - 1);
		if ((holder != null) && _owner.destroyItemByItemId("Take Achievement Box", Inventory.LCOIN_ID, fee, _owner, true))
		{
			if (_pendingBoxSlotId == slotId)
			{
				cancelTask();
			}
			finishAndUnlockChest(slotId);
		}
	}
	
	public bool setBoxTimeForOpen(long time)
	{
		if ((_boxOpenTask != null) && !(_boxOpenTask.isDone() || _boxOpenTask.isCancelled()))
		{
			return false;
		}
		
		_boxTimeForOpen = System.currentTimeMillis() + time;
		return true;
	}
	
	public void tryFinishBox()
	{
		if ((_boxTimeForOpen == 0) || (_boxTimeForOpen >= System.currentTimeMillis()))
		{
			return;
		}
		
		if ((_owner == null) || !_owner.isOnline())
		{
			return;
		}
		
		AchievementBoxHolder holder = getAchievementBox().get(_pendingBoxSlotId - 1);
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
	
	public long getBoxOpenTime()
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
			_boxTimeForOpen = 0;
			_pendingBoxSlotId = 0;
		}
		
		getAchievementBox().get(id - 1).setState(AchievementBoxState.RECEIVE_REWARD);
		sendBoxUpdate();
	}
	
	public void sendBoxUpdate()
	{
		_owner.sendPacket(new ExSteadyAllBoxUpdate(_owner));
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
			AchievementBoxHolder holder = new AchievementBoxHolder(slotId, 1, 0);
			holder.setState(AchievementBoxState.AVAILABLE);
			holder.setType(AchievementBoxType.LOCKED);
			getAchievementBox().add(slotId - 1, holder);
			sendBoxUpdate();
		}
	}
	
	public void getReward(int slotId)
	{
		AchievementBoxHolder holder = getAchievementBox().get(slotId - 1);
		if (holder.getState() != AchievementBoxState.RECEIVE_REWARD)
		{
			return;
		}
		
		int rnd = Rnd.get(100);
		ItemHolder reward = null;
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
			_owner.sendPacket(new ExSteadyBoxReward(slotId, reward.getId(), reward.getCount()));
		}
	}
	
	public void refreshPvpEndDate()
	{
		long currentTime = System.currentTimeMillis();
		Calendar calendar = Calendar.getInstance();
		calendar.setTimeInMillis(currentTime);
		calendar.set(Calendar.DAY_OF_MONTH, 1);
		calendar.set(Calendar.HOUR_OF_DAY, 6);
		if (calendar.getTimeInMillis() < currentTime)
		{
			calendar.add(Calendar.MONTH, 1);
		}
		
		_pvpEndDate = (int) (calendar.getTimeInMillis() / 1000);
	}
}