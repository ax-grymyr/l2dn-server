using System.Collections.Immutable;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author JIV
 */
public class OlympiadStadium
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(OlympiadStadium));
	
	private readonly OlympiadStadiumZone _zone;
	private readonly int _stadiumId;
	private readonly Instance _instance;
	private readonly List<Spawn> _buffers;
	private OlympiadGameTask _task = null;
	
	public OlympiadStadium(OlympiadStadiumZone olyzone, int stadiumId)
	{
		_zone = olyzone;
		_stadiumId = stadiumId;
		_instance = InstanceManager.getInstance().createInstance(olyzone.getInstanceTemplateId(), null);
		_buffers = _instance.getNpcs().Select(n => n.getSpawn()).ToList();
		_buffers.Select(s => s.getLastSpawn()).forEach(n => n.deleteMe());
	}
	
	public OlympiadStadiumZone getZone()
	{
		return _zone;
	}
	
	public void registerTask(OlympiadGameTask task)
	{
		_task = task;
	}
	
	public OlympiadGameTask getTask()
	{
		return _task;
	}
	
	public Instance getInstance()
	{
		return _instance;
	}
	
	public void openDoors()
	{
		_instance.getDoors().forEach(x => x.openMe());
	}
	
	public void closeDoors()
	{
		_instance.getDoors().forEach(x => x.closeMe());
	}
	
	public void spawnBuffers()
	{
		_buffers.forEach(spawn => spawn.doSpawn(false));
	}
	
	public void deleteBuffers()
	{
		_buffers.Select(s => s.getLastSpawn()).Where(o => o is not null).forEach(o => o.deleteMe());
	}
	
	public void broadcastStatusUpdate(Player player)
	{
		ExOlympiadUserInfoPacket packet = new ExOlympiadUserInfoPacket(player);
		foreach (Player target in _instance.getPlayers())
		{
			if (target.inObserverMode() || (target.getOlympiadSide() != player.getOlympiadSide()))
			{
				target.sendPacket(packet);
			}
		}
	}
	
	public void broadcastPacket<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		_instance.broadcastPacket().SendPackets(packet);
	}
	
	public void broadcastPacketToObservers<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		foreach (Player target in _instance.getPlayers())
		{
			if (target.inObserverMode())
			{
				target.sendPacket(packet);
			}
		}
	}
	
	public void updateZoneStatusForCharactersInside()
	{
		if (_task == null)
		{
			return;
		}
		
		bool battleStarted = _task.isBattleStarted();
		SystemMessagePacket sm;
		if (battleStarted)
		{
			sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ENTERED_A_COMBAT_ZONE);
		}
		else
		{
			sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_LEFT_A_COMBAT_ZONE);
		}
		
		foreach (Player player in _instance.getPlayers())
		{
			if (player.inObserverMode())
			{
				return;
			}
			
			if (battleStarted)
			{
				player.setInsideZone(ZoneId.PVP, true);
				player.sendPacket(sm);
			}
			else
			{
				player.setInsideZone(ZoneId.PVP, false);
				player.sendPacket(sm);
				player.sendPacket(new ExOlympiadMatchEndPacket());
			}
		}
	}
	
	public void updateZoneInfoForObservers()
	{
		if (_task == null)
		{
			return;
		}
		
		foreach (Player player in _instance.getPlayers())
		{
			if (!player.inObserverMode())
			{
				return;
			}

			ImmutableArray<Location3D> spectatorSpawns = getZone().getSpectatorSpawns();
			if (spectatorSpawns.Length == 0)
			{
				LOGGER.Warn(GetType().Name + ": Zone: " + getZone() + " doesn't have specatator spawns defined!");
				return;
			}

			Location3D loc = spectatorSpawns[Rnd.get(spectatorSpawns.Length)];
			player.enterOlympiadObserverMode(new Location(loc.X, loc.Y, loc.Z, 0), _stadiumId);

			_task.getGame().sendOlympiadInfo(player);
		}
	}
}