using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Mp Consume Per Level effect implementation.
/// </summary>
public sealed class MpConsumePerLevel: AbstractEffect
{
    private readonly double _power;

    public MpConsumePerLevel(StatSet @params)
    {
        _power = @params.getDouble("power", 0);
        Ticks = @params.getInt("ticks");
    }

    public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return false;

        double @base = _power * TicksMultiplier;
        TimeSpan? abnormalTime = skill.getAbnormalTime();
        double consume = abnormalTime > TimeSpan.Zero
            ? (effected.getLevel() - 1) / 7.5 * @base * abnormalTime.Value.TotalSeconds
            : @base;

        if (consume > effected.getCurrentMp())
        {
            effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
            return false;
        }

        effected.reduceCurrentMp(consume);
        return skill.isToggle();
    }

    public override int GetHashCode() => HashCode.Combine(_power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x.Ticks));
}