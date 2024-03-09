using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.TargetHandlers;

/**
 * Target Outpost npc (36590).
 * @author Nik
 */
public class AdvanceBase: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.ADVANCE_BASE;
	}
	
	public WorldObject getTarget(Creature creature, WorldObject selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		WorldObject target = creature.getTarget();
		if ((target != null) && target.isNpc() && (target.getId() == 36590) && !((Npc) target).isDead())
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