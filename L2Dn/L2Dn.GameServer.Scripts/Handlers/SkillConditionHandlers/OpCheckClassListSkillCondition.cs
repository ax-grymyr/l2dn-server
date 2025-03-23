using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpCheckClassList")]
public sealed class OpCheckClassListSkillCondition: ISkillCondition
{
    private readonly FrozenSet<CharacterClass> _classIds;
    private readonly SkillConditionAffectType _affectType;
    private readonly bool _isWithin;

    public OpCheckClassListSkillCondition(SkillConditionParameterSet parameters)
    {
        List<string>? classIds = parameters.GetStringListOptional(XmlSkillConditionParameterType.ClassIds);
        _classIds = classIds is null
            ? FrozenSet<CharacterClass>.Empty
            : classIds.Select(classId => Enum.Parse<CharacterClass>(classId, true)).ToFrozenSet();

        _affectType = parameters.GetEnum<SkillConditionAffectType>(XmlSkillConditionParameterType.AffectType);
        _isWithin = parameters.GetBoolean(XmlSkillConditionParameterType.IsWithin);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                Player? player = caster.getActingPlayer();
                return caster.isPlayer() && player != null && _classIds.Contains(player.getClassId()) == _isWithin;
            }
            case SkillConditionAffectType.TARGET:
            {
                Player? player = target?.getActingPlayer();
                return target != null && target.isPlayer() && player != null &&
                    _classIds.Contains(player.getClassId()) == _isWithin;
            }
            default:
            {
                return false;
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_classIds.GetSetHashCode(), _affectType, _isWithin);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._classIds.GetSetComparable(), x._affectType, x._isWithin));
}