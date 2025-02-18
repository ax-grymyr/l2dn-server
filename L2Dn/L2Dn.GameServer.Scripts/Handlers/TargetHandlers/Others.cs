using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * Target other things (skills with this target type appear to be disabled).
 * @author Nik
 */
public class Others: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.OTHERS;
	}

	public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		if (selectedTarget == creature)
		{
			creature.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_ON_YOURSELF);
			return null;
		}
		return selectedTarget;
	}
}