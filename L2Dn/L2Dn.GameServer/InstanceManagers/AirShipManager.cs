using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

public class AirShipManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AirShipManager));
	
	private const string LOAD_DB = "SELECT * FROM airships";
	private const string ADD_DB = "INSERT INTO airships (owner_id,fuel) VALUES (?,?)";
	private const string UPDATE_DB = "UPDATE airships SET fuel=? WHERE owner_id=?";
	
	private CreatureTemplate _airShipTemplate = null;
	private readonly Map<int, StatSet> _airShipsInfo = new();
	private readonly Map<int, AirShip> _airShips = new();
	private readonly Map<int, AirShipTeleportList> _teleports = new();
	
	protected AirShipManager()
	{
		StatSet npcDat = new StatSet();
		npcDat.set("npcId", 9);
		npcDat.set("level", 0);
		npcDat.set("jClass", "boat");
		npcDat.set("baseSTR", 0);
		npcDat.set("baseCON", 0);
		npcDat.set("baseDEX", 0);
		npcDat.set("baseINT", 0);
		npcDat.set("baseWIT", 0);
		npcDat.set("baseMEN", 0);
		npcDat.set("baseShldDef", 0);
		npcDat.set("baseShldRate", 0);
		npcDat.set("baseAccCombat", 38);
		npcDat.set("baseEvasRate", 38);
		npcDat.set("baseCritRate", 38);
		npcDat.set("collision_radius", 0);
		npcDat.set("collision_height", 0);
		npcDat.set("sex", "male");
		npcDat.set("type", "");
		npcDat.set("baseAtkRange", 0);
		npcDat.set("baseMpMax", 0);
		npcDat.set("baseCpMax", 0);
		npcDat.set("rewardExp", 0);
		npcDat.set("rewardSp", 0);
		npcDat.set("basePAtk", 0);
		npcDat.set("baseMAtk", 0);
		npcDat.set("basePAtkSpd", 0);
		npcDat.set("aggroRange", 0);
		npcDat.set("baseMAtkSpd", 0);
		npcDat.set("rhand", 0);
		npcDat.set("lhand", 0);
		npcDat.set("armor", 0);
		npcDat.set("baseWalkSpd", 0);
		npcDat.set("baseRunSpd", 0);
		npcDat.set("name", "AirShip");
		npcDat.set("baseHpMax", 50000);
		npcDat.set("baseHpReg", 3.0e-3f);
		npcDat.set("baseMpReg", 3.0e-3f);
		npcDat.set("basePDef", 100);
		npcDat.set("baseMDef", 100);
		_airShipTemplate = new CreatureTemplate(npcDat);
		load();
	}
	
	public AirShip getNewAirShip(int x, int y, int z, int heading)
	{
		AirShip airShip = new AirShip(_airShipTemplate);
		airShip.setHeading(heading);
		airShip.setXYZInvisible(x, y, z);
		airShip.spawnMe();
		airShip.getStat().setMoveSpeed(280);
		airShip.getStat().setRotationSpeed(2000);
		return airShip;
	}
	
	public AirShip getNewAirShip(int x, int y, int z, int heading, int ownerId)
	{
		StatSet info = _airShipsInfo.get(ownerId);
		if (info == null)
		{
			return null;
		}
		
		AirShip airShip;
		if (_airShips.containsKey(ownerId))
		{
			airShip = _airShips.get(ownerId);
			airShip.refreshId();
		}
		else
		{
			airShip = new ControllableAirShip(_airShipTemplate, ownerId);
			_airShips.put(ownerId, airShip);
			
			airShip.setMaxFuel(600);
			airShip.setFuel(info.getInt("fuel"));
			airShip.getStat().setMoveSpeed(280);
			airShip.getStat().setRotationSpeed(2000);
		}
		
		airShip.setHeading(heading);
		airShip.setXYZInvisible(x, y, z);
		airShip.spawnMe();
		return airShip;
	}
	
	public void removeAirShip(AirShip ship)
	{
		if (ship.getOwnerId() != 0)
		{
			storeInDb(ship.getOwnerId());
			StatSet info = _airShipsInfo.get(ship.getOwnerId());
			if (info != null)
			{
				info.set("fuel", ship.getFuel());
			}
		}
	}
	
	public bool hasAirShipLicense(int ownerId)
	{
		return _airShipsInfo.containsKey(ownerId);
	}
	
	public void registerLicense(int ownerId)
	{
		if (!_airShipsInfo.containsKey(ownerId))
		{
			StatSet info = new StatSet();
			info.set("fuel", 600);
			_airShipsInfo.put(ownerId, info);
			
			try 
			{
				Connection con = DatabaseFactory.getConnection();
				PreparedStatement ps = con.prepareStatement(ADD_DB);
				ps.setInt(1, ownerId);
				ps.setInt(2, info.getInt("fuel"));
				ps.executeUpdate();
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Could not add new airship license: " + e);
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Error while initializing: " + e);
			}
		}
	}
	
	public bool hasAirShip(int ownerId)
	{
		AirShip ship = _airShips.get(ownerId);
		return (ship != null) && (ship.isSpawned() || ship.isTeleporting());
	}
	
	public void registerAirShipTeleportList(int dockId, int locationId, VehiclePathPoint[][] tp, int[] fuelConsumption)
	{
		if (tp.Length != fuelConsumption.Length)
		{
			return;
		}
		
		_teleports.put(dockId, new AirShipTeleportList(locationId, fuelConsumption, tp));
	}
	
	public void sendAirShipTeleportList(Player player)
	{
		if ((player == null) || !player.isInAirShip())
		{
			return;
		}
		
		AirShip ship = player.getAirShip();
		if (!ship.isCaptain(player) || !ship.isInDock() || ship.isMoving())
		{
			return;
		}
		
		int dockId = ship.getDockId();
		if (!_teleports.containsKey(dockId))
		{
			return;
		}
		
		AirShipTeleportList all = _teleports.get(dockId);
		player.sendPacket(new ExAirShipTeleportList(all.getLocation(), all.getRoute(), all.getFuel()));
	}
	
	public VehiclePathPoint[] getTeleportDestination(int dockId, int index)
	{
		AirShipTeleportList all = _teleports.get(dockId);
		if (all == null)
		{
			return null;
		}
		
		if ((index < -1) || (index >= all.getRoute().Length))
		{
			return null;
		}
		
		return all.getRoute()[index + 1];
	}
	
	public int getFuelConsumption(int dockId, int index)
	{
		AirShipTeleportList all = _teleports.get(dockId);
		if (all == null)
		{
			return 0;
		}
		
		if ((index < -1) || (index >= all.getFuel().Length))
		{
			return 0;
		}
		
		return all.getFuel()[index + 1];
	}
	
	private void load()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			Statement s = con.createStatement();
			ResultSet rs = s.executeQuery(LOAD_DB);
			StatSet info;
			while (rs.next())
			{
				info = new StatSet();
				info.set("fuel", rs.getInt("fuel"));
				_airShipsInfo.put(rs.getInt("owner_id"), info);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not load airships table: " + e);
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error while initializing: " + e);
		}
		LOGGER.Info(GetType().Name +": Loaded " + _airShipsInfo.size() + " private airships");
	}
	
	private void storeInDb(int ownerId)
	{
		StatSet info = _airShipsInfo.get(ownerId);
		if (info == null)
		{
			return;
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(UPDATE_DB);
			ps.setInt(1, info.getInt("fuel"));
			ps.setInt(2, ownerId);
			ps.executeUpdate();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not update airships table: " + e);
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error while save: " + e);
		}
	}
	
	public static AirShipManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly AirShipManager INSTANCE = new AirShipManager();
	}
}