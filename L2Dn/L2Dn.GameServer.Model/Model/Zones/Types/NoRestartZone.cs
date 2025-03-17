using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A simple no restart zone
 * @author GKR
 */
public class NoRestartZone(int id, ZoneForm form): Zone(id, form)
{
	private TimeSpan _restartAllowedTime;
	private TimeSpan _restartTime;

    public override void setParameter(XmlZoneStatName name, string value)
	{
		if (name == XmlZoneStatName.default_enabled)
		{
			_enabled = bool.Parse(value);
		}
		else if (name == XmlZoneStatName.restartAllowedTime)
		{
			_restartAllowedTime = TimeSpan.FromSeconds(int.Parse(value));
		}
		else if (name == XmlZoneStatName.restartTime)
		{
			_restartTime = TimeSpan.FromSeconds(int.Parse(value));
		}
		else if (name == XmlZoneStatName.instanceId)
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