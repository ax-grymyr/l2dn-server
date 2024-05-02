using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Revenge;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius
 */
public class RevengeHistoryManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RevengeHistoryManager));
	
	private static readonly Map<int, List<RevengeHistoryHolder>> REVENGE_HISTORY = new();
	private static readonly SkillHolder HIDE_SKILL = new SkillHolder(922, 1);
	private static readonly TimeSpan REVENGE_DURATION = TimeSpan.FromHours(6); // Six hours.
	private static readonly int[] LOCATION_PRICE =
	{
		0,
		50000,
		100000,
		100000,
		200000
	};
	private static readonly int[] TELEPORT_PRICE =
	{
		10,
		50,
		100,
		100,
		200
	};
	
	protected RevengeHistoryManager()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (CharacterRevenge record in ctx.CharacterRevenges)
			{
				int charId = record.CharacterId;
				List<RevengeHistoryHolder> history = REVENGE_HISTORY.containsKey(charId) ? REVENGE_HISTORY.get(charId) : new();
				
				StatSet killer = new StatSet();
				killer.set("name", record.KillerName);
				killer.set("clan", record.KillerClan);
				killer.set("level", record.KillerLevel);
				killer.set("race", (int)((CharacterClass)record.KillerClass).GetRace());
				killer.set("class", record.KillerClass);
				
				StatSet victim = new StatSet();
				victim.set("name", record.VictimName);
				victim.set("clan", record.VictimClan);
				victim.set("level", record.VictimLevel);
				victim.set("race", (int)((CharacterClass)record.VictimClass).GetRace());
				victim.set("class", record.VictimClass);

				history.add(new RevengeHistoryHolder(killer, victim, (RevengeType)record.Type,
					record.Shared, record.ShowLocationRemaining,
					record.TeleportRemaining, record.SharedTeleportRemaining,
					record.KillTime, record.ShareTime));

				REVENGE_HISTORY.put(charId, history);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed loading revenge history! " + e);
		}
	}
	
	public void storeMe()
	{
		foreach (var entry in REVENGE_HISTORY)
		{
			List<RevengeHistoryHolder> history = entry.Value;
			if (history != null)
			{
				DateTime currentTime = DateTime.UtcNow;
				List<RevengeHistoryHolder> removals = new();
				foreach (RevengeHistoryHolder holder in history)
				{
					if (((holder.getKillTime() != DateTime.MinValue) && ((holder.getKillTime() + REVENGE_DURATION) < currentTime)) || //
						((holder.getShareTime() != DateTime.MinValue) && ((holder.getShareTime() + REVENGE_DURATION) < currentTime)))
					{
						removals.add(holder);
					}
				}
				foreach (RevengeHistoryHolder holder in removals)
				{
					history.Remove(holder);
				}
			}
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterRevenges.ExecuteDelete(); // TODO: delete everything and insert the data again is not a good idea
			
			foreach (var entry in REVENGE_HISTORY)
			{
				List<RevengeHistoryHolder> history = entry.Value;
				if ((history == null) || history.isEmpty())
				{
					continue;
				}
				
				foreach (RevengeHistoryHolder holder in history)
				{
					ctx.CharacterRevenges.Add(new CharacterRevenge()
					{
						CharacterId = entry.Key,
						Type = (int)holder.getType(),
						KillerName = holder.getKillerName(),
						KillerClan = holder.getKillerClanName(),
						KillerLevel = holder.getKillerLevel(),
						KillerClass = holder.getKillerClass(),
						VictimName = holder.getVictimName(),
						VictimClan = holder.getVictimClanName(),
						VictimLevel = holder.getVictimLevel(),
						VictimClass = holder.getVictimClass(),
						Shared = holder.wasShared(),
						ShowLocationRemaining = holder.getShowLocationRemaining(),
						TeleportRemaining = holder.getTeleportRemaining(),
						SharedTeleportRemaining = holder.getSharedTeleportRemaining(),
						KillTime = holder.getKillTime(),
						ShareTime = holder.getShareTime()
					});
				}
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + " Error while saving revenge history. " + e);
		}
	}
	
	public void addNewKill(Player victim, Player killer)
	{
		try
		{
			bool found = false;
			int victimObjectId = victim.getObjectId();
			DateTime currentTime = DateTime.UtcNow;
			List<RevengeHistoryHolder> removals = new();
			List<RevengeHistoryHolder> history = REVENGE_HISTORY.containsKey(victimObjectId) ? REVENGE_HISTORY.get(victimObjectId) : new();
			foreach (RevengeHistoryHolder holder in history)
			{
				if (((holder.getKillTime() != DateTime.MinValue) && ((holder.getKillTime() + REVENGE_DURATION) < currentTime)) || //
					((holder.getShareTime() != DateTime.MinValue) && ((holder.getShareTime() + REVENGE_DURATION) < currentTime)))
				{
					removals.add(holder);
				}
				else if (holder.getKillerName().equals(killer.getName()))
				{
					found = true;
				}
			}

			foreach (RevengeHistoryHolder holder in removals)
				history.Remove(holder);
			
			if (!found)
			{
				history.add(new RevengeHistoryHolder(killer, victim, RevengeType.REVENGE));
				REVENGE_HISTORY.put(victimObjectId, history);
				victim.sendPacket(new ExPvpBookShareRevengeNewRevengeInfoPacket(victim.getName(), killer.getName(), RevengeType.REVENGE));
				victim.sendPacket(new ExPvpBookShareRevengeListPacket(victim));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Failed adding revenge history! " + e);
		}
	}
	
	public void locateKiller(Player player, String killerName)
	{
		List<RevengeHistoryHolder> history = REVENGE_HISTORY.get(player.getObjectId());
		if (history == null)
		{
			return;
		}
		
		RevengeHistoryHolder revenge = null;
		foreach (RevengeHistoryHolder holder in history)
		{
			if (holder.getKillerName().equals(killerName))
			{
				revenge = holder;
				break;
			}
		}
		
		if (revenge == null)
		{
			return;
		}
		
		Player killer = World.getInstance().getPlayer(killerName);
		if ((killer == null) || !killer.isOnline())
		{
			player.sendPacket(SystemMessageId.THE_ENEMY_IS_OFFLINE_AND_CANNOT_BE_FOUND_RIGHT_NOW);
			return;
		}
		
		if (killer.isInsideZone(ZoneId.PEACE) || killer.isInInstance() || killer.isInTimedHuntingZone() || killer.isInsideZone(ZoneId.SIEGE) //
			|| player.isDead() || player.isInInstance() || player.isInTimedHuntingZone() || player.isInsideZone(ZoneId.SIEGE))
		{
			player.sendPacket(SystemMessageId.THE_CHARACTER_IS_IN_A_LOCATION_WHERE_IT_IS_IMPOSSIBLE_TO_USE_THIS_FUNCTION);
			return;
		}
		
		if (revenge.getShowLocationRemaining() > 0)
		{
			int price = LOCATION_PRICE[Math.Min(LOCATION_PRICE.Length - revenge.getShowLocationRemaining(), LOCATION_PRICE.Length - 1)];
			if (player.reduceAdena("Revenge find location", price, player, true))
			{
				revenge.setShowLocationRemaining(revenge.getShowLocationRemaining() - 1);
				player.sendPacket(new ExPvpBookShareRevengeKillerLocationPacket(killer));
				player.sendPacket(new ExPvpBookShareRevengeListPacket(player));
			}
		}
	}
	
	private bool checkTeleportConditions(Player player, Player killer)
	{
		if ((killer == null) || !killer.isOnline())
		{
			player.sendPacket(SystemMessageId.THE_ENEMY_IS_OFFLINE_AND_CANNOT_BE_FOUND_RIGHT_NOW);
			return false;
		}
		if (killer.isTeleporting() || killer.isInsideZone(ZoneId.PEACE) || killer.isInInstance() || killer.isInTimedHuntingZone() || killer.isInsideZone(ZoneId.SIEGE) || killer.isInsideZone(ZoneId.NO_BOOKMARK))
		{
			player.sendPacket(SystemMessageId.THE_CHARACTER_IS_IN_A_LOCATION_WHERE_IT_IS_IMPOSSIBLE_TO_USE_THIS_FUNCTION);
			return false;
		}
		if (killer.isDead())
		{
			player.sendPacket(SystemMessageId.THE_CHARACTER_IS_IN_A_LOCATION_WHERE_IT_IS_IMPOSSIBLE_TO_USE_THIS_FUNCTION);
			return false;
		}
		
		if (player.isInInstance() || player.isInTimedHuntingZone() || player.isInsideZone(ZoneId.SIEGE))
		{
			player.sendPacket(SystemMessageId.THE_CHARACTER_IS_IN_A_LOCATION_WHERE_IT_IS_IMPOSSIBLE_TO_USE_THIS_FUNCTION);
			return false;
		}
		if (player.isDead())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_TELEPORT_WHILE_YOU_ARE_DEAD);
			return false;
		}
		if (player.isInCombat() || player.isDisabled())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_WHILE_IN_COMBAT);
			return false;
		}
		
		return true;
	}
	
	public void teleportToKiller(Player player, String killerName)
	{
		List<RevengeHistoryHolder> history = REVENGE_HISTORY.get(player.getObjectId());
		if (history == null)
		{
			return;
		}
		
		RevengeHistoryHolder revenge = null;
		foreach (RevengeHistoryHolder holder in history)
		{
			if (holder.getKillerName().equals(killerName))
			{
				revenge = holder;
				break;
			}
		}
		
		if (revenge == null)
		{
			return;
		}
		
		if (revenge.wasShared())
		{
			return;
		}
		
		Player killer = World.getInstance().getPlayer(killerName);
		if (!checkTeleportConditions(player, killer))
		{
			return;
		}
		
		if (revenge.getTeleportRemaining() > 0)
		{
			int price = TELEPORT_PRICE[Math.Min(TELEPORT_PRICE.Length - revenge.getTeleportRemaining(), TELEPORT_PRICE.Length - 1)];
			if (player.destroyItemByItemId("Revenge Teleport", Inventory.LCOIN_ID, price, player, true))
			{
				revenge.setTeleportRemaining(revenge.getTeleportRemaining() - 1);
				HIDE_SKILL.getSkill().applyEffects(player, player);
				foreach (Summon summon in player.getServitorsAndPets())
				{
					HIDE_SKILL.getSkill().applyEffects(summon, summon);
				}
				player.teleToLocation(killer.getLocation().ToLocationHeading());
			}
		}
	}
	
	public void teleportToSharedKiller(Player player, String victimName, String killerName)
	{
		if (player.getName().equals(killerName))
		{
			return;
		}
		
		List<RevengeHistoryHolder> history = REVENGE_HISTORY.get(player.getObjectId());
		if (history == null)
		{
			return;
		}
		
		RevengeHistoryHolder revenge = null;
		foreach (RevengeHistoryHolder holder in history)
		{
			if (holder.getVictimName().equals(victimName) && holder.getKillerName().equals(killerName))
			{
				revenge = holder;
				break;
			}
		}
		
		if (revenge == null)
		{
			return;
		}
		
		if (!revenge.wasShared())
		{
			return;
		}
		
		Player killer = World.getInstance().getPlayer(killerName);
		if (!checkTeleportConditions(player, killer))
		{
			return;
		}
		
		if ((revenge.getSharedTeleportRemaining() > 0) && player.destroyItemByItemId("Revenge Teleport", Inventory.LCOIN_ID, 100, player, true))
		{
			revenge.setSharedTeleportRemaining(revenge.getSharedTeleportRemaining() - 1);
			HIDE_SKILL.getSkill().applyEffects(player, player);
			foreach (Summon summon in player.getServitorsAndPets())
			{
				HIDE_SKILL.getSkill().applyEffects(summon, summon);
			}
			player.teleToLocation(killer.getLocation().ToLocationHeading());
		}
	}
	
	public void requestHelp(Player player, Player killer, int type)
	{
		List<RevengeHistoryHolder> history = REVENGE_HISTORY.get(player.getObjectId());
		if (history == null)
		{
			return;
		}
		
		RevengeHistoryHolder revenge = null;
		foreach (RevengeHistoryHolder holder in history)
		{
			if (holder.getKillerName().equals(killer.getName()))
			{
				revenge = holder;
				break;
			}
		}
		
		if (revenge == null)
		{
			return;
		}
		
		if (revenge.wasShared())
		{
			return;
		}
		
		if (player.reduceAdena("Revenge request help", 100000, player, true))
		{
			DateTime currentTime = DateTime.UtcNow;
			revenge.setShared(true);
			revenge.setType(RevengeType.OWN_HELP_REQUEST);
			revenge.setShareTime(currentTime);
			
			List<Player> targets = new();
			if (type == 1)
			{
				if (player.getClan() != null)
				{
					foreach (ClanMember member in player.getClan().getMembers())
					{
						if (member.isOnline())
						{
							targets.add(member.getPlayer());
						}
						else
						{
							saveToRevengeHistory(player, killer, revenge, currentTime, member.getObjectId());
						}
					}
				}
			}
			else if (type == 2)
			{
				foreach (int playerObjectId in RankManager.getInstance().getTop50())
				{
					Player plr = World.getInstance().getPlayer(playerObjectId);
					if (plr != null)
					{
						targets.add(plr);
					}
					else
					{
						saveToRevengeHistory(player, killer, revenge, currentTime, playerObjectId);
					}
				}
			}
			
			foreach (Player target in targets)
			{
				if (target == killer)
				{
					continue;
				}
				
				int targetObjectId = target.getObjectId();
				saveToRevengeHistory(player, killer, revenge, currentTime, targetObjectId);
				
				target.sendPacket(new ExPvpBookShareRevengeNewRevengeInfoPacket(player.getName(), killer.getName(), RevengeType.HELP_REQUEST));
				target.sendPacket(new ExPvpBookShareRevengeListPacket(target));
			}
		}
		
		player.sendPacket(new ExPvpBookShareRevengeListPacket(player));
	}
	
	private void saveToRevengeHistory(Player player, Player killer, RevengeHistoryHolder revenge, DateTime currentTime, int objectId)
	{
		List<RevengeHistoryHolder> targetHistory = REVENGE_HISTORY.containsKey(objectId) ? REVENGE_HISTORY.get(objectId) : new();
		foreach (RevengeHistoryHolder holder in targetHistory)
		{
			if (holder.getVictimName().equals(player.getName()) && holder.getKillerName().equals(killer.getName()) && (holder != revenge))
			{
				targetHistory.Remove(holder);
				break;
			}
		}
		
		targetHistory.add(new RevengeHistoryHolder(killer, player, RevengeType.HELP_REQUEST, 1, revenge.getKillTime(), currentTime));
		REVENGE_HISTORY.put(objectId, targetHistory);
	}
	
	public List<RevengeHistoryHolder> getHistory(Player player)
	{
		return REVENGE_HISTORY.get(player.getObjectId());
	}
	
	public static RevengeHistoryManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly RevengeHistoryManager INSTANCE = new RevengeHistoryManager();
	}
}