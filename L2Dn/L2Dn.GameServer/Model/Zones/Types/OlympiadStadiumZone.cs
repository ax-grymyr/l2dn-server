using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * An olympiad stadium
 * @author durgus, DS
 */
public class OlympiadStadiumZone: ZoneRespawn
{
	private readonly List<Door> _doors = new(2);
	private readonly List<Spawn> _buffers = new(2);
	private readonly List<Location> _spectatorLocations = new(1);

	public OlympiadStadiumZone(int id): base(id)
	{
		AbstractZoneSettings settings = ZoneManager.getSettings(getName());
		if (settings == null)
		{
			settings = new Settings();
		}

		setSettings(settings);
	}

	public class Settings: AbstractZoneSettings
	{
		private OlympiadGameTask _task = null;

		public Settings()
		{
		}

		public OlympiadGameTask getOlympiadTask()
		{
			return _task;
		}

		public void setTask(OlympiadGameTask task)
		{
			_task = task;
		}

		public override void clear()
		{
			_task = null;
		}
	}

	public override Settings getSettings()
	{
		return (Settings)base.getSettings();
	}

	public override void parseLoc(int x, int y, int z, String type)
	{
		if ((type != null) && type.equals("spectatorSpawn"))
		{
			_spectatorLocations.add(new Location(x, y, z));
		}
		else
		{
			base.parseLoc(x, y, z, type);
		}
	}

	public void registerTask(OlympiadGameTask task)
	{
		getSettings().setTask(task);
	}

	protected override void onEnter(Creature creature)
	{
		if ((getSettings().getOlympiadTask() != null) && getSettings().getOlympiadTask().isBattleStarted())
		{
			creature.setInsideZone(ZoneId.PVP, true);
			if (creature.isPlayer())
			{
				creature.sendPacket(SystemMessageId.YOU_HAVE_ENTERED_A_COMBAT_ZONE);
				getSettings().getOlympiadTask().getGame().sendOlympiadInfo(creature);
			}
		}

		if (creature.isPlayable())
		{
			Player player = creature.getActingPlayer();
			if (player != null)
			{
				// only participants, observers and GMs allowed
				if (!player.canOverrideCond(PlayerCondOverride.ZONE_CONDITIONS) && !player.isInOlympiadMode() &&
				    !player.inObserverMode())
				{
					ThreadPool.execute(new KickPlayer(player));
				}
				else
				{
					// check for pet
					Summon pet = player.getPet();
					if (pet != null)
					{
						pet.unSummon(player);
					}
				}
			}
		}
	}

	protected override void onExit(Creature creature)
	{
		if ((getSettings().getOlympiadTask() != null) && getSettings().getOlympiadTask().isBattleStarted())
		{
			creature.setInsideZone(ZoneId.PVP, false);
			if (creature.isPlayer())
			{
				creature.sendPacket(SystemMessageId.YOU_HAVE_LEFT_A_COMBAT_ZONE);
				creature.sendPacket(default(ExOlympiadMatchEndPacket));
			}
		}
	}

	private class KickPlayer: Runnable
	{
		private Player _player;

		public KickPlayer(Player player)
		{
			_player = player;
		}

		public void run()
		{
			if (_player != null)
			{
				_player.getServitors().values().forEach(s => s.unSummon(_player));
				_player.teleToLocation(TeleportWhereType.TOWN, null);
				_player = null;
			}
		}
	}

	public List<Door> getDoors()
	{
		return _doors;
	}

	public List<Spawn> getBuffers()
	{
		return _buffers;
	}

	public List<Location> getSpectatorSpawns()
	{
		return _spectatorLocations;
	}
}