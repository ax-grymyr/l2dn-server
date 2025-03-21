using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Skills;

/// <summary>
/// Buff Info.
/// Complex DTO that holds all the information for a given buff (or debuff or dance/song) set of effects issued by an skill.
/// </summary>
public class BuffInfo
{
	// Data
	/** Data. */
	private readonly int _effectorObjectId;
	private readonly Creature _effector;
	private readonly Creature _effected;
	private readonly Skill _skill;
	/** The effects. */
	private readonly List<AbstractEffect> _effects = new(1);
	// Tasks
	/** Effect tasks for ticks. */
	private Map<AbstractEffect, EffectTaskInfo>? _tasks;
	// Time and ticks
	/** Abnormal time. */
	private TimeSpan? _abnormalTime;
	/** The game ticks at the start of this effect. */
	private int _periodStartTicks;
	// Misc
	/** If {@code true} then this effect has been cancelled. */
	private volatile SkillFinishType _finishType = SkillFinishType.NORMAL;
	/** If {@code true} then this effect is in use (or has been stop because an Herb took place). */
	private volatile bool _isInUse = true;
	private readonly bool _hideStartMessage;
	private readonly Item? _item;
	private readonly Option? _option;

	/**
	 * Buff Info constructor.
	 * @param effector the effector
	 * @param effected the effected
	 * @param skill the skill
	 * @param hideStartMessage hides start message
	 * @param item
	 * @param option
	 */
	public BuffInfo(Creature effector, Creature effected, Skill skill, bool hideStartMessage, Item? item, Option? option)
	{
		_effectorObjectId = effector.ObjectId;
		_effector = effector;
		_effected = effected;
		_skill = skill;
		_abnormalTime = Formulas.calcEffectAbnormalTime(effector, effected, skill);
		_periodStartTicks = GameTimeTaskManager.getInstance().getGameTicks();
		_hideStartMessage = hideStartMessage;
		_item = item;
		_option = option;
	}

	/**
	 * Gets the effects on this buff info.
	 * @return the effects
	 */
	public List<AbstractEffect> getEffects()
	{
		return _effects;
	}

	/**
	 * Adds an effect to this buff info.
	 * @param effect the effect to add
	 */
	public void addEffect(AbstractEffect effect)
	{
		_effects.Add(effect);
	}

	/**
	 * Adds an effect task to this buff info.<br>
	 * Uses double-checked locking to initialize the map if it's necessary.
	 * @param effect the effect that owns the task
	 * @param effectTaskInfo the task info
	 */
	private void addTask(AbstractEffect effect, EffectTaskInfo effectTaskInfo)
	{
		if (_tasks == null)
		{
			lock (this)
			{
				if (_tasks == null)
				{
					_tasks = new();
				}
			}
		}

		_tasks.put(effect, effectTaskInfo);
	}

	/**
	 * Gets the task for the given effect.
	 * @param effect the effect
	 * @return the task
	 */
	private EffectTaskInfo? getEffectTask(AbstractEffect effect)
	{
		return _tasks == null ? null : _tasks.get(effect);
	}

	/**
	 * Gets the skill that created this buff info.
	 * @return the skill
	 */
	public Skill getSkill()
	{
		return _skill;
	}

	/**
	 * Gets the calculated abnormal time.
	 * @return the abnormal time
	 */
	public TimeSpan? getAbnormalTime()
	{
		return _abnormalTime;
	}

	/**
	 * Sets the abnormal time.
	 * @param abnormalTime the abnormal time to set
	 */
	public void setAbnormalTime(TimeSpan? abnormalTime)
	{
		_abnormalTime = abnormalTime;
	}

	/**
	 * Gets the period start ticks.
	 * @return the period start
	 */
	public int getPeriodStartTicks()
	{
		return _periodStartTicks;
	}

	/**
	 * @return the item that triggered this skill
	 */
	public Item? getItem()
	{
		return _item;
	}

	/**
	 * @return the options that issued this effect
	 */
	public Option? getOption()
	{
		return _option;
	}

	/**
	 * Get the remaining time in seconds for this buff info.
	 * @return the elapsed time
	 */
	public TimeSpan? getTime()
	{
		int ticks = GameTimeTaskManager.getInstance().getGameTicks() - _periodStartTicks;
		return _abnormalTime - TimeSpan.FromSeconds(1.0 * ticks / GameTimeTaskManager.TICKS_PER_SECOND);
	}

	/**
	 * Verify if this buff info has been cancelled.
	 * @return {@code true} if this buff info has been cancelled, {@code false} otherwise
	 */
	public bool isRemoved()
	{
		return _finishType == SkillFinishType.REMOVED;
	}

	/**
	 * Set the buff info to removed.
	 * @param type the SkillFinishType to set
	 */
	public void setFinishType(SkillFinishType type)
	{
		_finishType = type;
	}

	/**
	 * Verify if this buff info is in use.
	 * @return {@code true} if this buff info is in use, {@code false} otherwise
	 */
	public bool isInUse()
	{
		return _isInUse;
	}

	/**
	 * Set the buff info to in use.
	 * @param value the value to set
	 */
	public void setInUse(bool value)
	{
		_isInUse = value;

		// Send message that the effect is applied or removed.
		if (_skill != null && !_skill.IsHidingMessages && _effected.isPlayer())
		{
			if (value)
			{
				if (!_hideStartMessage && !_skill.IsAura && isDisplayedForEffected())
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_USED_S1);
					sm.Params.addSkillName(_skill);
					_effected.sendPacket(sm);
				}
			}
			else
			{
				SystemMessagePacket sm = new SystemMessagePacket(_skill.IsToggle ? SystemMessageId.S1_HAS_BEEN_ABORTED : SystemMessageId.THE_EFFECT_OF_S1_HAS_BEEN_REMOVED);
				sm.Params.addSkillName(_skill);
				_effected.sendPacket(sm);
			}
		}
	}

	/**
	 * Gets the character's object id that launched the buff.
	 * @return the object id of the effector.
	 */
	public int getEffectorObjectId()
	{
		return _effectorObjectId;
	}

	/**
	 * Gets the character that launched the buff.
	 * @return the effector
	 */
	public Creature getEffector()
	{
		return _effector;
	}

	/**
	 * Gets the target of the skill.
	 * @return the effected
	 */
	public Creature getEffected()
	{
		return _effected;
	}

	/**
	 * Stops all the effects for this buff info.<br>
	 * Removes effects stats.<br>
	 * <b>It will not remove the buff info from the effect list</b>.<br>
	 * Instead call {@link EffectList#stopSkillEffects(SkillFinishType, Skill)}
	 * @param type determines the system message that will be sent.
	 */
	public void stopAllEffects(SkillFinishType type)
	{
		setFinishType(type);

		// Remove this buff info from BuffFinishTask.
		_effected.removeBuffInfoTime(this);
		finishEffects();
	}

	public void initializeEffects()
	{
		if (_effected == null || _skill == null)
		{
			return;
		}

		// When effects are initialized, the successfully landed.
		if (!_hideStartMessage && _effected.isPlayer() && !_skill.IsHidingMessages && !_skill.IsAura && isDisplayedForEffected())
		{
			SystemMessagePacket sm = new SystemMessagePacket(_skill.IsToggle ? SystemMessageId.YOU_VE_USED_S1 : SystemMessageId.YOU_FEEL_THE_S1_EFFECT);
			sm.Params.addSkillName(_skill);
			_effected.sendPacket(sm);
		}

		// Creates a task that will stop all the effects.
		if (_abnormalTime > TimeSpan.Zero)
		{
			_effected.addBuffInfoTime(this);
		}

		foreach (AbstractEffect effect in _effects)
		{
			if (effect.IsInstant || (_effected.isDead() && !_skill.IsPassive && !_skill.IsStayAfterDeath))
			{
				continue;
			}

			// Call on start.
			effect.OnStart(_effector, _effected, _skill, _item);

			// Do not add continuous effect if target just died from the initial effect, otherwise they'll be ticked forever.
			if (_effected.isDead())
			{
				continue;
			}

			// If it's a continuous effect, if has ticks schedule a task with period, otherwise schedule a simple task to end it.
			if (effect.Ticks > 0)
			{
				// The task for the effect ticks.
				EffectTickTask effectTask = new EffectTickTask(this, effect);
				ScheduledFuture scheduledFuture = ThreadPool.scheduleAtFixedRate(effectTask,
					effect.Ticks * TimeSpan.FromMilliseconds(Config.Character.EFFECT_TICK_RATIO),
					effect.Ticks * TimeSpan.FromMilliseconds(Config.Character.EFFECT_TICK_RATIO));

				// Adds the task for ticking.
				addTask(effect, new EffectTaskInfo(effectTask, scheduledFuture));
			}
		}
	}

	/**
	 * Called on each tick.<br>
	 * Verify if the effect should end and the effect task should be cancelled.
	 * @param effect the effect that is ticking
	 */
	public void onTick(AbstractEffect effect)
	{
		bool continueForever = false;
		// If the effect is in use, allow it to affect the effected.
		if (_isInUse)
		{
			// Callback for on action time event.
			continueForever = effect.OnActionTime(_effector, _effected, _skill, _item);
		}

		if (!continueForever && _skill.IsToggle)
		{
			EffectTaskInfo? task = getEffectTask(effect);
			if (task != null)
			{
				ScheduledFuture schedule = task.getScheduledFuture();
				if (schedule != null && !schedule.isCancelled() && !schedule.isDone())
				{
					schedule.cancel(true); // Don't allow to finish current run.
				}
				_effected.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, _skill); // Remove the buff from the effect list.
			}
		}
	}

	public void finishEffects()
	{
		// Cancels the ticking task.
		if (_tasks != null)
		{
			foreach (EffectTaskInfo effectTask in _tasks.Values)
			{
				ScheduledFuture schedule = effectTask.getScheduledFuture();
				if (schedule != null && !schedule.isCancelled() && !schedule.isDone())
				{
					schedule.cancel(true); // Don't allow to finish current run.
				}
			}
		}

		// Notify on exit.
		foreach (AbstractEffect effect in _effects)
		{
			// Instant effects shouldn't call onExit(..).
			// if (!effect.isInstant())
			// {
			effect.OnExit(_effector, _effected, _skill);
			// }
		}

		// Set the proper system message.
		if (_skill != null && !(_effected.isSummon() && !((Summon) _effected).getOwner().hasSummon()) && !_skill.IsHidingMessages)
		{
			SystemMessageId? smId = null;
			if (_finishType == SkillFinishType.SILENT || !isDisplayedForEffected())
			{
				// smId is null.
			}
			else if (_skill.IsToggle)
			{
				smId = SystemMessageId.S1_HAS_BEEN_ABORTED;
			}
			else if (_finishType == SkillFinishType.REMOVED)
			{
				smId = SystemMessageId.THE_EFFECT_OF_S1_HAS_BEEN_REMOVED;
			}
			else if (!_skill.IsPassive)
			{
				smId = SystemMessageId.S1_HAS_WORN_OFF;
			}

            Player? effectedPlayer = _effected.getActingPlayer();
			if (smId != null && effectedPlayer != null && effectedPlayer.isOnline())
			{
				SystemMessagePacket sm = new SystemMessagePacket(smId.Value);
				sm.Params.addSkillName(_skill);
				_effected.sendPacket(sm);
			}
		}
	}

	public void resetAbnormalTime(TimeSpan? abnormalTime)
	{
		if (_abnormalTime > TimeSpan.Zero)
		{
			_periodStartTicks = GameTimeTaskManager.getInstance().getGameTicks();
			_abnormalTime = abnormalTime;
			_effected.removeBuffInfoTime(this);
			_effected.addBuffInfoTime(this);
		}
	}

	public bool isAbnormalType(AbnormalType type)
	{
		return _skill.AbnormalType == type;
	}

	/**
	 * Determines if this BuffInfo is displayed for the effected. These checks are needed to display A3 skills properly.<br>
	 * A3 skills are a type of skills that has different behavior depending on the effector and the effected.
	 * @return true if this BuffInfo is displayed for the effected, false otherwise.
	 */
	public bool isDisplayedForEffected()
	{
		return !_skill.IsSelfContinuous || _effected == _effector || !_skill.HasEffects(SkillEffectScope.Self);
	}
}