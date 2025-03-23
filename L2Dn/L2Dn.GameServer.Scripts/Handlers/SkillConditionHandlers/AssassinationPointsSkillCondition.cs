using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("AssassinationPoints")]
public sealed class AssassinationPointsSkillCondition: ISkillCondition
{
    private readonly int _amount;

    public AssassinationPointsSkillCondition(SkillConditionParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount) * 10000;
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return caster.getActingPlayer()?.getAssassinationPoints() >= _amount;
    }

    public override int GetHashCode() => _amount;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}