using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.ItemContainers;

public class PlayerRandomCraft
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PlayerRandomCraft));
	
	public const int MAX_FULL_CRAFT_POINTS = 99;
	public const int MAX_CRAFT_POINTS = 5000000;
	
	private readonly Player _player;
	private readonly List<RandomCraftRewardItemHolder> _rewardList = new(5);
	
	private int _fullCraftPoints = 0;
	private int _craftPoints = 0;
	private bool _isSayhaRoll = false;
	
	public PlayerRandomCraft(Player player)
	{
		_player = player;
	}
	
	public void restore()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement("SELECT * FROM character_random_craft WHERE charId=?");
			ps.setInt(1, _player.getObjectId());
			try
			{
				ResultSet rs = ps.executeQuery();
				if (rs.next())
				{
					try
					{
						_fullCraftPoints = rs.getInt("random_craft_full_points");
						_craftPoints = rs.getInt("random_craft_points");
						_isSayhaRoll = rs.getBoolean("sayha_roll");
						for (int i = 1; i <= 5; i++)
						{
							int itemId = rs.getInt("item_" + i + "_id");
							long itemCount = rs.getLong("item_" + i + "_count");
							bool itemLocked = rs.getBoolean("item_" + i + "_locked");
							int itemLockLeft = rs.getInt("item_" + i + "_lock_left");
							RandomCraftRewardItemHolder holder = new RandomCraftRewardItemHolder(itemId, itemCount, itemLocked, itemLockLeft);
							_rewardList.add(i - 1, holder);
						}
					}
					catch (Exception e)
					{
						LOGGER.Warn("Could not restore random craft for " + _player);
					}
				}
				else
				{
					storeNew();
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore random craft for " + _player + ": " + e);
		}
	}
	
	public void store()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(
				"UPDATE character_random_craft SET random_craft_full_points=?,random_craft_points=?,sayha_roll=?,item_1_id=?,item_1_count=?,item_1_locked=?,item_1_lock_left=?,item_2_id=?,item_2_count=?,item_2_locked=?,item_2_lock_left=?,item_3_id=?,item_3_count=?,item_3_locked=?,item_3_lock_left=?,item_4_id=?,item_4_count=?,item_4_locked=?,item_4_lock_left=?,item_5_id=?,item_5_count=?,item_5_locked=?,item_5_lock_left=? WHERE charId=?");
			ps.setInt(1, _fullCraftPoints);
			ps.setInt(2, _craftPoints);
			ps.setBoolean(3, _isSayhaRoll);
			for (int i = 0; i < 5; i++)
			{
				if (_rewardList.Count >= (i + 1))
				{
					RandomCraftRewardItemHolder holder = _rewardList[i];
					ps.setInt(4 + (i * 4), holder == null ? 0 : holder.getItemId());
					ps.setLong(5 + (i * 4), holder == null ? 0 : holder.getItemCount());
					ps.setBoolean(6 + (i * 4), (holder != null) && holder.isLocked());
					ps.setInt(7 + (i * 4), holder == null ? 20 : holder.getLockLeft());
				}
				else
				{
					ps.setInt(4 + (i * 4), 0);
					ps.setLong(5 + (i * 4), 0);
					ps.setBoolean(6 + (i * 4), false);
					ps.setInt(7 + (i * 4), 20);
				}
			}
			ps.setInt(24, _player.getObjectId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store RandomCraft for: " + _player + ": " + e);
		}
	}
	
	public void storeNew()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps =
				con.prepareStatement(
					"INSERT INTO character_random_craft VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)");
			ps.setInt(1, _player.getObjectId());
			ps.setInt(2, _fullCraftPoints);
			ps.setInt(3, _craftPoints);
			ps.setBoolean(4, _isSayhaRoll);
			for (int i = 0; i < 5; i++)
			{
				ps.setInt(5 + (i * 4), 0);
				ps.setLong(6 + (i * 4), 0);
				ps.setBoolean(7 + (i * 4), false);
				ps.setInt(8 + (i * 4), 0);
			}
			ps.executeUpdate();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store new RandomCraft for: " + _player + ": " + e);
		}
	}
	
	public void refresh()
	{
		if (_player.hasItemRequest() || _player.hasRequest<RandomCraftRequest>())
		{
			return;
		}
		_player.addRequest(new RandomCraftRequest(_player));
		
		if ((_fullCraftPoints > 0) && _player.reduceAdena("RandomCraft Refresh", Config.RANDOM_CRAFT_REFRESH_FEE, _player, true))
		{
			_player.sendPacket(new ExCraftInfo(_player));
			_player.sendPacket(new ExCraftRandomRefresh());
			_fullCraftPoints--;
			if (_isSayhaRoll)
			{
				_player.addItem("RandomCraft Roll", 91641, 2, _player, true);
				_isSayhaRoll = false;
			}
			_player.sendPacket(new ExCraftInfo(_player));
			
			for (int i = 0; i < 5; i++)
			{
				RandomCraftRewardItemHolder holder;
				if (i > (_rewardList.size() - 1))
				{
					holder = null;
				}
				else
				{
					holder = _rewardList.get(i);
				}
				
				if (holder == null)
				{
					_rewardList.add(i, getNewReward());
				}
				else if (!holder.isLocked())
				{
					_rewardList.set(i, getNewReward());
				}
				else
				{
					holder.decLock();
				}
			}
			_player.sendPacket(new ExCraftRandomInfo(_player));
		}
		
		_player.removeRequest<RandomCraftRequest>();
	}
	
	private RandomCraftRewardItemHolder getNewReward()
	{
		if (RandomCraftData.getInstance().isEmpty())
		{
			return null;
		}
		
		RandomCraftRewardItemHolder result = null;
		while (result == null)
		{
			result = RandomCraftData.getInstance().getNewReward();
			foreach (RandomCraftRewardItemHolder reward in _rewardList)
			{
				if (reward.getItemId() == result.getItemId())
				{
					result = null;
					break;
				}
			}
		}
		return result;
	}
	
	public void make()
	{
		if (_player.hasItemRequest() || _player.hasRequest<RandomCraftRequest>())
		{
			return;
		}
		_player.addRequest(new RandomCraftRequest(_player));
		
		if (_player.reduceAdena("RandomCraft Make", Config.RANDOM_CRAFT_CREATE_FEE, _player, true))
		{
			int madeId = Rnd.get(0, 4);
			RandomCraftRewardItemHolder holder = _rewardList.get(madeId);
			_rewardList.Clear();
			
			int itemId = holder.getItemId();
			long itemCount = holder.getItemCount();
			Item item = _player.addItem("RandomCraft Make", itemId, itemCount, _player, true);
			if (RandomCraftData.getInstance().isAnnounce(itemId))
			{
				Broadcast.toAllOnlinePlayers(new ExItemAnnounce(_player, item, ExItemAnnounce.RANDOM_CRAFT));
			}
			
			_player.sendPacket(new ExCraftRandomMake(itemId, itemCount));
			_player.sendPacket(new ExCraftRandomInfo(_player));
		}
		
		_player.removeRequest<RandomCraftRequest>();
	}
	
	public List<RandomCraftRewardItemHolder> getRewards()
	{
		return _rewardList;
	}
	
	public int getFullCraftPoints()
	{
		return _fullCraftPoints;
	}
	
	public void addFullCraftPoints(int value)
	{
		addFullCraftPoints(value, false);
	}
	
	public void addFullCraftPoints(int value, bool broadcast)
	{
		_fullCraftPoints = Math.Min(_fullCraftPoints + value, MAX_FULL_CRAFT_POINTS);
		if (_craftPoints >= MAX_CRAFT_POINTS)
		{
			_craftPoints = 0;
		}
		if (value > 0)
		{
			_isSayhaRoll = true;
		}
		if (broadcast)
		{
			_player.sendPacket(new ExCraftInfo(_player));
		}
	}
	
	public void removeFullCraftPoints(int value)
	{
		_fullCraftPoints -= value;
		_player.sendPacket(new ExCraftInfo(_player));
	}
	
	public void addCraftPoints(int value)
	{
		if ((_craftPoints - 1) < MAX_CRAFT_POINTS)
		{
			_craftPoints += value;
		}
		
		int fullPointsToAdd = _craftPoints / MAX_CRAFT_POINTS;
		int pointsToRemove = MAX_CRAFT_POINTS * fullPointsToAdd;
		
		_craftPoints -= pointsToRemove;
		addFullCraftPoints(fullPointsToAdd);
		if (_fullCraftPoints >= MAX_FULL_CRAFT_POINTS)
		{
			_craftPoints = MAX_CRAFT_POINTS;
		}
		
		SystemMessage sm = new SystemMessage(SystemMessageId.CRAFT_POINTS_S1);
		sm.addLong(value);
		_player.sendPacket(sm);
		_player.sendPacket(new ExCraftInfo(_player));
	}
	
	public int getCraftPoints()
	{
		return _craftPoints;
	}
	
	public void setIsSayhaRoll(bool value)
	{
		_isSayhaRoll = value;
	}
	
	public bool isSayhaRoll()
	{
		return _isSayhaRoll;
	}
	
	public int getLockedSlotCount()
	{
		int count = 0;
		foreach (RandomCraftRewardItemHolder holder in _rewardList)
		{
			if (holder.isLocked())
			{
				count++;
			}
		}
		return count;
	}
}