using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.InstanceManagers;

public class BoatManager
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(BoatManager));

	private readonly Map<int, Boat> _boats = new();
	private readonly bool[] _docksBusy = new bool[3];

	public const int TALKING_ISLAND = 1;
	public const int GLUDIN_HARBOR = 2;
	public const int RUNE_HARBOR = 3;

	public static BoatManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	protected BoatManager()
	{
		for (int i = 0; i < _docksBusy.Length; i++)
		{
			_docksBusy[i] = false;
		}
	}

	public Boat? getNewBoat(int boatId, int x, int y, int z, int heading)
	{
		if (!Config.ALLOW_BOAT)
		{
			return null;
		}

		StatSet npcDat = new StatSet();
		npcDat.set("npcId", boatId);
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

		// npcDat.set("name", "");
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
		npcDat.set("baseHpMax", 50000);
		npcDat.set("baseHpReg", 3.0e-3f);
		npcDat.set("baseMpReg", 3.0e-3f);
		npcDat.set("basePDef", 100);
		npcDat.set("baseMDef", 100);
		CreatureTemplate template = new CreatureTemplate(npcDat);
		Boat boat = new Boat(template);
		_boats.put(boat.ObjectId, boat);
		boat.setHeading(heading);
		boat.setXYZInvisible(new Location3D(x, y, z));
		boat.spawnMe();
		return boat;
	}

	/**
	 * @param boatId
	 * @return
	 */
	public Boat? getBoat(int boatId)
	{
		return _boats.get(boatId);
	}

	/**
	 * Lock/unlock dock so only one ship can be docked
	 * @param h Dock Id
	 * @param value True if dock is locked
	 */
	public void dockShip(int h, bool value)
	{
		try
		{
			_docksBusy[h] = value;
		}
		catch (IndexOutOfRangeException e)
		{
            _logger.Trace(e);
			// Ignore.
		}
	}

	/**
	 * Check if dock is busy
	 * @param h Dock Id
	 * @return Trye if dock is locked
	 */
	public bool dockBusy(int h)
	{
		try
		{
			return _docksBusy[h];
		}
		catch (IndexOutOfRangeException e)
		{
            _logger.Trace(e);
			return false;
		}
	}

	/**
	 * Broadcast one packet in both path points
	 * @param point1
	 * @param point2
	 * @param packet
	 */
	public void broadcastPacket<TPacket>(VehiclePathPoint point1, VehiclePathPoint point2, TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		broadcastPacketsToPlayers(point1, point2).SendPackets(packet);
	}

	/**
	 * Broadcast several packets in both path points
	 * @param point1
	 * @param point2
	 * @param packets
	 */
	public PacketSendUtil broadcastPackets(VehiclePathPoint point1, VehiclePathPoint point2)
	{
		return broadcastPacketsToPlayers(point1, point2);
	}

	private PacketSendUtil broadcastPacketsToPlayers(VehiclePathPoint point1, VehiclePathPoint point2)
	{
		IEnumerable<Player> players = World.getInstance().getPlayers().Where(player =>
			double.Hypot(player.getX() - point1.Location.X, player.getY() - point1.Location.Y) <
            Config.BOAT_BROADCAST_RADIUS || //
			double.Hypot(player.getX() - point2.Location.X, player.getY() - point2.Location.Y) <
            Config.BOAT_BROADCAST_RADIUS);

		return new PacketSendUtil(players);
	}

	private static class SingletonHolder
	{
		public static readonly BoatManager INSTANCE = new BoatManager();
	}
}