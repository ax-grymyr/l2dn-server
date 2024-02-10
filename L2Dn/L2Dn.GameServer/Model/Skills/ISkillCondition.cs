using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Skills;

/**
 * @author NosBit
 */
public interface ISkillCondition
{
	bool canUse(Creature caster, Skill skill, WorldObject target);
}