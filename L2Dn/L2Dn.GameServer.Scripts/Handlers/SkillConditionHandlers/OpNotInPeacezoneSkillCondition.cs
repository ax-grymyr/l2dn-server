using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Liamxroy
 */
public class OpNotInPeacezoneSkillCondition: ISkillCondition
{
    public OpNotInPeacezoneSkillCondition(StatSet @params)
    {
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return !caster.isInsideZone(ZoneId.PEACE);
    }
}