using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;

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
	
	public int getStatus(Player player)
	{
		DailyMissionPlayerEntry entry = getPlayerEntry(player.getObjectId(), false);
		return entry != null ? entry.getStatus() : DailyMissionStatus.NOT_AVAILABLE;
	}
	
	public int getProgress(Player player)
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
	public void reset()
	{
		if (!_holder.dailyReset())
		{
			return;
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps =
				con.prepareStatement("DELETE FROM character_daily_rewards WHERE rewardId = ? AND status = ?");
			ps.setInt(1, _holder.getId());
			ps.setInt(2, DailyMissionStatus.COMPLETED);
			ps.execute();
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
			entry.setLastCompleted(System.currentTimeMillis());
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
						player.sendPacket(new SystemMessage(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2).addItemName(MISSION_LEVEL_POINTS).addLong(expAmount));
					}
					break;
				}
				case MISSION_LEVEL_POINTS:
				{
					int levelPoints = (int) holder.getCount();
					MissionLevelPlayerDataHolder info = player.getMissionLevelProgress();
					info.calculateEXP(levelPoints);
					info.storeInfoInVariable(player);
					player.sendPacket(new SystemMessage(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2).addItemName(MISSION_LEVEL_POINTS).addLong(levelPoints));
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
			PreparedStatement ps = con.prepareStatement(
				"REPLACE INTO character_daily_rewards (charId, rewardId, status, progress, lastCompleted) VALUES (?, ?, ?, ?, ?)");
			ps.setInt(1, entry.getObjectId());
			ps.setInt(2, entry.getRewardId());
			ps.setInt(3, entry.getStatus());
			ps.setInt(4, entry.getProgress());
			ps.setLong(5, entry.getLastCompleted());
			ps.execute();
			
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
			PreparedStatement ps =
				con.prepareStatement("SELECT * FROM character_daily_rewards WHERE charId = ? AND rewardId = ?");
			ps.setInt(1, objectId);
			ps.setInt(2, _holder.getId());

			{
				ResultSet rs = ps.executeQuery();
				if (rs.next())
				{
					DailyMissionPlayerEntry entry = new DailyMissionPlayerEntry(rs.getInt("charId"), rs.getInt("rewardId"), rs.getInt("status"), rs.getInt("progress"), rs.getLong("lastCompleted"));
					_entries.put(objectId, entry);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while loading reward " + _holder.getId() + " for player: " + objectId + " in database: " + e);
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
