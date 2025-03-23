using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpCheckClass")]
public sealed class OpCheckClassSkillCondition: ISkillCondition
{
    private readonly CharacterClass _classId;
    private readonly SkillConditionAffectType _affectType;
    private readonly bool _isWithin;

    public OpCheckClassSkillCondition(SkillConditionParameterSet parameters)
    {
        _classId = parameters.GetEnum<CharacterClass>(XmlSkillConditionParameterType.ClassId);
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
                return caster.isPlayer() && player != null && _isWithin == (_classId == player.getClassId());
            }
            case SkillConditionAffectType.TARGET:
            {
                Player? player = target?.getActingPlayer();
                if (target != null && !target.isPlayer() && player != null)
                {
                    return _isWithin == (_classId == player.getClassId());
                }

                break;
            }
        }

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(_classId, _affectType, _isWithin);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._classId, x._affectType, x._isWithin));
}