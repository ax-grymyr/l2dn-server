using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpCheckCrtEffectSkillCondition: ISkillCondition
{
    public OpCheckCrtEffectSkillCondition(StatSet @params)
    {
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return target != null && target.isNpc() && ((Npc)target).getTemplate().canBeCrt();
    }
}