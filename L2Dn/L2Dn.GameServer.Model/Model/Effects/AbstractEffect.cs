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
/// Instant effects should not override <see cref="OnExit"/>.
/// Instant effects should not override <see cref="CanStart"/>, all checks should be done <see cref="OnStart"/>.
/// Do not call super class methods <see cref="OnStart"/> nor <see cref="OnExit"/>.
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

    /// <summary>
    /// Verify if this effect is an instant effect.
    /// </summary>
    /// <value>true if this effect is instant, false otherwise</value>
    public virtual bool IsInstant => false;

    /// <summary>
    /// Calculates whether this effects land or not. If it lands will be scheduled and added to the character
    /// effect list. Override in effect implementation to change behavior.
    /// <b>Warning:</b> Must be used only for instant effects continuous effects will not call this they have
    /// their success handled by activate_rate.
    /// </summary>
    /// <param name="effector"></param>
    /// <param name="effected"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public virtual bool CalcSuccess(Creature effector, Creature effected, Skill skill) => true;

    /// <summary>
    /// Verify if the buff can start. Used for continuous effects.
    /// </summary>
    /// <param name="effector"></param>
    /// <param name="effected"></param>
    /// <param name="skill"></param>
    /// <returns>True if all the start conditions are meet, false otherwise.</returns>
    public virtual bool CanStart(Creature effector, Creature effected, Skill skill) => true;

    public virtual void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
    }

    public virtual void ContinuousInstant(Creature effector, Creature effected, Skill skill, Item? item)
    {
    }

    public virtual void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
    }

    public virtual void OnExit(Creature effector, Creature effected, Skill skill)
    {
    }

    /// <summary>
    /// Called on each tick. If the abnormal time is lesser than zero it will last forever.
    /// </summary>
    /// <param name="effector"></param>
    /// <param name="effected"></param>
    /// <param name="skill"></param>
    /// <param name="item"></param>
    /// <returns>If true this effect will continue forever, if false it will stop after abnormal time has passed.
    /// </returns>
    public virtual bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item) => false;

    public virtual bool CheckCondition(int id) => true;

    /// <summary>
    /// Returns true if <see cref="Pump"/> can be invoked, false otherwise.
    /// </summary>
    public virtual bool CanPump(Creature? effector, Creature effected, Skill? skill) => true;

    public virtual void Pump(Creature effected, Skill skill)
    {
    }

    public override string ToString() => "Effect " + GetType().Name;
    public override int GetHashCode() => this.GetSingletonHashCode();
}