using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A simple no restart zone
 * @author GKR
 */
public class NoRestartZone: ZoneType
{
	private TimeSpan _restartAllowedTime;
	private TimeSpan _restartTime;
	
	public NoRestartZone(int id):base(id)
	{
		
	}
	
	public override void setParameter(String name, String value)
	{
		if (name.equalsIgnoreCase("default_enabled"))
		{
			_enabled = bool.Parse(value);
		}
		else if (name.equalsIgnoreCase("restartAllowedTime"))
		{
			_restartAllowedTime = TimeSpan.FromSeconds(int.Parse(value));
		}
		else if (name.equalsIgnoreCase("restartTime"))
		{
			_restartTime = TimeSpan.FromSeconds(int.Parse(value));
		}
		else if (name.equalsIgnoreCase("instanceId"))
		{
			// Do nothing.
		}
		else
		{
			base.setParameter(name, value);
		}
	}
	
	protected override void onEnter(Creature creature)
	{
		if (!_enabled)
		{
			return;
		}
		
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.NO_RESTART, true);
		}
	}
	
	protected override void onExit(Creature creature)
	{
		if (!_enabled)
		{
			return;
		}
		
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.NO_RESTART, false);
		}
	}
	
	public override void onPlayerLoginInside(Player player)
	{
		if (!_enabled)
		{
			return;
		}
		
		if (DateTime.UtcNow - player.getLastAccess() > _restartTime && 
		    DateTime.UtcNow - ServerInfo.ServerStarted > _restartAllowedTime)
		{
			player.teleToLocation(TeleportWhereType.TOWN);
		}
	}
	
	public TimeSpan getRestartAllowedTime()
	{
		return _restartAllowedTime;
	}
	
	public void setRestartAllowedTime(TimeSpan time)
	{
		_restartAllowedTime = time;
	}
	
	public TimeSpan getRestartTime()
	{
		return _restartTime;
	}
	
	public void setRestartTime(TimeSpan time)
	{
		_restartTime = time;
	}
}