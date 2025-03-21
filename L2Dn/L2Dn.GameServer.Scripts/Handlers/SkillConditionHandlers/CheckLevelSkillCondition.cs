using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("CheckLevel")]
public sealed class CheckLevelSkillCondition: ISkillCondition
{
    private readonly int _minLevel;
    private readonly int _maxLevel;
    private readonly SkillConditionAffectType _affectType;

    public CheckLevelSkillCondition(SkillConditionParameterSet parameters)
    {
        _minLevel = parameters.GetInt32(XmlSkillConditionParameterType.MinLevel, 1);
        _maxLevel = parameters.GetInt32(XmlSkillConditionParameterType.MaxLevel, int.MaxValue);
        _affectType = parameters.GetEnum(XmlSkillConditionParameterType.AffectType, SkillConditionAffectType.CASTER);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                return caster.getLevel() >= _minLevel && caster.getLevel() <= _maxLevel;
            }
            case SkillConditionAffectType.TARGET:
            {
                Player? player = target?.getActingPlayer();
                if (target != null && target.isPlayer() && player != null)
                {
                    return player.getLevel() >= _minLevel && player.getLevel() <= _maxLevel;
                }

                break;
            }
        }

        return false;
    }
}