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
			using GameServerDbContext ctx = new();
			int characterId = _player.getObjectId();
			CharacterRandomCraft? record = ctx.CharacterRandomCrafts.SingleOrDefault(r => r.CharacterId == characterId);
			if (record != null)
			{
				try
				{
					_fullCraftPoints = record.FullPoints;
					_craftPoints = record.Points;
					_isSayhaRoll = record.IsSayhaRoll;

					CharacterRandomCraftItem item = record.Item1;
					_rewardList.Add(new RandomCraftRewardItemHolder(item.Id, item.Count, item.Locked, item.LockLeft));

					item = record.Item2;
					_rewardList.Add(new RandomCraftRewardItemHolder(item.Id, item.Count, item.Locked, item.LockLeft));

					item = record.Item3;
					_rewardList.Add(new RandomCraftRewardItemHolder(item.Id, item.Count, item.Locked, item.LockLeft));

					item = record.Item4;
					_rewardList.Add(new RandomCraftRewardItemHolder(item.Id, item.Count, item.Locked, item.LockLeft));

					item = record.Item5;
					_rewardList.Add(new RandomCraftRewardItemHolder(item.Id, item.Count, item.Locked, item.LockLeft));
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
			using GameServerDbContext ctx = new();
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

			static CharacterRandomCraftItem GetRewardItem(RandomCraftRewardItemHolder? holder) => new()
			{
				Id = holder?.getItemId() ?? 0,
				Count = holder?.getItemCount() ?? 0,
				Locked = holder?.isLocked() ?? false,
				LockLeft = holder?.getLockLeft() ?? 0
			};

			record.Item1 = GetRewardItem(_rewardList.Count > 0 ? _rewardList[0] : null);
			record.Item2 = GetRewardItem(_rewardList.Count > 1 ? _rewardList[1] : null);
			record.Item3 = GetRewardItem(_rewardList.Count > 2 ? _rewardList[2] : null);
			record.Item4 = GetRewardItem(_rewardList.Count > 3 ? _rewardList[3] : null);
			record.Item5 = GetRewardItem(_rewardList.Count > 4 ? _rewardList[4] : null);

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
			using GameServerDbContext ctx = new();
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
			record.Item1 = default;
			record.Item2 = default;
			record.Item3 = default;
			record.Item4 = default;
			record.Item5 = default;

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