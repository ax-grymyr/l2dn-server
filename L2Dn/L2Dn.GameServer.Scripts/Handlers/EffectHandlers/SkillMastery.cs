using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SkillMastery: AbstractEffect
{
    private readonly double _stat;

    public SkillMastery(StatSet @params)
    {
        _stat = (int)@params.getEnum("stat", BaseStat.STR);
    }

    public override void pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeAdd(Stat.SKILL_MASTERY, _stat);
    }

    public override int GetHashCode() => HashCode.Combine(_stat);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._stat);
}