using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.InstanceManagers;

public class CastleManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CastleManager));

	private readonly Map<int, Castle> _castles = new();
	private readonly Map<int, DateTime> _castleSiegeDate = new();

    private static readonly int[] _castleCirclets = [0, 6838, 6835, 6839, 6837, 6840, 6834, 6836, 8182, 8183];

	public Castle? findNearestCastle(WorldObject obj, long maxDistanceValue = long.MaxValue)
	{
		Castle? nearestCastle = getCastle(obj);
		if (nearestCastle == null)
		{
			long maxDistance = maxDistanceValue;
			foreach (Castle castle in _castles.Values)
			{
				double distance = castle.getDistance(obj);
				if (maxDistance > distance)
				{
					maxDistance = (long) distance;
					nearestCastle = castle;
				}
			}
		}
		return nearestCastle;
	}

	public Castle? getCastleById(int castleId)
	{
		return _castles.get(castleId);
	}

	public Castle? getCastleByOwner(Clan clan)
	{
		if (clan == null)
		{
			return null;
		}
		foreach (Castle temp in _castles.Values)
		{
			if (temp.getOwnerId() == clan.getId())
			{
				return temp;
			}
		}
		return null;
	}

	public Castle? getCastle(string name)
	{
		foreach (Castle temp in _castles.Values)
		{
			if (temp.getName().equalsIgnoreCase(name.Trim()))
			{
				return temp;
			}
		}
		return null;
	}

	public Castle? getCastle(Location3D location)
	{
		foreach (Castle temp in _castles.Values)
		{
			if (temp.checkIfInZone(location))
			{
				return temp;
			}
		}
		return null;
	}

	public Castle? getCastle(WorldObject activeObject)
	{
		return getCastle(activeObject.Location.Location3D);
	}

	public ICollection<Castle> getCastles()
	{
		return _castles.Values;
	}

	public bool hasOwnedCastle()
	{
		bool hasOwnedCastle = false;
		foreach (Castle castle in _castles.Values)
		{
			if (castle.getOwnerId() > 0)
			{
				hasOwnedCastle = true;
				break;
			}
		}
		return hasOwnedCastle;
	}

	public int getCircletByCastleId(int castleId)
	{
		if (castleId > 0 && castleId < 10)
		{
			return _castleCirclets[castleId];
		}
		return 0;
	}

	// remove this castle's circlets from the clan
	public void removeCirclet(Clan clan, int castleId)
	{
		foreach (ClanMember member in clan.getMembers())
		{
			removeCirclet(member, castleId);
		}
	}

	public void removeCirclet(ClanMember member, int castleId)
	{
		if (member == null)
		{
			return;
		}
		Player player = member.getPlayer();
		int circletId = getCircletByCastleId(castleId);
		if (circletId != 0)
		{
			// online-player circlet removal
			if (player != null)
			{
				try
				{
					Item? circlet = player.getInventory().getItemByItemId(circletId);
					if (circlet != null)
					{
						if (circlet.isEquipped())
						{
							player.getInventory().unEquipItemInSlot(circlet.getLocationSlot());
						}
						player.destroyItemByItemId("CastleCircletRemoval", circletId, 1, player, true);
					}
					return;
				}
				catch (NullReferenceException exception)
				{
                    LOGGER.Trace(exception);
					// continue removing offline
				}
			}
			// else offline-player circlet removal
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				int ownerId = member.getObjectId();
				ctx.Items.Where(item => item.OwnerId == ownerId && item.ItemId == circletId).ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Failed to remove castle circlets offline for player " + member.getName() + ": " + e);
			}
		}
	}

	public void loadInstances()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (var castle in ctx.Castles.Select(c => new { c.Id, c.Name }).OrderBy(c => c.Id))
				_castles.put(castle.Id, new Castle(castle.Id, castle.Name));

			if (_castles.Count == 0)
			{
				// TODO hack for now
				ctx.Castles.Add(new DbCastle()
				{
					Id = 1,
					Name = "Gludio",
				});
				ctx.Castles.Add(new DbCastle()
				{
					Id = 2,
					Name = "Dion",
				});
				ctx.Castles.Add(new DbCastle()
				{
					Id = 3,
					Name = "Giran",
				});
				ctx.Castles.Add(new DbCastle()
				{
					Id = 4,
					Name = "Oren",
				});
				ctx.Castles.Add(new DbCastle()
				{
					Id = 5,
					Name = "Aden",
				});

				ctx.SaveChanges();

				foreach (var castle in ctx.Castles.Select(c => new { c.Id, c.Name }).OrderBy(c => c.Id))
					_castles.put(castle.Id, new Castle(castle.Id, castle.Name));
			}

			LOGGER.Info(GetType().Name +": Loaded " + _castles.Values.Count + " castles.");
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Exception: loadCastleData():" + e);
		}
	}

	public void activateInstances()
	{
		foreach (Castle castle in _castles.Values)
		{
			castle.activateInstance();
		}
	}

	public void registerSiegeDate(int castleId, DateTime siegeDate)
	{
		_castleSiegeDate.put(castleId, siegeDate);
	}

	public int getSiegeDates(DateTime siegeDate)
	{
		int count = 0;
		foreach (DateTime date in _castleSiegeDate.Values)
		{
			if (Algorithms.Abs(date - siegeDate) < TimeSpan.FromMilliseconds(1000))
			{
				count++;
			}
		}
		return count;
	}

	public static CastleManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly CastleManager INSTANCE = new();
	}
}