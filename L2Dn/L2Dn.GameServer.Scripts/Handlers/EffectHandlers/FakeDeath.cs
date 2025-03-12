using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Fake Death effect implementation.
/// </summary>
public sealed class FakeDeath: AbstractEffect
{
    private readonly double _power;

    public FakeDeath(StatSet @params)
    {
        _power = @params.getDouble("power", 0);
        Ticks = @params.getInt("ticks");
    }

    public override long getEffectFlags() => EffectFlag.FAKE_DEATH.getMask();

    public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return false;

        double manaDam = _power * TicksMultiplier;
        if (manaDam > effected.getCurrentMp() && skill.isToggle())
        {
            effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
            return false;
        }

        effected.reduceCurrentMp(manaDam);

        return skill.isToggle();
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        if (effected.isPlayer())
            effected.getActingPlayer()?.setRecentFakeDeath(true);

        effected.broadcastPacket(new ChangeWaitTypePacket(effected, ChangeWaitTypePacket.WT_STOP_FAKEDEATH));
        effected.broadcastPacket(new RevivePacket(effected));
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.startFakeDeath();
    }

    public override int GetHashCode() => HashCode.Combine(_power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x.Ticks));
}