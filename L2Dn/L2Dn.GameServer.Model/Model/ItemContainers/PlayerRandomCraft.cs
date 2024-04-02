using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.RandomCraft;
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _player.getObjectId();
			CharacterRandomCraft? record = ctx.CharacterRandomCrafts.SingleOrDefault(r => r.CharacterId == characterId);
			if (record != null)
			{
				try
				{
					_fullCraftPoints = record.FullPoints;
					_craftPoints = record.Points;
					_isSayhaRoll = record.IsSayhaRoll;

					_rewardList.Add(new RandomCraftRewardItemHolder(record.Item1Id, record.Item1Count, record.Item1Locked, record.Item1LockLeft));
					_rewardList.Add(new RandomCraftRewardItemHolder(record.Item2Id, record.Item2Count, record.Item2Locked, record.Item2LockLeft));
					_rewardList.Add(new RandomCraftRewardItemHolder(record.Item3Id, record.Item3Count, record.Item3Locked, record.Item3LockLeft));
					_rewardList.Add(new RandomCraftRewardItemHolder(record.Item4Id, record.Item4Count, record.Item4Locked, record.Item4LockLeft));
					_rewardList.Add(new RandomCraftRewardItemHolder(record.Item5Id, record.Item5Count, record.Item5Locked, record.Item5LockLeft));
				}
				catch (Exception e)
				{
					LOGGER.Error("Could not restore random craft for " + _player);
				}
			}
			else
			{
				storeNew();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore random craft for " + _player + ": " + e);
		}
	}
	
	public void store()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _player.getObjectId();
			CharacterRandomCraft? record = ctx.CharacterRandomCrafts.SingleOrDefault(r => r.CharacterId == characterId);
			if (record is null)
			{
				record = new CharacterRandomCraft();
				record.CharacterId = characterId;
				ctx.CharacterRandomCrafts.Add(record);
			}

			record.FullPoints = _fullCraftPoints;
			record.Points = _craftPoints;
			record.IsSayhaRoll = _isSayhaRoll;

			if (_rewardList.Count > 0)
			{
				RandomCraftRewardItemHolder holder = _rewardList[0];
				record.Item1Id = holder.getItemId();
				record.Item1Count = holder.getItemCount();
				record.Item1Locked = holder.isLocked();
				record.Item1LockLeft = holder.getLockLeft();
			}
			else
			{
				record.Item1Id = 0;
				record.Item1Count = 0;
				record.Item1Locked = false;
				record.Item1LockLeft = 0;
			}

			if (_rewardList.Count > 1)
			{
				RandomCraftRewardItemHolder holder = _rewardList[1];
				record.Item2Id = holder.getItemId();
				record.Item2Count = holder.getItemCount();
				record.Item2Locked = holder.isLocked();
				record.Item2LockLeft = holder.getLockLeft();
			}
			else
			{
				record.Item2Id = 0;
				record.Item2Count = 0;
				record.Item2Locked = false;
				record.Item2LockLeft = 0;
			}

			if (_rewardList.Count > 2)
			{
				RandomCraftRewardItemHolder holder = _rewardList[2];
				record.Item3Id = holder.getItemId();
				record.Item3Count = holder.getItemCount();
				record.Item3Locked = holder.isLocked();
				record.Item3LockLeft = holder.getLockLeft();
			}
			else
			{
				record.Item3Id = 0;
				record.Item3Count = 0;
				record.Item3Locked = false;
				record.Item3LockLeft = 0;
			}

			if (_rewardList.Count > 3)
			{
				RandomCraftRewardItemHolder holder = _rewardList[3];
				record.Item4Id = holder.getItemId();
				record.Item4Count = holder.getItemCount();
				record.Item4Locked = holder.isLocked();
				record.Item4LockLeft = holder.getLockLeft();
			}
			else
			{
				record.Item4Id = 0;
				record.Item4Count = 0;
				record.Item4Locked = false;
				record.Item4LockLeft = 0;
			}

			if (_rewardList.Count > 4)
			{
				RandomCraftRewardItemHolder holder = _rewardList[4];
				record.Item5Id = holder.getItemId();
				record.Item5Count = holder.getItemCount();
				record.Item5Locked = holder.isLocked();
				record.Item5LockLeft = holder.getLockLeft();
			}
			else
			{
				record.Item5Id = 0;
				record.Item5Count = 0;
				record.Item5Locked = false;
				record.Item5LockLeft = 0;
			}

			ctx.SaveChanges();
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _player.getObjectId();
			CharacterRandomCraft? record = ctx.CharacterRandomCrafts.SingleOrDefault(r => r.CharacterId == characterId);
			if (record is null)
			{
				record = new CharacterRandomCraft();
				record.CharacterId = characterId;
				ctx.CharacterRandomCrafts.Add(record);
			}

			record.FullPoints = _fullCraftPoints;
			record.Points = _craftPoints;
			record.IsSayhaRoll = _isSayhaRoll;

			ctx.SaveChanges();
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
			_player.sendPacket(new ExCraftInfoPacket(_player));
			_player.sendPacket(new ExCraftRandomRefreshPacket());
			_fullCraftPoints--;
			if (_isSayhaRoll)
			{
				_player.addItem("RandomCraft Roll", 91641, 2, _player, true);
				_isSayhaRoll = false;
			}
			_player.sendPacket(new ExCraftInfoPacket(_player));
			
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
			_player.sendPacket(new ExCraftRandomInfoPacket(_player));
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
				Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(_player, item, ExItemAnnouncePacket.RANDOM_CRAFT));
			}
			
			_player.sendPacket(new ExCraftRandomMakePacket(itemId, itemCount));
			_player.sendPacket(new ExCraftRandomInfoPacket(_player));
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
			_player.sendPacket(new ExCraftInfoPacket(_player));
		}
	}
	
	public void removeFullCraftPoints(int value)
	{
		_fullCraftPoints -= value;
		_player.sendPacket(new ExCraftInfoPacket(_player));
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
		
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.CRAFT_POINTS_S1);
		sm.Params.addLong(value);
		_player.sendPacket(sm);
		_player.sendPacket(new ExCraftInfoPacket(_player));
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