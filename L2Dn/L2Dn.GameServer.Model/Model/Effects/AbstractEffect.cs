using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;
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
    protected static readonly Logger Logger = LogManager.GetLogger(nameof(AbstractEffect));

    /// <summary>
    /// Gets the effect ticks.
    /// </summary>
    public int Ticks { get; protected init; }

    protected double TicksMultiplier => Ticks * (Config.Character.EFFECT_TICK_RATIO / 1000.0);

    /**
     * Get this effect's type.
     * TODO: Remove.
     * @return the effect type
     */
    public virtual EffectTypes EffectType => EffectTypes.NONE;

    /// <summary>
    /// Get the effect flags.
    /// </summary>
    /// <value>bit flag for current effect</value>
    public virtual EffectFlags EffectFlags => EffectFlags.NONE;

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
    public virtual bool canStart(Creature effector, Creature effected, Skill skill) => true;

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
    public virtual bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item) => false;

    public virtual bool checkCondition(int id) => true;

    /// <summary>
    /// Verify if this effect is an instant effect.
    /// </summary>
    /// <value>true if this effect is instant, false otherwise</value>
    public virtual bool IsInstant => false;

    /**
     * @param effector
     * @param effected
     * @param skill
     * @return {@code true} if pump can be invoked, {@code false} otherwise
     */
    public virtual bool canPump(Creature? effector, Creature effected, Skill? skill) => true;

    public virtual void pump(Creature effected, Skill skill)
    {
    }

    public override string ToString() => "Effect " + GetType().Name;
    public override int GetHashCode() => this.GetSingletonHashCode();
}