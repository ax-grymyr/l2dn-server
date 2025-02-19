using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid, Mobius
 */
public class OpCheckClassListSkillCondition: ISkillCondition
{
    private readonly Set<CharacterClass> _classIds = new();
    private readonly SkillConditionAffectType _affectType;
    private readonly bool _isWithin;

    public OpCheckClassListSkillCondition(StatSet @params)
    {
        List<CharacterClass> classIds = @params.getEnumList<CharacterClass>("classIds");
        if (classIds != null)
        {
            _classIds.addAll(classIds);
        }

        _affectType = @params.getEnum<SkillConditionAffectType>("affectType");
        _isWithin = @params.getBoolean("isWithin");
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