using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class SoulSavedSkillCondition: ISkillCondition
{
    private readonly SoulType _type;
    private readonly int _amount;

    public SoulSavedSkillCondition(SkillConditionParameterSet parameters)
    {
        _type = parameters.GetEnum(XmlSkillConditionParameterType.Type, SoulType.LIGHT);
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return caster.isPlayer() && caster.getActingPlayer()?.getChargedSouls(_type) >= _amount;
    }
}