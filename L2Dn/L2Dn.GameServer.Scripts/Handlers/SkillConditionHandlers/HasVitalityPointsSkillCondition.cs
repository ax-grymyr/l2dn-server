using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Mode
 */
public class HasVitalityPointsSkillCondition: ISkillCondition
{
    private readonly int _amount;

    public HasVitalityPointsSkillCondition(StatSet @params)
    {
        _amount = @params.getInt("amount", 1);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        return caster.isPlayer() && player != null && player.getVitalityPoints() >= _amount;
    }
}