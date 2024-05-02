using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Siege Guard Manager.
 * @author St3eT
 */
public class SiegeGuardManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SiegeGuardManager));
	private static readonly Set<Item> _droppedTickets = new();
	private static readonly Map<int, Set<Spawn>> _siegeGuardSpawn = new();
	
	protected SiegeGuardManager()
	{
		_droppedTickets.clear();
		load();
	}
	
	private void load()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (CastleSiegeGuard record in ctx.CastleSiegeGuards.Where(r => r.IsHired))
			{
				int npcId = record.NpcId;
				int x = record.X;
				int y = record.Y;
				int z = record.Z;
				Castle castle = CastleManager.getInstance().getCastle(x, y, z);
				if (castle == null)
				{
					LOGGER.Error($"Siege guard ticket cannot be placed! Castle is null at X: {x}, Y: {y}, Z: {z}");
					continue;
				}

				SiegeGuardHolder holder = getSiegeGuardByNpc(castle.getResidenceId(), npcId);
				if ((holder != null) && !castle.getSiege().isInProgress())
				{
					Item dropticket = new Item(holder.getItemId());
					dropticket.setItemLocation(ItemLocation.VOID);
					dropticket.dropMe(null, x, y, z);
					World.getInstance().addObject(dropticket);
					_droppedTickets.add(dropticket);
				}
			}

			LOGGER.Info(GetType().Name + ": Loaded " + _droppedTickets.Count + " siege guards tickets.");
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}
	
	/**
	 * Finds {@code SiegeGuardHolder} equals to castle id and npc id.
	 * @param castleId the ID of the castle
	 * @param itemId the ID of the item
	 * @return the {@code SiegeGuardHolder} for this castle ID and item ID if any, otherwise {@code null}
	 */
	public SiegeGuardHolder getSiegeGuardByItem(int castleId, int itemId)
	{
		foreach (SiegeGuardHolder holder in CastleData.getInstance().getSiegeGuardsForCastle(castleId))
		{
			if (holder.getItemId() == itemId)
			{
				return holder;
			}
		}
		return null;
	}
	
	/**
	 * Finds {@code SiegeGuardHolder} equals to castle id and npc id.
	 * @param castleId the ID of the castle
	 * @param npcId the ID of the npc
	 * @return the {@code SiegeGuardHolder} for this castle ID and npc ID if any, otherwise {@code null}
	 */
	public SiegeGuardHolder getSiegeGuardByNpc(int castleId, int npcId)
	{
		foreach (SiegeGuardHolder holder in CastleData.getInstance().getSiegeGuardsForCastle(castleId))
		{
			if (holder.getNpcId() == npcId)
			{
				return holder;
			}
		}
		return null;
	}
	
	/**
	 * Checks if {@code Player} is too much close to another ticket.
	 * @param player the Player
	 * @return {@code true} if {@code Player} is too much close to another ticket, {@code false} otherwise
	 */
	public bool isTooCloseToAnotherTicket(Player player)
	{
		foreach (Item ticket in _droppedTickets)
		{
			if (ticket.Distance3D(player) < 25)
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * Checks if castle is under npc limit.
	 * @param castleId the ID of the castle
	 * @param itemId the ID of the item
	 * @return {@code true} if castle is under npc limit, {@code false} otherwise
	 */
	public bool isAtNpcLimit(int castleId, int itemId)
	{
		SiegeGuardHolder holder = getSiegeGuardByItem(castleId, itemId);
		long count = 0;
		foreach (Item ticket in _droppedTickets)
		{
			if (ticket.getId() == itemId)
			{
				count++;
			}
		}
		return count >= holder.getMaxNpcAmout();
	}
	
	/**
	 * Adds ticket in current world.
	 * @param itemId the ID of the item
	 * @param player the Player
	 */
	public void addTicket(int itemId, Player player)
	{
		Castle castle = CastleManager.getInstance().getCastle(player);
		if (castle == null)
		{
			return;
		}
		
		if (isAtNpcLimit(castle.getResidenceId(), itemId))
		{
			return;
		}
		
		SiegeGuardHolder holder = getSiegeGuardByItem(castle.getResidenceId(), itemId);
		if (holder != null)
		{
			try 
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.CastleSiegeGuards.Add(new CastleSiegeGuard()
				{
					CastleId = (short)castle.getResidenceId(),
					NpcId = holder.getNpcId(),
					X = player.getX(),
					Y = player.getY(),
					Z = player.getZ(),
					Heading = player.getHeading(),
					RespawnDelay = TimeSpan.Zero,
					IsHired = true
				});

				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Warn("Error adding siege guard for castle " + castle.getName() + ": " + e);
			}
			
			spawnMercenary(player.Location, holder);
			Item dropticket = new Item(itemId);
			dropticket.setItemLocation(ItemLocation.VOID);
			dropticket.dropMe(null, player.getX(), player.getY(), player.getZ());
			World.getInstance().addObject(dropticket);
			_droppedTickets.add(dropticket);
		}
	}
	
	/**
	 * Spawns Siege Guard in current world.
	 * @param pos the object containing the spawn location coordinates
	 * @param holder SiegeGuardHolder holder
	 */
	private void spawnMercenary(Location location, SiegeGuardHolder holder)
	{
		NpcTemplate template = NpcData.getInstance().getTemplate(holder.getNpcId());
		if (template != null)
		{
			Defender npc = new Defender(template);
			npc.setCurrentHpMp(npc.getMaxHp(), npc.getMaxMp());
			npc.setDecayed(false);
			npc.setHeading(location.Heading);
			npc.spawnMe(location.X, location.Y, location.Z + 20);
			npc.scheduleDespawn(TimeSpan.FromSeconds(3));
			npc.setImmobilized(holder.isStationary());
		}
	}
	
	/**
	 * Delete all tickets from a castle.
	 * @param castleId the ID of the castle
	 */
	public void deleteTickets(int castleId)
	{
		foreach (Item ticket in _droppedTickets)
		{
			if ((ticket != null) && (getSiegeGuardByItem(castleId, ticket.getId()) != null))
			{
				ticket.decayMe();
				_droppedTickets.remove(ticket);
			}
		}
	}
	
	/**
	 * remove a single ticket and its associated spawn from the world (used when the castle lord picks up a ticket, for example).
	 * @param item the item ID
	 */
	public void removeTicket(Item item)
	{
		Castle castle = CastleManager.getInstance().getCastle(item);
		if (castle == null)
		{
			return;
		}
		
		SiegeGuardHolder holder = getSiegeGuardByItem(castle.getResidenceId(), item.getId());
		if (holder == null)
		{
			return;
		}
		
		removeSiegeGuard(holder.getNpcId(), item.Location.Location3D);
		_droppedTickets.remove(item);
	}
	
	/**
	 * Loads all siege guards for castle.
	 * @param castle the castle instance
	 */
	private void loadSiegeGuard(Castle castle)
	{
		try 
		{
			int castleId = castle.getResidenceId();
			bool isHired = castle.getOwnerId() > 0;

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (CastleSiegeGuard record in ctx.CastleSiegeGuards.Where(r =>
				         r.CastleId == castleId && r.IsHired == isHired))
			{
				Spawn spawn = new Spawn(record.NpcId);
				spawn.setAmount(1);
				spawn.Location = new Location(record.X, record.Y, record.Z, record.Heading);
				spawn.setRespawnDelay(record.RespawnDelay);
				spawn.setLocationId(0);

				getSpawnedGuards(castle.getResidenceId()).add(spawn);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error loading siege guard for castle " + castle.getName() + ": " + e);
		}
	}
	
	/**
	 * Remove single siege guard.
	 * @param npcId the ID of NPC
	 * @param pos
	 */
	public void removeSiegeGuard(int npcId, Location3D location)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CastleSiegeGuards
				.Where(r => r.NpcId == npcId && r.X == location.X && r.Y == location.Y && r.Z == location.Z)
				.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error deleting hired siege guard at " + location + " : " + e);
		}
	}
	
	/**
	 * Remove all siege guards for castle.
	 * @param castle the castle instance
	 */
	public void removeSiegeGuards(Castle castle)
	{
		try
		{
			int castleId = castle.getResidenceId();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CastleSiegeGuards.Where(r => r.CastleId == castleId && r.IsHired).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error deleting hired siege guard for castle " + castle.getName() + ": " + e);
		}
	}
	
	/**
	 * Spawn all siege guards for castle.
	 * @param castle the castle instance
	 */
	public void spawnSiegeGuard(Castle castle)
	{
		try
		{
			bool isHired = (castle.getOwnerId() > 0);
			loadSiegeGuard(castle);
			
			foreach (Spawn spawn in getSpawnedGuards(castle.getResidenceId()))
			{
				if (spawn != null)
				{
					spawn.init();
					if (isHired || (spawn.getRespawnDelay() == TimeSpan.Zero))
					{
						spawn.stopRespawn();
					}
					
					SiegeGuardHolder holder = getSiegeGuardByNpc(castle.getResidenceId(), spawn.getLastSpawn().getId());
					if (holder == null)
					{
						continue;
					}
					
					spawn.getLastSpawn().setImmobilized(holder.isStationary());
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error spawning siege guards for castle " + castle.getName(), e);
		}
	}
	
	/**
	 * Unspawn all siege guards for castle.
	 * @param castle the castle instance
	 */
	public void unspawnSiegeGuard(Castle castle)
	{
		foreach (Spawn spawn in getSpawnedGuards(castle.getResidenceId()))
		{
			if ((spawn != null) && (spawn.getLastSpawn() != null))
			{
				spawn.stopRespawn();
				spawn.getLastSpawn().doDie(spawn.getLastSpawn());
			}
		}
		getSpawnedGuards(castle.getResidenceId()).clear();
	}
	
	public Set<Spawn> getSpawnedGuards(int castleId)
	{
		return _siegeGuardSpawn.computeIfAbsent(castleId, key => new());
	}
	
	/**
	 * Gets the single instance of {@code MercTicketManager}.
	 * @return single instance of {@code MercTicketManager}
	 */
	public static SiegeGuardManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SiegeGuardManager INSTANCE = new SiegeGuardManager();
	}
}