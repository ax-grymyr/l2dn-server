using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("TargetMyParty")]
public sealed class TargetMyPartySkillCondition: ISkillCondition
{
    private readonly bool _includeMe;

    public TargetMyPartySkillCondition(SkillConditionParameterSet parameters)
    {
        _includeMe = parameters.GetBoolean(XmlSkillConditionParameterType.IncludeMe);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (target == null || !target.isPlayer())
        {
            return false;
        }

        Party? party = caster.getParty();
        Party? targetParty = target.getActingPlayer()?.getParty();
        return party == null
            ? _includeMe && caster == target
            : _includeMe ? party == targetParty : party == targetParty && caster != target;
    }

    public override int GetHashCode() => HashCode.Combine(_includeMe);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._includeMe);
}