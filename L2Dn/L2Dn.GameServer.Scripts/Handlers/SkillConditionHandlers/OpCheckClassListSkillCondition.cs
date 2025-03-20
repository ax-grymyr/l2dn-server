using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class OpCheckClassListSkillCondition: ISkillCondition
{
    private readonly Set<CharacterClass> _classIds = new();
    private readonly SkillConditionAffectType _affectType;
    private readonly bool _isWithin;

    public OpCheckClassListSkillCondition(SkillConditionParameterSet parameters)
    {
        List<string>? classIds = parameters.GetStringListOptional(XmlSkillConditionParameterType.ClassIds);
        if (classIds != null)
        {
            foreach (string classId in classIds)
                _classIds.Add(Enum.Parse<CharacterClass>(classId, true));
        }

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
}