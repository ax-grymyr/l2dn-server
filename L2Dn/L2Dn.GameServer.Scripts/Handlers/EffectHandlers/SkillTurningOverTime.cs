using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SkillTurningOverTime: AbstractEffect
{
    private readonly int _chance;
    private readonly bool _staticChance;

    public SkillTurningOverTime(StatSet @params)
    {
        _chance = @params.getInt("chance", 100);
        _staticChance = @params.getBoolean("staticChance", false);
        Ticks = @params.getInt("ticks");
    }

    public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == null || effected == effector || effected.isRaid())
            return false;

        bool skillSuccess = _staticChance
            ? Formulas.calcProbability(_chance, effector, effected, skill)
            : Rnd.get(100) < _chance;

        if (skillSuccess && effected.isCastingNow())
        {
            effected.abortAllSkillCasters();
            effected.sendPacket(SystemMessageId.YOUR_CASTING_HAS_BEEN_INTERRUPTED);
        }

        return base.onActionTime(effector, effected, skill, item);
    }

    public override int GetHashCode() => HashCode.Combine(_chance, _staticChance, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._chance, x._staticChance, x.Ticks));
}