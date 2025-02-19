using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class OpCheckClassSkillCondition: ISkillCondition
{
    private readonly CharacterClass _classId;
    private readonly SkillConditionAffectType _affectType;
    private readonly bool _isWithin;

    public OpCheckClassSkillCondition(StatSet @params)
    {
        _classId = @params.getEnum<CharacterClass>("classId");
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
}