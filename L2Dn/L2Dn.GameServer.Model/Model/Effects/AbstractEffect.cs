using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using NLog;

namespace L2Dn.GameServer.Model.Effects;

/// <summary>
/// Abstract effect implementation.
/// Instant effects should not override <see cref="onExit"/>.
/// Instant effects should not override <see cref="canStart"/>, all checks should be done <see cref="onStart"/>.
/// Do not call super class methods <see cref="onStart"/> nor <see cref="onExit"/>.
/// </summary>
public abstract class AbstractEffect
{
    protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(AbstractEffect));

    /// <summary>
    /// Gets the effect ticks.
    /// </summary>
    public int Ticks { get; protected init; }

    protected double TicksMultiplier => Ticks * (Config.EFFECT_TICK_RATIO / 1000.0);

    /**
     * Calculates whether this effects land or not.<br>
     * If it lands will be scheduled and added to the character effect list.<br>
     * Override in effect implementation to change behavior.<br>
     * <b>Warning:</b> Must be used only for instant effects continuous effects will not call this they have their success handled by activate_rate.
     * @param effector
     * @param effected
     * @param skill
     * @return {@code true} if this effect land, {@code false} otherwise
     */
    public virtual bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return true;
    }

    /**
     * Verify if the buff can start.<br>
     * Used for continuous effects.
     * @param effector
     * @param effected
     * @param skill
     * @return {@code true} if all the start conditions are meet, {@code false} otherwise
     */
    public virtual bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return true;
    }

    public virtual void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
    }

    public virtual void continuousInstant(Creature effector, Creature effected, Skill skill, Item? item)
    {
    }

    public virtual void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
    }

    public virtual void onExit(Creature effector, Creature effected, Skill skill)
    {
    }

    /**
     * Called on each tick.<br>
     * If the abnormal time is lesser than zero it will last forever.
     * @param effector
     * @param effected
     * @param skill
     * @param item
     * @return if {@code true} this effect will continue forever, if {@code false} it will stop after abnormal time has passed
     */
    public virtual bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        return false;
    }

    /// <summary>
    /// Get the effect flags.
    /// </summary>
    /// <returns>bit flag for current effect</returns>
    public virtual long getEffectFlags() => 1L << (int)EffectFlag.NONE;

    public virtual bool checkCondition(int id)
    {
        return true;
    }

    /// <summary>
    /// Verify if this effect is an instant effect.
    /// </summary>
    /// <returns>true if this effect is instant, false otherwise</returns>
    public virtual bool isInstant() => false;

    /**
     * @param effector
     * @param effected
     * @param skill
     * @return {@code true} if pump can be invoked, {@code false} otherwise
     */
    public virtual bool canPump(Creature? effector, Creature effected, Skill? skill)
    {
        return true;
    }

    public virtual void pump(Creature effected, Skill skill)
    {
    }

    /**
     * Get this effect's type.
     * TODO: Remove.
     * @return the effect type
     */
    public virtual EffectType getEffectType() => EffectType.NONE;

    public override string ToString() => "Effect " + GetType().Name;
    public override int GetHashCode() => GetType().Name.GetHashCode();
}