using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A no landing zone
 * @author durgus
 */
public class NoLandingZone(int id, ZoneForm form): ZoneType(id, form)
{
	private int _dismountDelay = 5;

    public override void setParameter(string name, string value)
	{
		if (name.equals("dismountDelay"))
		{
			_dismountDelay = int.Parse(value);
		}
		else
		{
			base.setParameter(name, value);
		}
	}

	protected override void onEnter(Creature creature)
	{
        Player? player = creature.getActingPlayer();
        if (creature.isPlayer() && player != null)
		{
			creature.setInsideZone(ZoneId.NO_LANDING, true);
			if (player.getMountType() == MountType.WYVERN)
			{
				creature.sendPacket(SystemMessageId.THIS_AREA_CANNOT_BE_ENTERED_WHILE_MOUNTED_ATOP_OF_A_WYVERN_YOU_WILL_BE_DISMOUNTED_FROM_YOUR_WYVERN_IF_YOU_DO_NOT_LEAVE);
                player.enteredNoLanding(_dismountDelay);
			}
		}
	}

	protected override void onExit(Creature creature)
	{
        Player? player = creature.getActingPlayer();
        if (creature.isPlayer() && player != null)
		{
			creature.setInsideZone(ZoneId.NO_LANDING, false);
			if (player.getMountType() == MountType.WYVERN)
			{
                player.exitedNoLanding();
			}
		}
	}
}