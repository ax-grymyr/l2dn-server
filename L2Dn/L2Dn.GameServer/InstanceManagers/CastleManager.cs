using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

public class CastleManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CastleManager));
	
	private readonly Map<int, Castle> _castles = new();
	private readonly Map<int, long> _castleSiegeDate = new();
	
	private static readonly int[] _castleCirclets =
	{
		0,
		6838,
		6835,
		6839,
		6837,
		6840,
		6834,
		6836,
		8182,
		8183
	};
	
	public Castle findNearestCastle(WorldObject obj)
	{
		return findNearestCastle(obj, long.MaxValue);
	}
	
	public Castle findNearestCastle(WorldObject obj, long maxDistanceValue)
	{
		Castle nearestCastle = getCastle(obj);
		if (nearestCastle == null)
		{
			double distance;
			long maxDistance = maxDistanceValue;
			foreach (Castle castle in _castles.values())
			{
				distance = castle.getDistance(obj);
				if (maxDistance > distance)
				{
					maxDistance = (long) distance;
					nearestCastle = castle;
				}
			}
		}
		return nearestCastle;
	}
	
	public Castle getCastleById(int castleId)
	{
		return _castles.get(castleId);
	}
	
	public Castle getCastleByOwner(Clan clan)
	{
		if (clan == null)
		{
			return null;
		}
		foreach (Castle temp in _castles.values())
		{
			if (temp.getOwnerId() == clan.getId())
			{
				return temp;
			}
		}
		return null;
	}
	
	public Castle getCastle(String name)
	{
		foreach (Castle temp in _castles.values())
		{
			if (temp.getName().equalsIgnoreCase(name.Trim()))
			{
				return temp;
			}
		}
		return null;
	}
	
	public Castle getCastle(int x, int y, int z)
	{
		foreach (Castle temp in _castles.values())
		{
			if (temp.checkIfInZone(x, y, z))
			{
				return temp;
			}
		}
		return null;
	}
	
	public Castle getCastle(WorldObject activeObject)
	{
		return getCastle(activeObject.getX(), activeObject.getY(), activeObject.getZ());
	}
	
	public ICollection<Castle> getCastles()
	{
		return _castles.values();
	}
	
	public bool hasOwnedCastle()
	{
		bool hasOwnedCastle = false;
		foreach (Castle castle in _castles.values())
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
		if ((castleId > 0) && (castleId < 10))
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
					Item circlet = player.getInventory().getItemByItemId(circletId);
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
				catch (NullReferenceException e)
				{
					// continue removing offline
				}
			}
			// else offline-player circlet removal
			try 
			{
				using GameServerDbContext ctx = new();
				PreparedStatement ps = con.prepareStatement("DELETE FROM items WHERE owner_id = ? and item_id = ?");
				ps.setInt(1, member.getObjectId());
				ps.setInt(2, circletId);
				ps.execute();
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Failed to remove castle circlets offline for player " + member.getName() + ": " + e);
			}
		}
	}
	
	public void loadInstances()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			Statement s = con.createStatement();
			ResultSet rs = s.executeQuery("SELECT id FROM castle ORDER BY id");
			while (rs.next())
			{
				int castleId = rs.getInt("id");
				_castles.put(castleId, new Castle(castleId));
			}
			LOGGER.Info(GetType().Name +": Loaded " + _castles.values().Count + " castles.");
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Exception: loadCastleData():" + e);
		}
	}
	
	public void activateInstances()
	{
		foreach (Castle castle in _castles.values())
		{
			castle.activateInstance();
		}
	}
	
	public void registerSiegeDate(int castleId, long siegeDate)
	{
		_castleSiegeDate.put(castleId, siegeDate);
	}
	
	public int getSiegeDates(long siegeDate)
	{
		int count = 0;
		foreach (long date in _castleSiegeDate.values())
		{
			if (Math.Abs(date - siegeDate) < 1000)
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
		public static readonly CastleManager INSTANCE = new CastleManager();
	}
}