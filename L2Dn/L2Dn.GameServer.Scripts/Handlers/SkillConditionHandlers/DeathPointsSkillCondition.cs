using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("DeathPoints")]
public sealed class DeathPointsSkillCondition: ISkillCondition
{
    private readonly int _amount;
    private readonly bool _less;

    public DeathPointsSkillCondition(SkillConditionParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount);
        _less = parameters.GetBoolean(XmlSkillConditionParameterType.Less, false);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (_less)
        {
            return caster.getActingPlayer()?.getDeathPoints() <= _amount;
        }

        return caster.getActingPlayer()?.getDeathPoints() >= _amount;
    }
}