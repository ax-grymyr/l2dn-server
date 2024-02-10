using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A simple no restart zone
 * @author GKR
 */
public class NoRestartZone : ZoneType
{
	private int _restartAllowedTime = 0;
	private int _restartTime = 0;
	
	public NoRestartZone(int id):base(id)
	{
		
	}
	
	public void setParameter(String name, String value)
	{
		if (name.equalsIgnoreCase("default_enabled"))
		{
			_enabled = bool.Parse(value);
		}
		else if (name.equalsIgnoreCase("restartAllowedTime"))
		{
			_restartAllowedTime = int.Parse(value) * 1000;
		}
		else if (name.equalsIgnoreCase("restartTime"))
		{
			_restartTime = int.Parse(value) * 1000;
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
		
		if (((System.currentTimeMillis() - player.getLastAccess()) > _restartTime) && ((System.currentTimeMillis() - GameServer.dateTimeServerStarted.getTimeInMillis()) > _restartAllowedTime))
		{
			player.teleToLocation(TeleportWhereType.TOWN);
		}
	}
	
	public int getRestartAllowedTime()
	{
		return _restartAllowedTime;
	}
	
	public void setRestartAllowedTime(int time)
	{
		_restartAllowedTime = time;
	}
	
	public int getRestartTime()
	{
		return _restartTime;
	}
	
	public void setRestartTime(int time)
	{
		_restartTime = time;
	}
}