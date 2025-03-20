using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpTargetNpcSkillCondition: ISkillCondition
{
    private readonly Set<int> _npcIds = new();

    public OpTargetNpcSkillCondition(StatSet @params)
    {
        _npcIds.addAll(@params.getList<int>("npcIds") ?? []);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        WorldObject? actualTarget = caster == null || !caster.isPlayer() ? target : caster.getTarget();
        return actualTarget != null && (actualTarget.isNpc() || actualTarget.isDoor()) &&
            _npcIds.Contains(actualTarget.Id);
    }
}