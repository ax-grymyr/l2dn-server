using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("ReduceDropPenalty")]
public sealed class ReduceDropPenalty: AbstractEffect
{
    private readonly double _exp;
    private readonly double _deathPenalty;
    private readonly ReduceDropType _type;

    public ReduceDropPenalty(EffectParameterSet parameters)
    {
        _exp = parameters.GetDouble(XmlSkillEffectParameterType.Exp, 0);
        _deathPenalty = parameters.GetDouble(XmlSkillEffectParameterType.DeathPenalty, 0);
        _type = parameters.GetEnum(XmlSkillEffectParameterType.Type, ReduceDropType.MOB);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        switch (_type)
        {
            case ReduceDropType.MOB:
            {
                effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_MOB, _exp / 100 + 1);
                effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_MOB, _deathPenalty / 100 + 1);
                break;
            }
            case ReduceDropType.PK:
            {
                effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_PVP, _exp / 100 + 1);
                effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_PVP, _deathPenalty / 100 + 1);
                break;
            }
            case ReduceDropType.RAID:
            {
                effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_RAID, _exp / 100 + 1);
                effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_RAID, _deathPenalty / 100 + 1);
                break;
            }
            case ReduceDropType.ANY:
            {
                effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_MOB, _exp / 100 + 1);
                effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_MOB, _deathPenalty / 100 + 1);
                effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_PVP, _exp / 100 + 1);
                effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_PVP, _deathPenalty / 100 + 1);
                effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_RAID, _exp / 100 + 1);
                effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_RAID, _deathPenalty / 100 + 1);
                break;
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_exp, _deathPenalty, _type);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._exp, x._deathPenalty, x._type));
}