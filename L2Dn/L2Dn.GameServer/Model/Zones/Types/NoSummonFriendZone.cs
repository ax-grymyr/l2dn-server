using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A simple no summon zone
 * @author JIV
 */
public class NoSummonFriendZone : ZoneType
{
	public NoSummonFriendZone(int id): base(id)
	{
	}
	
	protected override void onEnter(Creature creature)
	{
		creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, true);
	}
	
	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, false);
	}
}