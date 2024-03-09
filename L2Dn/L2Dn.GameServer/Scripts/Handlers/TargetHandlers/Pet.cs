using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;

namespace L2Dn.GameServer.Handlers.TargetHandlers;

/**
 * @author Mobius
 */
public class Pet: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.PET;
	}
	
	public WorldObject getTarget(Creature creature, WorldObject selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		if (creature.isPet())
		{
			return creature;
		}
		if (creature.hasPet())
		{
			return creature.getPet();
		}
		return null;
	}
}