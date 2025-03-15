using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * An olympiad stadium
 * @author durgus, DS
 */
public class OlympiadStadiumZone: ZoneRespawn
{
	private readonly List<Door> _doors = [];
	private readonly List<Spawn> _buffers = [];
	private ImmutableArray<Location3D> _spectatorLocations = ImmutableArray<Location3D>.Empty;

	public OlympiadStadiumZone(int id, ZoneForm form): base(id, form)
	{
		AbstractZoneSettings? settings = ZoneManager.getSettings(getName());
		if (settings == null)
		{
			settings = new Settings();
		}

		setSettings(settings);
	}

	public sealed class Settings: AbstractZoneSettings
	{
		private OlympiadGameTask? _task;

		public OlympiadGameTask? getOlympiadTask() => _task;

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

	public override void parseLoc(Location3D location, string type)
	{
		if (string.Equals(type, "spectatorSpawn"))
		{
			_spectatorLocations = _spectatorLocations.Add(location);
		}
		else
		{
			base.parseLoc(location, type);
		}
	}

	public void registerTask(OlympiadGameTask task)
	{
		getSettings().setTask(task);
	}

	protected override void onEnter(Creature creature)
    {
        OlympiadGameTask? olympiadTask = getSettings().getOlympiadTask();
		if (olympiadTask != null && olympiadTask.isBattleStarted())
		{
			creature.setInsideZone(ZoneId.PVP, true);
			if (creature.isPlayer())
			{
				creature.sendPacket(SystemMessageId.YOU_HAVE_ENTERED_A_COMBAT_ZONE);
                olympiadTask.getGame()?.sendOlympiadInfo(creature);
			}
		}

		if (creature.isPlayable())
		{
			Player? player = creature.getActingPlayer();
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
					Summon? pet = player.getPet();
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
        OlympiadGameTask? olympiadTask = getSettings().getOlympiadTask();
		if (olympiadTask != null && olympiadTask.isBattleStarted())
		{
			creature.setInsideZone(ZoneId.PVP, false);
			if (creature.isPlayer())
			{
				creature.sendPacket(SystemMessageId.YOU_HAVE_LEFT_A_COMBAT_ZONE);
				creature.sendPacket(default(ExOlympiadMatchEndPacket));
			}
		}
	}

	private class KickPlayer(Player player): Runnable
    {
		private Player? _player = player;

        public void run()
        {
            if (_player == null)
                return;

            _player.getServitors().Values.ForEach(s => s.unSummon(_player));
            _player.teleToLocation(TeleportWhereType.TOWN, null);
            _player = null;
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

	public ImmutableArray<Location3D> getSpectatorSpawns()
	{
		return _spectatorLocations;
	}
}