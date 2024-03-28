using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * An Undying Zone
 * @author UnAfraid
 */
public class UndyingZone : ZoneType
{
	public UndyingZone(int id): base(id)
	{
	}
	
	protected override void onEnter(Creature creature)
	{
		creature.setInsideZone(ZoneId.UNDYING, true);
	}
	
	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.UNDYING, false);
	}
}