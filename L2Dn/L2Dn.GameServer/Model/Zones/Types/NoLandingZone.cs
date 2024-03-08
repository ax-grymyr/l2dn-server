using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A no landing zone
 * @author durgus
 */
public class NoLandingZone : ZoneType
{
	private int dismountDelay = 5;
	
	public NoLandingZone(int id):base(id)
	{
	}
	
	public override void setParameter(String name, String value)
	{
		if (name.equals("dismountDelay"))
		{
			dismountDelay = int.Parse(value);
		}
		else
		{
			base.setParameter(name, value);
		}
	}
	
	protected override void onEnter(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.NO_LANDING, true);
			if (creature.getActingPlayer().getMountType() == MountType.WYVERN)
			{
				creature.sendPacket(SystemMessageId.THIS_AREA_CANNOT_BE_ENTERED_WHILE_MOUNTED_ATOP_OF_A_WYVERN_YOU_WILL_BE_DISMOUNTED_FROM_YOUR_WYVERN_IF_YOU_DO_NOT_LEAVE);
				creature.getActingPlayer().enteredNoLanding(dismountDelay);
			}
		}
	}
	
	protected override void onExit(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.NO_LANDING, false);
			if (creature.getActingPlayer().getMountType() == MountType.WYVERN)
			{
				creature.getActingPlayer().exitedNoLanding();
			}
		}
	}
}