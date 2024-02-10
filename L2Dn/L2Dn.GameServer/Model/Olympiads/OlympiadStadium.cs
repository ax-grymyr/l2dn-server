using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Utilities;
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
	
	protected OlympiadStadium(OlympiadStadiumZone olyzone, int stadiumId)
	{
		_zone = olyzone;
		_stadiumId = stadiumId;
		_instance = InstanceManager.getInstance().createInstance(olyzone.getInstanceTemplateId(), null);
		_buffers = _instance.getNpcs().Select(n => n.Key.getSpawn()).ToList();
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
		ExOlympiadUserInfo packet = new ExOlympiadUserInfo(player);
		foreach (Player target in _instance.getPlayers().Keys)
		{
			if (target.inObserverMode() || (target.getOlympiadSide() != player.getOlympiadSide()))
			{
				target.sendPacket(packet);
			}
		}
	}
	
	public void broadcastPacket(ServerPacket packet)
	{
		_instance.broadcastPacket(packet);
	}
	
	public void broadcastPacketToObservers(ServerPacket packet)
	{
		foreach (Player target in _instance.getPlayers().Keys)
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
		SystemMessage sm;
		if (battleStarted)
		{
			sm = new SystemMessage(SystemMessageId.YOU_HAVE_ENTERED_A_COMBAT_ZONE);
		}
		else
		{
			sm = new SystemMessage(SystemMessageId.YOU_HAVE_LEFT_A_COMBAT_ZONE);
		}
		
		foreach (Player player in _instance.getPlayers().Keys)
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
				player.sendPacket(ExOlympiadMatchEnd.STATIC_PACKET);
			}
		}
	}
	
	public void updateZoneInfoForObservers()
	{
		if (_task == null)
		{
			return;
		}
		
		foreach (Player player in _instance.getPlayers().Keys)
		{
			if (!player.inObserverMode())
			{
				return;
			}
			
			List<Location> spectatorSpawns = getZone().getSpectatorSpawns();
			if (spectatorSpawns.isEmpty())
			{
				LOGGER.Warn(GetType().Name + ": Zone: " + getZone() + " doesn't have specatator spawns defined!");
				return;
			}
			
			Location loc = spectatorSpawns.get(Rnd.get(spectatorSpawns.size()));
			player.enterOlympiadObserverMode(loc, _stadiumId);
			
			_task.getGame().sendOlympiadInfo(player);
		}
	}
}