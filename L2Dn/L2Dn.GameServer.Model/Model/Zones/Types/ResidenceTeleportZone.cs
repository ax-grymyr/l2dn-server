using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * based on Kerberos work for custom CastleTeleportZone
 * @author Nyaran
 */
public class ResidenceTeleportZone : ZoneRespawn
{
	private int _residenceId;
	
	public ResidenceTeleportZone(int id): base(id)
	{
	}
	
	public override void setParameter(String name, String value)
	{
		if (name.equals("residenceId"))
		{
			_residenceId = int.Parse(value);
		}
		else
		{
			base.setParameter(name, value);
		}
	}
	
	protected override void onEnter(Creature creature)
	{
		creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, true); // FIXME: Custom ?
	}
	
	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, false); // FIXME: Custom ?
	}
	
	public override void oustAllPlayers()
	{
		foreach (Player player in getPlayersInside())
		{
			if ((player != null) && player.isOnline())
			{
				player.teleToLocation(new Location(getSpawnLoc(), 0), 200);
			}
		}
	}
	
	public int getResidenceId()
	{
		return _residenceId;
	}
}