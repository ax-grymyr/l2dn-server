using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A PVP Zone
 * @author durgus
 */
public class ArenaZone : ZoneType
{
	public ArenaZone(int id): base(id)
	{
	}
	
	protected override void onEnter(Creature creature)
	{
		if (creature.isPlayer() && !creature.isInsideZone(ZoneId.PVP))
		{
			creature.sendPacket(SystemMessageId.YOU_HAVE_ENTERED_A_COMBAT_ZONE);
		}
		creature.setInsideZone(ZoneId.PVP, true);
	}
	
	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.PVP, false);
		if (creature.isPlayer() && !creature.isInsideZone(ZoneId.PVP))
		{
			creature.sendPacket(SystemMessageId.YOU_HAVE_LEFT_A_COMBAT_ZONE);
		}
	}
}