using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class CheckLevelSkillCondition: ISkillCondition
{
    private readonly int _minLevel;
    private readonly int _maxLevel;
    private readonly SkillConditionAffectType _affectType;

    public CheckLevelSkillCondition(StatSet @params)
    {
        _minLevel = @params.getInt("minLevel", 1);
        _maxLevel = @params.getInt("maxLevel", int.MaxValue);
        _affectType = @params.getEnum("affectType", SkillConditionAffectType.CASTER);
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