using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Brutallis
 */
public class HeavenlyRiftManager
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(HeavenlyRiftManager));
	private static readonly TimeSpan _despawnTime = TimeSpan.FromMilliseconds(1800000);

    private static readonly Zone _zone =
        ZoneManager.Instance.getZoneByName("heavenly_rift") ??
        throw new InvalidOperationException("Zone heavenly_rift not found");

	public static Zone getZone()
	{
		return _zone;
	}

	public static int getAliveNpcCount(int npcId)
	{
		int result = 0;
		foreach (Creature creature in _zone.getCharactersInside())
		{
			if (creature.isMonster() && !creature.isDead() && creature.Id == npcId)
			{
				result++;
			}
		}

		return result;
	}

	public static void startEvent20Bomb(Player player)
	{
		_zone.broadcastPacket(new ExShowScreenMessagePacket(NpcStringId.SET_OFF_BOMBS_AND_GET_TREASURES, 2, 5000));
		spawnMonster(18003, 113352, 12936, 10976, _despawnTime);
		spawnMonster(18003, 113592, 13272, 10976, _despawnTime);
		spawnMonster(18003, 113816, 13592, 10976, _despawnTime);
		spawnMonster(18003, 113080, 13192, 10976, _despawnTime);
		spawnMonster(18003, 113336, 13528, 10976, _despawnTime);
		spawnMonster(18003, 113560, 13832, 10976, _despawnTime);
		spawnMonster(18003, 112776, 13512, 10976, _despawnTime);
		spawnMonster(18003, 113064, 13784, 10976, _despawnTime);
		spawnMonster(18003, 112440, 13848, 10976, _despawnTime);
		spawnMonster(18003, 112728, 14104, 10976, _despawnTime);
		spawnMonster(18003, 112760, 14600, 10976, _despawnTime);
		spawnMonster(18003, 112392, 14456, 10976, _despawnTime);
		spawnMonster(18003, 112104, 14184, 10976, _despawnTime);
		spawnMonster(18003, 111816, 14488, 10976, _despawnTime);
		spawnMonster(18003, 112104, 14760, 10976, _despawnTime);
		spawnMonster(18003, 112392, 15032, 10976, _despawnTime);
		spawnMonster(18003, 112120, 15288, 10976, _despawnTime);
		spawnMonster(18003, 111784, 15064, 10976, _despawnTime);
		spawnMonster(18003, 111480, 14824, 10976, _despawnTime);
		spawnMonster(18003, 113144, 14216, 10976, _despawnTime);
	}

	public static void startEventTower(Player player)
	{
		_zone.broadcastPacket(new ExShowScreenMessagePacket(NpcStringId.PROTECT_THE_CENTRAL_TOWER_FROM_DIVINE_ANGELS, 2,
			5000));
		spawnMonster(18004, 112648, 14072, 10976, _despawnTime);
		ThreadPool.schedule(() =>
		{
			for (int i = 0; i < 20; ++i)
			{
				spawnMonster(20139, 112696, 13960, 10958, _despawnTime);
			}
		}, 10000);
	}

	public static void startEvent40Angels(Player player)
	{
		_zone.broadcastPacket(new ExShowScreenMessagePacket(NpcStringId.DESTROY_WEAKENED_DIVINE_ANGELS, 2, 5000));
		for (int i = 0; i < 40; ++i)
		{
			spawnMonster(20139, 112696, 13960, 10958, _despawnTime);
		}
	}

	private static void spawnMonster(int npcId, int x, int y, int z, TimeSpan despawnTime)
	{
		try
		{
			Spawn spawn = new Spawn(npcId);
			spawn.Location = new Location(x, y, z, 0);
			Npc? npc = spawn.doSpawn();
            if (npc == null)
            {
                _logger.Error("Failed to spawn monster with id: " + npcId);
                return;
            }

			npc.scheduleDespawn(despawnTime);
		}
		catch (Exception e)
		{
            _logger.Error(e);
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
			foreach (Creature creature in _zone.getCharactersInside())
			{
				if (creature.isPlayer())
				{
					creature.teleToLocation(new Location3D(114264, 13352, -5104));
				}
				else if (creature.isNpc() && creature.Id != 30401)
				{
					creature.decayMe();
				}
			}

			_npc.setBusy(false);
		}
	}
}