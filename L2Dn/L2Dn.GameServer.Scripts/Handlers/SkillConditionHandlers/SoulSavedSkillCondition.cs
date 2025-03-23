using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("SoulSaved")]
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

    public override int GetHashCode() => HashCode.Combine(_type, _amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._type, x._amount));
}