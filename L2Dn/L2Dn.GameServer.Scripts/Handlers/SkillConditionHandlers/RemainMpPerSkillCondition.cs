using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class RemainMpPerSkillCondition: ISkillCondition
{
    private readonly int _amount;
    private readonly SkillConditionPercentType _percentType;

    public RemainMpPerSkillCondition(StatSet @params)
    {
        _amount = @params.getInt("amount");
        _percentType = @params.getEnum<SkillConditionPercentType>("percentType");
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return _percentType.test(caster.getCurrentMpPercent(), _amount);
    }
}