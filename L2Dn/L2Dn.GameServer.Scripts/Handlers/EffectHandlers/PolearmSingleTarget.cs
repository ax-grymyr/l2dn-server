using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PolearmSingleTarget: AbstractEffect
{
    public PolearmSingleTarget(StatSet @params)
    {
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isPlayer())
            effected.getStat().addFixedValue(Stat.PHYSICAL_POLEARM_TARGET_SINGLE, 1.0);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        if (effected.isPlayer())
            effected.getStat().removeFixedValue(Stat.PHYSICAL_POLEARM_TARGET_SINGLE);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}