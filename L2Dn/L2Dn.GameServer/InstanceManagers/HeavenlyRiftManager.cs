using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Brutallis
 */
public class HeavenlyRiftManager
{
	protected static readonly ZoneType ZONE = ZoneManager.getInstance().getZoneByName("heavenly_rift");

	public static ZoneType getZone()
	{
		return ZONE;
	}

	public static int getAliveNpcCount(int npcId)
	{
		int result = 0;
		foreach (Creature creature in ZONE.getCharactersInside())
		{
			if (creature.isMonster() && !creature.isDead() && (creature.getId() == npcId))
			{
				result++;
			}
		}

		return result;
	}

	public static void startEvent20Bomb(Player player)
	{
		ZONE.broadcastPacket(new ExShowScreenMessagePacket(NpcStringId.SET_OFF_BOMBS_AND_GET_TREASURES, 2, 5000));
		spawnMonster(18003, 113352, 12936, 10976, 1800000);
		spawnMonster(18003, 113592, 13272, 10976, 1800000);
		spawnMonster(18003, 113816, 13592, 10976, 1800000);
		spawnMonster(18003, 113080, 13192, 10976, 1800000);
		spawnMonster(18003, 113336, 13528, 10976, 1800000);
		spawnMonster(18003, 113560, 13832, 10976, 1800000);
		spawnMonster(18003, 112776, 13512, 10976, 1800000);
		spawnMonster(18003, 113064, 13784, 10976, 1800000);
		spawnMonster(18003, 112440, 13848, 10976, 1800000);
		spawnMonster(18003, 112728, 14104, 10976, 1800000);
		spawnMonster(18003, 112760, 14600, 10976, 1800000);
		spawnMonster(18003, 112392, 14456, 10976, 1800000);
		spawnMonster(18003, 112104, 14184, 10976, 1800000);
		spawnMonster(18003, 111816, 14488, 10976, 1800000);
		spawnMonster(18003, 112104, 14760, 10976, 1800000);
		spawnMonster(18003, 112392, 15032, 10976, 1800000);
		spawnMonster(18003, 112120, 15288, 10976, 1800000);
		spawnMonster(18003, 111784, 15064, 10976, 1800000);
		spawnMonster(18003, 111480, 14824, 10976, 1800000);
		spawnMonster(18003, 113144, 14216, 10976, 1800000);
	}

	public static void startEventTower(Player player)
	{
		ZONE.broadcastPacket(new ExShowScreenMessagePacket(NpcStringId.PROTECT_THE_CENTRAL_TOWER_FROM_DIVINE_ANGELS, 2,
			5000));
		spawnMonster(18004, 112648, 14072, 10976, 1800000);
		ThreadPool.schedule(() =>
		{
			for (int i = 0; i < 20; ++i)
			{
				spawnMonster(20139, 112696, 13960, 10958, 1800000);
			}
		}, 10000);
	}

	public static void startEvent40Angels(Player player)
	{
		ZONE.broadcastPacket(new ExShowScreenMessagePacket(NpcStringId.DESTROY_WEAKENED_DIVINE_ANGELS, 2, 5000));
		for (int i = 0; i < 40; ++i)
		{
			spawnMonster(20139, 112696, 13960, 10958, 1800000);
		}
	}

	private static void spawnMonster(int npcId, int x, int y, int z, long despawnTime)
	{
		try
		{
			Spawn spawn = new Spawn(npcId);
			Location location = new Location(x, y, z);
			spawn.setLocation(location);
			Npc npc = spawn.doSpawn();
			npc.scheduleDespawn(despawnTime);
		}
		catch (Exception e)
		{
		}
	}

	public class ClearZoneTask: Runnable
	{
		private readonly Npc _npc;

		public ClearZoneTask(Npc npc)
		{
			_npc = npc;
		}

		public void run()
		{
			foreach (Creature creature in ZONE.getCharactersInside())
			{
				if (creature.isPlayer())
				{
					creature.teleToLocation(114264, 13352, -5104);
				}
				else if (creature.isNpc() && (creature.getId() != 30401))
				{
					creature.decayMe();
				}
			}

			_npc.setBusy(false);
		}
	}
}