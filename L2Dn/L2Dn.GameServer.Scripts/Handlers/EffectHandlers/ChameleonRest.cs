using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Chameleon Rest effect implementation.
/// </summary>
public sealed class ChameleonRest: AbstractEffect
{
    private readonly double _power;

    public ChameleonRest(StatSet @params)
    {
        _power = @params.getDouble("power", 0);
        Ticks = @params.getInt("ticks");
    }

    public override EffectFlags getEffectFlags() => EffectFlags.SILENT_MOVE | EffectFlags.RELAXING;

    public override EffectTypes EffectType => EffectTypes.RELAXING;

    public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return false;

        Player? player = effected.getActingPlayer();
        if (effected.isPlayer() && player != null && !player.isSitting())
            return false;

        double manaDam = _power * TicksMultiplier;
        if (manaDam > effected.getCurrentMp())
        {
            effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
            return false;
        }

        effected.reduceCurrentMp(manaDam);
        return skill.IsToggle;
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (effected.isPlayer() && player != null)
            player.sitDown(false);
        else
            effected.getAI().setIntention(CtrlIntention.AI_INTENTION_REST);
    }

    public override int GetHashCode() => HashCode.Combine(_power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x.Ticks));
}