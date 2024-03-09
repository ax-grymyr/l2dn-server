using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.TargetHandlers;

/**
 * Target fortress flagpole
 * @author Nik
 */
public class FortressFlagpole: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.FORTRESS_FLAGPOLE;
	}
	
	public WorldObject getTarget(Creature creature, WorldObject selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		WorldObject target = creature.getTarget();
		if ((target != null) && creature.isInsideZone(ZoneId.HQ) && creature.isInsideZone(ZoneId.FORT) && !target.isPlayable() && target.getName().toLowerCase().contains("flagpole"))
		{
			return target;
		}
		
		if (sendMessage)
		{
			creature.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
		}
		
		return null;
	}
}