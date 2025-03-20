using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SkillMastery: AbstractEffect
{
    private readonly double _stat;

    public SkillMastery(EffectParameterSet parameters)
    {
        _stat = (int)parameters.GetEnum(XmlSkillEffectParameterType.Stat, BaseStat.STR);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeAdd(Stat.SKILL_MASTERY, _stat); // TODO: why stat is added?
    }

    public override int GetHashCode() => HashCode.Combine(_stat);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._stat);
}