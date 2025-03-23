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

[HandlerStringKey("OpCheckSkillList")]
public sealed class OpCheckSkillListSkillCondition: ISkillCondition
{
    private readonly FrozenSet<int> _skillIds;
    private readonly SkillConditionAffectType _affectType;

    public OpCheckSkillListSkillCondition(SkillConditionParameterSet parameters)
    {
        List<int>? skillIds = parameters.GetInt32ListOptional(XmlSkillConditionParameterType.SkillIds);
        _skillIds = skillIds is null ? FrozenSet<int>.Empty : skillIds.ToFrozenSet();
        _affectType = parameters.GetEnum<SkillConditionAffectType>(XmlSkillConditionParameterType.AffectType);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                foreach (int id in _skillIds)
                {
                    if (caster.getSkillLevel(id) > 0)
                    {
                        return true;
                    }
                }

                break;
            }
            case SkillConditionAffectType.TARGET:
            {
                Player? player = target?.getActingPlayer();
                if (target != null && !target.isPlayer() && player != null)
                {
                    foreach (int id in _skillIds)
                    {
                        if (player.getSkillLevel(id) > 0)
                        {
                            return true;
                        }
                    }
                }

                break;
            }
        }

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(_skillIds.GetSetHashCode(), _affectType);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._skillIds.GetSetComparable(), x._affectType));
}