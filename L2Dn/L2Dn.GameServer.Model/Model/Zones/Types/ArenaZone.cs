using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A PVP Zone
 * @author durgus
 */
public sealed class ArenaZone(int id, ZoneForm form): Zone(id, form)
{
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