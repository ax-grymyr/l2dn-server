using System.Runtime.CompilerServices;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Handlers;

/**
 * @author Sdw
 */
public abstract class AbstractDailyMissionHandler: ListenersContainer
{
	public const int MISSION_LEVEL_POINTS = 97224;
	private const int CLAN_EXP = 94481;
	
	protected readonly Logger LOGGER = LogManager.GetLogger(nameof(AbstractDailyMissionHandler));
	
	private readonly Map<int, DailyMissionPlayerEntry> _entries = new();
	private readonly DailyMissionDataHolder _holder;
	
	protected AbstractDailyMissionHandler(DailyMissionDataHolder holder)
	{
		_holder = holder;
		init();
	}
	
	public DailyMissionDataHolder getHolder()
	{
		return _holder;
	}
	
	public abstract bool isAvailable(Player player);
	
	public abstract void init();
	
	public DailyMissionStatus getStatus(Player player)
	{
		DailyMissionPlayerEntry entry = getPlayerEntry(player.getObjectId(), false);
		return entry != null ? entry.getStatus() : DailyMissionStatus.NOT_AVAILABLE;
	}
	
	public virtual int getProgress(Player player)
	{
		DailyMissionPlayerEntry entry = getPlayerEntry(player.getObjectId(), false);
		return entry != null ? entry.getProgress() : 0;
	}
	
	public bool isRecentlyCompleted(Player player)
	{
		DailyMissionPlayerEntry entry = getPlayerEntry(player.getObjectId(), false);
		return (entry != null) && entry.isRecentlyCompleted();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public virtual void reset()
	{
		if (!_holder.dailyReset())
		{
			return;
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			int rewardId = _holder.getId();
			ctx.CharacterDailyRewards.Where(r => r.RewardId == rewardId && r.Status == DailyMissionStatus.COMPLETED)
				.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while clearing data for: " + e);
		}
		finally
		{
			_entries.clear();
		}
	}
	
	public bool requestReward(Player player)
	{
		if (isAvailable(player))
		{
			giveRewards(player);
			
			DailyMissionPlayerEntry entry = getPlayerEntry(player.getObjectId(), true);
			entry.setStatus(DailyMissionStatus.COMPLETED);
			entry.setLastCompleted(DateTime.UtcNow);
			entry.setRecentlyCompleted(true);
			storePlayerEntry(entry);
			
			return true;
		}
		return false;
	}
	
	protected void giveRewards(Player player)
	{
		foreach (ItemHolder holder in _holder.getRewards())
		{
			switch (holder.getId())
			{
				case CLAN_EXP:
				{
					Clan clan = player.getClan();
					if (clan != null)
					{
						int expAmount = (int) holder.getCount();
						clan.addExp(player.getObjectId(), expAmount);
						SystemMessagePacket packet = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
						packet.Params.addItemName(MISSION_LEVEL_POINTS).addLong(expAmount);
						player.sendPacket(packet);
					}
					break;
				}
				case MISSION_LEVEL_POINTS:
				{
					int levelPoints = (int) holder.getCount();
					MissionLevelPlayerDataHolder info = player.getMissionLevelProgress();
					info.calculateEXP(levelPoints);
					info.storeInfoInVariable(player);
					SystemMessagePacket packet = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
					packet.Params.addItemName(MISSION_LEVEL_POINTS).addLong(levelPoints);
					player.sendPacket(packet);
					break;
				}
				default:
				{
					player.addItem("One Day Reward", holder, player, true);
					break;
				}
			}
		}
	}
	
	protected void storePlayerEntry(DailyMissionPlayerEntry entry)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			
			// TODO: delete existing record?
			
			ctx.CharacterDailyRewards.Add(new CharacterDailyReward
			{
				CharacterId = entry.getObjectId(),
				RewardId = entry.getRewardId(),
				Status = entry.getStatus(),
				Progress = entry.getProgress(),
				LastCompleted = entry.getLastCompleted()
			});

			ctx.SaveChanges();
			
			// Cache if not exists
			_entries.computeIfAbsent(entry.getObjectId(), id => entry);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while saving reward " + entry.getRewardId() + " for player: " + entry.getObjectId() + " in database: " + e);
		}
	}
	
	protected DailyMissionPlayerEntry getPlayerEntry(int objectId, bool createIfNone)
	{
		DailyMissionPlayerEntry existingEntry = _entries.get(objectId);
		if (existingEntry != null)
		{
			return existingEntry;
		}

		try
		{
			using GameServerDbContext ctx = new();
			int rewardId = _holder.getId();
			CharacterDailyReward? reward =
				ctx.CharacterDailyRewards.SingleOrDefault(r => r.CharacterId == objectId && r.RewardId == rewardId);

			if (reward is not null)
			{
				DailyMissionPlayerEntry entry = new DailyMissionPlayerEntry(reward.CharacterId, reward.RewardId,
					reward.Status, reward.Progress, reward.LastCompleted);

				_entries.put(objectId, entry);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while loading reward " + _holder.getId() + " for player: " + objectId +
			            " in database: " + e);
		}

		if (createIfNone)
		{
			DailyMissionPlayerEntry entry = new DailyMissionPlayerEntry(objectId, _holder.getId());
			_entries.put(objectId, entry);
			return entry;
		}
		return null;
	}
}
