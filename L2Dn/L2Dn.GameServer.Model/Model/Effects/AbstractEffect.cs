using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using NLog;

namespace L2Dn.GameServer.Model.Effects;

/**
 * Abstract effect implementation.<br>
 * Instant effects should not override {@link #onExit(Creature, Creature, Skill)}.<br>
 * Instant effects should not override {@link #canStart(Creature, Creature, Skill)}, all checks should be done {@link #onStart(Creature, Creature, Skill, Item)}.<br>
 * Do not call super class methods {@link #onStart(Creature, Creature, Skill, Item)} nor {@link #onExit(Creature, Creature, Skill)}.
 * @author Zoey76
 */
public abstract class AbstractEffect
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(AbstractEffect));
	
	private int _ticks;
	
	/**
	 * Gets the effect ticks
	 * @return the ticks
	 */
	public virtual int getTicks()
	{
		return _ticks;
	}
	
	/**
	 * Sets the effect ticks
	 * @param ticks the ticks
	 */
	protected void setTicks(int ticks)
	{
		_ticks = ticks;
	}
	
	public double getTicksMultiplier()
	{
		return (getTicks() * Config.EFFECT_TICK_RATIO) / 1000f;
	}
	
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
	
	public virtual void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
	}
	
	public virtual void continuousInstant(Creature effector, Creature effected, Skill skill, Item item)
	{
	}
	
	public virtual void onStart(Creature effector, Creature effected, Skill skill, Item item)
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
	public virtual bool onActionTime(Creature effector, Creature effected, Skill skill, Item item)
	{
		return false;
	}
	
	/**
	 * Get the effect flags.
	 * @return bit flag for current effect
	 */
	public virtual long getEffectFlags()
	{
		return 1L << (int)EffectFlag.NONE;
	}
	
	public virtual bool checkCondition(int id)
	{
		return true;
	}
	
	/**
	 * Verify if this effect is an instant effect.
	 * @return {@code true} if this effect is instant, {@code false} otherwise
	 */
	public virtual bool isInstant()
	{
		return false;
	}
	
	/**
	 * @param effector
	 * @param effected
	 * @param skill
	 * @return {@code true} if pump can be invoked, {@code false} otherwise
	 */
	public virtual bool canPump(Creature effector, Creature effected, Skill skill)
	{
		return true;
	}
	
	/**
	 * @param effected
	 * @param skill
	 */
	public virtual void pump(Creature effected, Skill skill)
	{
	}
	
	/**
	 * Get this effect's type.<br>
	 * TODO: Remove.
	 * @return the effect type
	 */
	public virtual EffectType getEffectType()
	{
		return EffectType.NONE;
	}
	
	public override string ToString()
	{
		return "Effect " + GetType().Name;
	}
}