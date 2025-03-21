using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("HasVitalityPoints")]
public sealed class HasVitalityPointsSkillCondition: ISkillCondition
{
    private readonly int _amount;

    public HasVitalityPointsSkillCondition(SkillConditionParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount, 1);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        return caster.isPlayer() && player != null && player.getVitalityPoints() >= _amount;
    }
}