using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Mobius
 */
public class OpTargetDoorSkillCondition: ISkillCondition
{
    private readonly Set<int> _doorIds = [];

    public OpTargetDoorSkillCondition(StatSet @params)
    {
        _doorIds.addAll(@params.getList<int>("doorIds") ?? []);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return target != null && target.isDoor() && _doorIds.Contains(target.getId());
    }
}