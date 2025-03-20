using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model;

/**
 * Effect lists.<br>
 * Holds all the buff infos that are affecting a creature.<br>
 * Manages the logic that controls whether a buff is added, remove, replaced or set inactive.<br>
 * Uses maps with skill ID as key and buff info DTO as value to avoid iterations.<br>
 * Uses Double-Checked Locking to avoid useless initialization and synchronization issues and overhead.<br>
 * Methods may resemble List interface, although it doesn't implement such interface.
 * @author Zoey76
 */
public class EffectList
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EffectList));

	/** Queue containing all effects from buffs for this effect list. */
	private readonly Set<BuffInfo> _actives = new();

	/** List containing all passives for this effect list. They bypass most of the actions and they are not included in most operations. */
	private readonly Set<BuffInfo> _passives = new();

	/** List containing all options for this effect list. They bypass most of the actions and they are not included in most operations. */
	private readonly Set<BuffInfo> _options = new();

	/** Map containing the all stacked effect in progress for each {@code AbnormalType}. */
	private Set<AbnormalType> _stackedEffects = new();

	/** Set containing all {@code AbnormalType}s that shouldn't be added to this creature effect list. */
	private readonly Set<AbnormalType> _blockedAbnormalTypes = new();

	/** Set containing all abnormal visual effects this creature currently displays. */
	private Set<AbnormalVisualEffect> _abnormalVisualEffects = new();

	/** Short buff skill ID. */
	private BuffInfo? _shortBuff;

	/** Count of specific types of buffs. */
	private readonly AtomicInteger _buffCount = new();
	private readonly AtomicInteger _triggerBuffCount = new();
	private readonly AtomicInteger _danceCount = new();
	private readonly AtomicInteger _toggleCount = new();
	private readonly AtomicInteger _debuffCount = new();

	/** If {@code true} this effect list has buffs removed on any action. */
	private readonly AtomicInteger _hasBuffsRemovedOnAnyAction = new();

	/** If {@code true} this effect list has buffs removed on damage. */
	private readonly AtomicInteger _hasBuffsRemovedOnDamage = new();

	/** Effect flags. */
	private long _effectFlags;

	/** The owner of this effect list. */
	private readonly Creature _owner;

	/** Hidden buffs count, prevents iterations. */
	private readonly AtomicInteger _hiddenBuffs = new();

	/** Delay task **/
	private ScheduledFuture? _updateEffectIconTask;
	private readonly AtomicBoolean _updateAbnormalStatus = new();

	/**
	 * Constructor for effect list.
	 * @param owner the creature that owns this effect list
	 */
	public EffectList(Creature owner)
	{
		_owner = owner;
	}

	/**
	 * Gets passive effects.
	 * @return an unmodifiable set containing all passives.
	 */
	public Set<BuffInfo> getPassives()
	{
		return _passives;
	}

	/**
	 * Gets option effects.
	 * @return an unmodifiable set containing all options.
	 */
	public Set<BuffInfo> getOptions()
	{
		return _options;
	}

	/**
	 * Gets all the active effects on this effect list.
	 * @return an unmodifiable set containing all the active effects on this effect list
	 */
	public ICollection<BuffInfo> getEffects()
	{
		return _actives;
	}

	/**
	 * Gets all the active positive effects on this effect list.
	 * @return all the buffs on this effect list
	 */
	public List<BuffInfo> getBuffs()
	{
		List<BuffInfo> result = new();
		foreach (BuffInfo info in _actives)
		{
			if (info.getSkill().BuffType == SkillBuffType.BUFF)
			{
				result.Add(info);
			}
		}
		return result;
	}

	/**
	 * Gets all the active positive effects on this effect list.
	 * @return all the dances songs on this effect list
	 */
	public List<BuffInfo> getDances()
	{
		List<BuffInfo> result = new();
		foreach (BuffInfo info in _actives)
		{
			if (info.getSkill().BuffType == SkillBuffType.DANCE)
			{
				result.Add(info);
			}
		}
		return result;
	}

	/**
	 * Gets all the active negative effects on this effect list.
	 * @return all the debuffs on this effect list
	 */
	public List<BuffInfo> getDebuffs()
	{
		List<BuffInfo> result = new();
		foreach (BuffInfo info in _actives)
		{
			if (info.getSkill().BuffType == SkillBuffType.DEBUFF)
			{
				result.Add(info);
			}
		}
		return result;
	}

	/**
	 * Verifies if this effect list contains the given skill ID.
	 * @param skillId the skill ID to verify
	 * @return {@code true} if the skill ID is present in the effect list (includes active and passive effects), {@code false} otherwise
	 */
	public bool isAffectedBySkill(int skillId)
	{
		foreach (BuffInfo info in _actives)
		{
			if (info.getSkill().Id == skillId)
			{
				return true;
			}
		}
		foreach (BuffInfo info in _passives)
		{
			if (info.getSkill().Id == skillId)
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * Gets the first {@code BuffInfo} found in this effect list.
	 * @param skillId the skill ID
	 * @return {@code BuffInfo} of the first active or passive effect found.
	 */
	public BuffInfo? getBuffInfoBySkillId(int skillId)
	{
		foreach (BuffInfo info in _actives)
		{
			if (info.getSkill().Id == skillId)
			{
				return info;
			}
		}
		foreach (BuffInfo info in _passives)
		{
			if (info.getSkill().Id == skillId)
			{
				return info;
			}
		}
		return null;
	}

	/**
	 * Check if any active {@code BuffInfo} of this {@code AbnormalType} exists.
	 * @param type the abnormal skill type
	 * @return {@code true} if there is any {@code BuffInfo} matching the specified {@code AbnormalType}, {@code false} otherwise
	 */
	public bool hasAbnormalType(AbnormalType type)
	{
		return _stackedEffects.Contains(type);
	}

	/**
	 * Check if any active {@code BuffInfo} of this {@code AbnormalType} exists.
	 * @param types the abnormal skill type
	 * @return {@code true} if there is any {@code BuffInfo} matching one of the specified {@code AbnormalType}s, {@code false} otherwise
	 */
	public bool hasAbnormalType(ICollection<AbnormalType> types)
	{
		foreach (AbnormalType abnormalType in _stackedEffects)
		{
			if (types.Contains(abnormalType))
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * @param type the {@code AbnormalType} to match for.
	 * @param filter any additional filters to match for once a {@code BuffInfo} of this {@code AbnormalType} is found.
	 * @return {@code true} if there is any {@code BuffInfo} matching the specified {@code AbnormalType} and given filter, {@code false} otherwise
	 */
	public bool hasAbnormalType(AbnormalType type, Predicate<BuffInfo> filter)
	{
		if (hasAbnormalType(type))
		{
			foreach (BuffInfo info in _actives)
			{
				if (info.isAbnormalType(type) && filter(info))
				{
					return true;
				}
			}
		}
		return false;
	}

	/**
	 * Gets the first {@code BuffInfo} found by the given {@code AbnormalType}.<br>
	 * <font color="red">There are some cases where there are multiple {@code BuffInfo} per single {@code AbnormalType}</font>.
	 * @param type the abnormal skill type
	 * @return the {@code BuffInfo} if it's present, {@code null} otherwise
	 */
	public BuffInfo? getFirstBuffInfoByAbnormalType(AbnormalType type)
	{
		if (hasAbnormalType(type))
		{
			foreach (BuffInfo info in _actives)
			{
				if (info.isAbnormalType(type))
				{
					return info;
				}
			}
		}
		return null;
	}

	/**
	 * Adds {@code AbnormalType}s to the blocked buff slot set.
	 * @param blockedAbnormalTypes the blocked buff slot set to add
	 */
	public void addBlockedAbnormalTypes(IEnumerable<AbnormalType> blockedAbnormalTypes)
	{
		_blockedAbnormalTypes.addAll(blockedAbnormalTypes);
	}

	/**
	 * Removes {@code AbnormalType}s from the blocked buff slot set.
	 * @param blockedBuffSlots the blocked buff slot set to remove
	 * @return {@code true} if the blocked buff slots set has been modified, {@code false} otherwise
	 */
	public bool removeBlockedAbnormalTypes(IEnumerable<AbnormalType> blockedBuffSlots)
	{
		return _blockedAbnormalTypes.removeAll(blockedBuffSlots);
	}

	/**
	 * Gets all the blocked {@code AbnormalType}s for this creature effect list.
	 * @return the current blocked {@code AbnormalType}s set in unmodifiable view.
	 */
	public Set<AbnormalType> getBlockedAbnormalTypes()
	{
		return _blockedAbnormalTypes;
	}

	/**
	 * Sets the Short Buff data and sends an update if the effected is a player.
	 * @param info the {@code BuffInfo}
	 */
	public void shortBuffStatusUpdate(BuffInfo? info)
	{
		if (_owner.isPlayer())
		{
			_shortBuff = info;
			if (info == null)
			{
				_owner.sendPacket(ShortBuffStatusUpdatePacket.RESET_SHORT_BUFF);
			}
			else
			{
				_owner.sendPacket(new ShortBuffStatusUpdatePacket(info.getSkill().Id, info.getSkill().Level,
					info.getSkill().SubLevel, (int)(info.getTime() ?? TimeSpan.Zero).TotalSeconds));
			}
		}
	}

	/**
	 * Gets the buffs count without including the hidden buffs (after getting an Herb buff).<br>
	 * Prevents initialization.
	 * @return the number of buffs in this creature effect list
	 */
	public int getBuffCount()
	{
		return _actives.Count != 0 ? _buffCount.get() - _hiddenBuffs.get() : 0;
	}

	/**
	 * Gets the Songs/Dances count.<br>
	 * Prevents initialization.
	 * @return the number of Songs/Dances in this creature effect list
	 */
	public int getDanceCount()
	{
		return _danceCount.get();
	}

	/**
	 * Gets the triggered buffs count.<br>
	 * Prevents initialization.
	 * @return the number of triggered buffs in this creature effect list
	 */
	public int getTriggeredBuffCount()
	{
		return _triggerBuffCount.get();
	}

	/**
	 * Gets the toggled skills count.<br>
	 * Prevents initialization.
	 * @return the number of toggle skills in this creature effect list
	 */
	public int getToggleCount()
	{
		return _toggleCount.get();
	}

	/**
	 * Gets the debuff skills count.<br>
	 * Prevents initialization.
	 * @return the number of debuff effects in this creature effect list
	 */
	public int getDebuffCount()
	{
		return _debuffCount.get();
	}

	/**
	 * Gets the hidden buff count.
	 * @return the number of hidden buffs
	 */
	public int getHiddenBuffsCount()
	{
		return _hiddenBuffs.get();
	}

	/**
	 * Exits all effects in this effect list.<br>
	 * Stops all the effects, clear the effect lists and updates the effect flags and icons.
	 * @param broadcast {@code true} to broadcast update packets, {@code false} otherwise.
	 */
	public void stopAllEffects(bool broadcast)
	{
		stopEffects(b => !b.getSkill().IsIrreplacableBuff, true, broadcast);
	}

	/**
	 * Stops all effects in this effect list except those that last through death.
	 */
	public void stopAllEffectsExceptThoseThatLastThroughDeath()
	{
		stopEffects(info => !info.getSkill().IsStayAfterDeath, true, true);
	}

	/**
	 * Exits all active, passive and option effects in this effect list without excluding anything,<br>
	 * like necessary toggles, irreplacable buffs or effects that last through death.<br>
	 * Stops all the effects, clear the effect lists and updates the effect flags and icons.
	 * @param update set to true to update the effect flags and icons.
	 * @param broadcast {@code true} to broadcast update packets, {@code false} otherwise.
	 */
	public void stopAllEffectsWithoutExclusions(bool update, bool broadcast)
	{
		foreach (BuffInfo info in _actives)
		{
			remove(info);
		}

		foreach (BuffInfo info in _passives)
		{
			remove(info);
		}

		foreach (BuffInfo info in _options)
		{
			remove(info);
		}

		// Update stats, effect flags and icons.
		if (update)
		{
			updateEffectList(broadcast);
		}
	}

	/**
	 * Stops all active toggle skills.
	 */
	public void stopAllToggles()
	{
		if (_toggleCount.get() > 0)
		{
			stopEffects(b => b.getSkill().IsToggle && !b.getSkill().IsIrreplacableBuff, true, true);
		}
	}

	public void stopAllTogglesOfGroup(int toggleGroup)
	{
		if (_toggleCount.get() > 0)
		{
			stopEffects(b => b.getSkill().IsToggle && b.getSkill().ToggleGroupId == toggleGroup, true, true);
		}
	}

	/**
	 * Stops all active dances/songs skills.
	 * @param update set to true to update the effect flags and icons
	 * @param broadcast {@code true} to broadcast update packets if updating, {@code false} otherwise.
	 */
	public void stopAllPassives(bool update, bool broadcast)
	{
		if (!_passives.isEmpty())
		{
			_passives.ForEach(x => remove(x));
			// Update stats, effect flags and icons.
			if (update)
			{
				updateEffectList(broadcast);
			}
		}
	}

	/**
	 * Stops all active dances/songs skills.
	 * @param update set to true to update the effect flags and icons
	 * @param broadcast {@code true} to broadcast update packets if updating, {@code false} otherwise.
	 */
	public void stopAllOptions(bool update, bool broadcast)
	{
		if (!_options.isEmpty())
		{
			_options.ForEach(remove);

			// Update stats, effect flags and icons.
			if (update)
			{
				updateEffectList(broadcast);
			}
		}
	}

	/**
	 * Exit all effects having a specified flag.
	 * @param effectFlag the flag of the effect to stop
	 */
	public void stopEffects(EffectFlag effectFlag)
	{
		if (isAffected(effectFlag) && _actives.Count != 0)
		{
			bool update = false;
			foreach (BuffInfo info in _actives)
			{
				foreach (AbstractEffect effect in info.getEffects())
				{
					if (effect != null && (effect.getEffectFlags() & effectFlag.getMask()) != 0)
					{
						remove(info);
						update = true;
					}
				}
			}

			// Update stats, effect flags and icons.
			if (update)
			{
				updateEffectList(true);
			}
		}
	}

	/**
	 * Exits all effects created by a specific skill ID.<br>
	 * Removes the effects from the effect list.<br>
	 * Removes the stats from the creature.<br>
	 * Updates the effect flags and icons.<br>
	 * Presents overload:<br>
	 * {@link #stopSkillEffects(SkillFinishType, Skill)}
	 * @param type determines the system message that will be sent.
	 * @param skillId the skill ID
	 */
	public void stopSkillEffects(SkillFinishType type, int skillId)
	{
		BuffInfo? info = getBuffInfoBySkillId(skillId);
		if (info != null)
		{
			remove(info, type, true, true);
		}
	}

	/**
	 * Exits all effects created by a specific skill.<br>
	 * Removes the effects from the effect list.<br>
	 * Removes the stats from the creature.<br>
	 * Updates the effect flags and icons.<br>
	 * Presents overload:<br>
	 * {@link #stopSkillEffects(SkillFinishType, int)}
	 * @param type determines the system message that will be sent.
	 * @param skill the skill
	 */
	public void stopSkillEffects(SkillFinishType type, Skill skill)
	{
		stopSkillEffects(type, skill.Id);
	}

	/**
	 * Exits all effects created by a specific skill {@code AbnormalType}.<br>
	 * <font color="red">This function should not be used recursively, because it updates on every execute.</font>
	 * @param type the skill {@code AbnormalType}
	 * @return {@code true} if there was any {@code BuffInfo} with the given {@code AbnormalType}, {@code false} otherwise
	 */
	public bool stopEffects(AbnormalType type)
	{
		if (hasAbnormalType(type))
		{
			stopEffects(i => i.isAbnormalType(type), true, true);
			return true;
		}
		return false;
	}

	/**
	 * Exits all effects created by a specific skill {@code AbnormalType}s.
	 * @param types the skill {@code AbnormalType}s to be checked and removed.
	 * @return {@code true} if there was any {@code BuffInfo} with one of the given {@code AbnormalType}s, {@code false} otherwise
	 */
	public bool stopEffects(ICollection<AbnormalType> types)
	{
		if (hasAbnormalType(types))
		{
			stopEffects(i => types.Contains(i.getSkill().AbnormalType), true, true);
			return true;
		}
		return false;
	}

	/**
	 * Exits all effects matched by a specific filter.
	 * @param filter any filter to apply when selecting which {@code BuffInfo}s to be removed.
	 * @param update update effect flags and icons after the operation finishes.
	 * @param broadcast {@code true} to broadcast update packets if updating, {@code false} otherwise.
	 */
	public void stopEffects(Predicate<BuffInfo> filter, bool update, bool broadcast)
	{
		if (_actives.Count != 0)
		{
			foreach (BuffInfo info in _actives)
			{
				if (filter(info))
				{
					remove(info);
				}
			}

			// Update stats, effect flags and icons.
			if (update)
			{
				updateEffectList(broadcast);
			}
		}
	}

	/**
	 * Exits all buffs effects of the skills with "removedOnAnyAction" set.<br>
	 * Called on any action except movement (attack, cast).
	 */
	public void stopEffectsOnAction()
	{
		if (_hasBuffsRemovedOnAnyAction.get() > 0)
		{
			stopEffects(info => info.getSkill().IsRemovedOnAnyActionExceptMove, true, true);
		}
	}

	public void stopEffectsOnDamage()
	{
		if (_hasBuffsRemovedOnDamage.get() > 0)
		{
			stopEffects(info => info.getSkill().IsRemovedOnDamage, true, true);
		}
	}

	/**
	 * Checks if a given effect limitation is exceeded.
	 * @param buffTypes the {@code SkillBuffType} of the skill.
	 * @return {@code true} if the current effect count for any of the given types is greater than the limit, {@code false} otherwise.
	 */
	private bool isLimitExceeded(ImmutableArray<SkillBuffType> buffTypes)
	{
		foreach (SkillBuffType buffType in buffTypes)
		{
			switch (buffType)
			{
				case SkillBuffType.TRIGGER:
				{
					if (_triggerBuffCount.get() > Config.Character.TRIGGERED_BUFFS_MAX_AMOUNT)
					{
						return true;
					}
					break;
				}
				case SkillBuffType.DANCE:
				{
					if (_danceCount.get() > Config.Character.DANCES_MAX_AMOUNT)
					{
						return true;
					}
					break;
				}
				// case TOGGLE: Do toggles have limit?
				case SkillBuffType.DEBUFF:
				{
					if (_debuffCount.get() > 24)
					{
						return true;
					}
					break;
				}
				case SkillBuffType.BUFF:
				{
					if (getBuffCount() > _owner.getStat().getMaxBuffCount())
					{
						return true;
					}
					break;
				}
			}
		}
		return false;
	}

	/**
	 * @param info the {@code BuffInfo} whose buff category will be increased/decreased in count.
	 * @param increase {@code true} to increase the category count of this {@code BuffInfo}, {@code false} to decrease.
	 * @return the new count of the given {@code BuffInfo}'s category.
	 */
	private int increaseDecreaseCount(BuffInfo info, bool increase)
	{
		// If it's a hidden buff, manage hidden buff count.
		if (!info.isInUse())
		{
			if (increase)
			{
				_hiddenBuffs.incrementAndGet();
			}
			else
			{
				_hiddenBuffs.decrementAndGet();
			}
		}

		// Update flag for skills being removed on action or damage.
		if (info.getSkill().IsRemovedOnAnyActionExceptMove)
		{
			if (increase)
			{
				_hasBuffsRemovedOnAnyAction.incrementAndGet();
			}
			else
			{
				_hasBuffsRemovedOnAnyAction.decrementAndGet();
			}
		}
		if (info.getSkill().IsRemovedOnDamage)
		{
			if (increase)
			{
				_hasBuffsRemovedOnDamage.incrementAndGet();
			}
			else
			{
				_hasBuffsRemovedOnDamage.decrementAndGet();
			}
		}

		// Increase specific buff count
		switch (info.getSkill().BuffType)
		{
			case SkillBuffType.TRIGGER:
			{
				return increase ? _triggerBuffCount.incrementAndGet() : _triggerBuffCount.decrementAndGet();
			}
			case SkillBuffType.DANCE:
			{
				return increase ? _danceCount.incrementAndGet() : _danceCount.decrementAndGet();
			}
			case SkillBuffType.TOGGLE:
			{
				return increase ? _toggleCount.incrementAndGet() : _toggleCount.decrementAndGet();
			}
			case SkillBuffType.DEBUFF:
			{
				return increase ? _debuffCount.incrementAndGet() : _debuffCount.decrementAndGet();
			}
			case SkillBuffType.BUFF:
			{
				return increase ? _buffCount.incrementAndGet() : _buffCount.decrementAndGet();
			}
		}

		return 0;
	}

	/**
	 * Removes a set of effects from this effect list.<br>
	 * <font color="red">Does NOT update effect icons and flags. </font>
	 * @param info the effects to remove
	 */
	private void remove(BuffInfo info)
	{
		remove(info, SkillFinishType.REMOVED, false, false);
	}

	/**
	 * Removes a set of effects from this effect list.
	 * @param info the effects to remove
	 * @param type determines the system message that will be sent.
	 * @param update {@code true} if effect flags and icons should be updated after this removal, {@code false} otherwise.
	 * @param broadcast {@code true} to broadcast update packets if updating, {@code false} otherwise.
	 */
	public void remove(BuffInfo info, SkillFinishType type, bool update, bool broadcast)
	{
		if (info == null)
		{
			return;
		}

		if (info.getOption() != null)
		{
			// Remove separately if its an option.
			removeOption(info, type);
		}
		else if (info.getSkill().IsPassive)
		{
			// Remove Passive effect.
			removePassive(info, type);
		}
		else
		{
			// Remove active effect.
			removeActive(info, type);
			if (_owner.isNpc()) // Fix for all NPC debuff animations removed.
			{
				updateEffectList(broadcast);
			}
		}

		// Update stats, effect flags and icons.
		if (update)
		{
			updateEffectList(broadcast);
		}
	}

	private void removeActive(BuffInfo info, SkillFinishType type)
	{
		if (_actives.Count != 0)
		{
			// Removes the buff from the given effect list.
			_actives.Remove(info);

			// Remove short buff.
			if (info == _shortBuff)
			{
				shortBuffStatusUpdate(null);
			}

			// Stop the buff effects.
			info.stopAllEffects(type);

			// Decrease specific buff count
			increaseDecreaseCount(info, false);
			info.getSkill().ApplyEffectScope(SkillEffectScope.End, info, true, false);
		}
	}

	private void removePassive(BuffInfo info, SkillFinishType type)
	{
		if (!_passives.isEmpty())
		{
			_passives.remove(info);
			info.stopAllEffects(type);
		}
	}

	private void removeOption(BuffInfo info, SkillFinishType type)
	{
		if (!_options.isEmpty())
		{
			_options.remove(info);
			info.stopAllEffects(type);
		}
	}

	/**
	 * Adds a set of effects to this effect list.
	 * @param info the {@code BuffInfo}
	 */
	public void add(BuffInfo info)
	{
		if (info == null)
		{
			return;
		}

		Skill skill = info.getSkill();

		// Prevent adding and initializing buffs/effects on dead creatures.
		if (info.getEffected().isDead() && skill != null && !skill.IsPassive && !skill.IsStayAfterDeath)
		{
			return;
		}

		if (skill == null)
		{
			// Only options are without skills.
			addOption(info);
		}
		else if (skill.IsPassive)
		{
			// Passive effects are treated specially
			addPassive(info);
		}
		else
		{
			// Add active effect
			addActive(info);
		}

		// Update stats, effect flags and icons.
		updateEffectList(true);
	}

	private void addActive(BuffInfo info)
	{
		Skill skill = info.getSkill();

		// Cannot add active buff to dead creature. Even in retail if you are dead with Lv. 3 Shillien's Breath, it will disappear instead of going 1 level down.
		if (info.getEffected().isDead() && !skill.IsStayAfterDeath)
		{
			return;
		}

		if (_blockedAbnormalTypes != null && _blockedAbnormalTypes.Contains(skill.AbnormalType))
		{
			return;
		}

		// Fix for stacking trigger skills
		if (skill.IsTriggeredSkill)
		{
			BuffInfo? triggerInfo = info.getEffected().getEffectList().getBuffInfoBySkillId(skill.Id);
			if (triggerInfo != null && triggerInfo.getSkill().Level >= skill.Level)
			{
				return;
			}
		}

		if (info.getEffector() != null)
		{
			// Check for debuffs against target.
			if (info.getEffector() != info.getEffected() && skill.IsBad)
			{
				// Check if effected is debuff blocked.
                if (info.getEffected().isDebuffBlocked() ||
                    (info.getEffector() is Player effectorPlayer && effectorPlayer.isGM() &&
                        !effectorPlayer.getAccessLevel().CanGiveDamage))
                {
                    return;
                }

                if (info.getEffector().isPlayer() && info.getEffected().isPlayer() &&
                    info.getEffected().isAffected(EffectFlag.DUELIST_FURY) &&
                    !info.getEffector().isAffected(EffectFlag.DUELIST_FURY))
                {
                    return;
                }
            }

			// Check if buff skills are blocked.
			if (info.getEffected().isBuffBlocked() && !skill.IsBad)
			{
				return;
			}
		}

		// Manage effect stacking.
		if (hasAbnormalType(skill.AbnormalType))
		{
			foreach (BuffInfo existingInfo in _actives)
			{
				Skill existingSkill = existingInfo.getSkill();
				// Check if existing effect should be removed due to stack.
				// Effects with no abnormal don't stack if their ID is the same. Effects of the same abnormal type don't stack.
				if ((skill.AbnormalType == AbnormalType.NONE && existingSkill.Id == skill.Id) ||
				    (skill.AbnormalType != AbnormalType.NONE &&
				     existingSkill.AbnormalType == skill.AbnormalType))
				{
					// Check if there is subordination abnormal. Skills with subordination abnormal stack with each other, unless the caster is the same.
					if (skill.SubordinationAbnormalType != AbnormalType.NONE &&
					    skill.SubordinationAbnormalType == existingSkill.SubordinationAbnormalType //
					    && (info.getEffectorObjectId() == 0 || existingInfo.getEffectorObjectId() == 0 ||
					        info.getEffectorObjectId() != existingInfo.getEffectorObjectId()))
					{
						continue;
					}

					// The effect we are adding overrides the existing effect. Delete or disable the existing effect.
					if (skill.AbnormalLevel >= existingSkill.AbnormalLevel)
					{
						// If it is an herb, set as not in use the lesser buff, unless it is the same skill.
						if ((skill.IsAbnormalInstant || existingSkill.IsIrreplacableBuff) &&
						    skill.Id != existingSkill.Id)
						{
							existingInfo.setInUse(false);
							_hiddenBuffs.incrementAndGet();
						}
						else
						{
							// Remove effect that gets overridden.
							remove(existingInfo);
						}
					}
					else if (skill.IsIrreplacableBuff) // The effect we try to add should be hidden.
					{
						info.setInUse(false);
					}
					else // The effect we try to add should be overridden.
					{
						return;
					}
				}
			}
		}

		// Increase buff count.
		increaseDecreaseCount(info, true);

		// Check if any effect limit is exceeded.
		if (isLimitExceeded(EnumUtil.GetValues<SkillBuffType>()))
		{
			// Check for each category.
			foreach (BuffInfo existingInfo in _actives)
			{
				if (existingInfo.isInUse() && !skill.Is7Signs &&
				    isLimitExceeded([existingInfo.getSkill().BuffType]))
				{
					remove(existingInfo);
				}

				// Break further loops if there is no any other limit exceeding.
				if (!isLimitExceeded(EnumUtil.GetValues<SkillBuffType>()))
				{
					break;
				}
			}
		}

		// After removing old buff (same ID) or stacked buff (same abnormal type),
		// Add the buff to the end of the effect list.
		_actives.Add(info);
		// Initialize effects.
		info.initializeEffects();
	}

	private void addPassive(BuffInfo info)
	{
		Skill skill = info.getSkill();

		// Passive effects don't need stack type!
		if (skill.AbnormalType != AbnormalType.NONE)
		{
			LOGGER.Warn("Passive " + skill + " with abnormal type: " + skill.AbnormalType + "!");
		}

		// Remove previous passives of this id.
		foreach (BuffInfo b in _passives)
		{
			if (b != null && b.getSkill().Id == skill.Id)
			{
				b.setInUse(false);
				_passives.remove(b);
			}
		}

		_passives.add(info);

		// Initialize effects.
		info.initializeEffects();
	}

	private void addOption(BuffInfo info)
    {
        Options.Options? option = info.getOption();
		if (option != null)
		{
			// Remove previous options of this id.
			foreach (BuffInfo b in _options)
			{
				if (b.getOption()?.getId() == option.getId())
				{
					b.setInUse(false);
					_options.remove(b);
				}
			}

			_options.add(info);

			// Initialize effects.
			info.initializeEffects();
		}
	}

	/**
	 * Update effect icons.<br>
	 * Prevents initialization.
	 * @param partyOnly {@code true} only party icons need to be updated.
	 */
	public void updateEffectIcons(bool partyOnly)
	{
		if (!partyOnly)
		{
			_updateAbnormalStatus.compareAndSet(false, true);
		}

		if (_updateEffectIconTask == null)
		{
			_updateEffectIconTask = ThreadPool.schedule(() =>
			{
				Player? player = _owner.getActingPlayer();
				if (player != null)
				{
					Party? party = player.getParty();
					AbnormalStatusUpdatePacket? asu = _owner.isPlayer() && _updateAbnormalStatus.get() ? new AbnormalStatusUpdatePacket(new List<BuffInfo>()) : null;
					PartySpelledPacket? ps = party != null || _owner.isSummon() ? new PartySpelledPacket(_owner) : null;
					ExOlympiadSpelledInfoPacket? os = player.isInOlympiadMode() && player.isOlympiadStart() ? new ExOlympiadSpelledInfoPacket(player) : null;
					if (_actives.Count != 0)
					{
						foreach (BuffInfo info in _actives.OrderBy(x => x.getPeriodStartTicks()))
						{
							if (info != null && info.isInUse())
							{
								if (info.getSkill().IsHealingPotionSkill)
								{
									shortBuffStatusUpdate(info);
								}
								else if (info.isDisplayedForEffected())
								{
									if (asu != null)
										asu.Value.addSkill(info);

									if (ps != null && !info.getSkill().IsToggle)
										ps.Value.addSkill(info);

									if (os != null)
										os.Value.addSkill(info);
								}
							}
						}
					}

					// Send icon update for player buff bar.
					if (asu != null)
						_owner.sendPacket(asu.Value);

					// Player or summon is in party. Broadcast packet to everyone in the party.
					if (ps != null)
					{
						if (party != null)
						{
							party.broadcastPacket(ps.Value);
						}
						else // Not in party, then its a summon info for its owner.
						{
							player.sendPacket(ps.Value);
						}
					}

					// Send icon update to all olympiad observers.
					if (os != null)
					{
						OlympiadGameTask? game = OlympiadGameManager.getInstance().getOlympiadTask(player.getOlympiadGameId());
						if (game != null && game.isBattleStarted())
						{
							game.getStadium().broadcastPacketToObservers(os.Value);
						}
					}
				}

				// Update effect icons for everyone targeting this owner.
				ExAbnormalStatusUpdateFromTargetPacket upd = new ExAbnormalStatusUpdateFromTargetPacket(_owner);
				foreach (Creature creature in _owner.getStatus().getStatusListener())
				{
					if (creature != null && creature.isPlayer())
					{
						creature.sendPacket(upd);
					}
				}

				if (_owner.isPlayer() && _owner.getTarget() == _owner)
				{
					_owner.sendPacket(upd);
				}

				_updateAbnormalStatus.set(false);
				_updateEffectIconTask = null;
			}, 300);
		}
	}

	/**
	 * Gets the currently applied abnormal visual effects.
	 * @return the abnormal visual effects
	 */
	public Set<AbnormalVisualEffect> getCurrentAbnormalVisualEffects()
	{
		return _abnormalVisualEffects;
	}

	/**
	 * Checks if the creature has the abnormal visual effect.
	 * @param ave the abnormal visual effect
	 * @return {@code true} if the creature has the abnormal visual effect, {@code false} otherwise
	 */
	public bool hasAbnormalVisualEffect(AbnormalVisualEffect ave)
	{
		return _abnormalVisualEffects.Contains(ave);
	}

	/**
	 * Adds the abnormal visual and sends packet for updating them in client.
	 * @param aves the abnormal visual effects
	 */
	public void startAbnormalVisualEffect(params AbnormalVisualEffect[] aves)
	{
		foreach (AbnormalVisualEffect ave in aves)
		{
			_abnormalVisualEffects.add(ave);
		}
		_owner.updateAbnormalVisualEffects();
	}

	/**
	 * Removes the abnormal visual and sends packet for updating them in client.
	 * @param aves the abnormal visual effects
	 */
	public void stopAbnormalVisualEffect(params AbnormalVisualEffect[] aves)
	{
		foreach (AbnormalVisualEffect ave in aves)
		{
			_abnormalVisualEffects.remove(ave);
		}
		_owner.updateAbnormalVisualEffects();
	}

	/**
	 * Wrapper to update abnormal icons and effect flags.
	 * @param broadcast {@code true} sends update packets to observing players, {@code false} doesn't send any packets.
	 */
	private void updateEffectList(bool broadcast)
	{
		// Create new empty flags.
		long flags = 0;
		Set<AbnormalType> abnormalTypeFlags = new();
		Set<AbnormalVisualEffect> abnormalVisualEffectFlags = new();
		Set<BuffInfo> unhideBuffs = new();

		// Recalculate new flags
		foreach (BuffInfo info in _actives)
		{
			if (info != null && info.isDisplayedForEffected())
			{
				Skill skill = info.getSkill();

				// Handle hidden buffs. Check if there was such abnormal before so we can continue.
				if (_hiddenBuffs.get() > 0 && _stackedEffects.Contains(skill.AbnormalType))
				{
					// If incoming buff isnt hidden, remove any hidden buffs with its abnormal type.
					if (info.isInUse())
					{
						unhideBuffs.removeIf(b => b.isAbnormalType(skill.AbnormalType));
					}

					// If this incoming buff is hidden and its first of its abnormal, or it removes
					// any previous hidden buff with the same or lower abnormal level and add this instead.
					else if (!abnormalTypeFlags.Contains(skill.AbnormalType) || unhideBuffs.removeIf(b =>
						         b.isAbnormalType(skill.AbnormalType) &&
						         b.getSkill().AbnormalLevel <= skill.AbnormalLevel))
					{
						unhideBuffs.add(info);
					}
				}

				// Add the EffectType flag.
				foreach (AbstractEffect e in info.getEffects())
				{
					flags |= e.getEffectFlags();
				}

				// Add the AbnormalType flag.
				abnormalTypeFlags.add(skill.AbnormalType);

				// Add AbnormalVisualEffect flag.
				if (skill.HasAbnormalVisualEffects)
				{
					foreach (AbnormalVisualEffect ave in skill.AbnormalVisualEffects)
					{
						abnormalVisualEffectFlags.add(ave);
						_abnormalVisualEffects.add(ave);
					}
					if (broadcast)
					{
						_owner.updateAbnormalVisualEffects();
					}
				}
			}
		}
		// Add passive effect flags.
		foreach (BuffInfo info in _passives)
		{
			if (info != null)
			{
				// Add the EffectType flag.
				foreach (AbstractEffect e in info.getEffects())
				{
					flags |= e.getEffectFlags();
				}

				// Add AbnormalVisualEffect flag.
				Skill skill = info.getSkill();
				if (skill.HasAbnormalVisualEffects)
				{
					foreach (AbnormalVisualEffect ave in skill.AbnormalVisualEffects)
					{
						abnormalVisualEffectFlags.add(ave);
						_abnormalVisualEffects.add(ave);
					}
					if (broadcast)
					{
						_owner.updateAbnormalVisualEffects();
					}
				}
			}
		}

		// Replace the old flags with the new flags.
		_effectFlags = flags;
		_stackedEffects = abnormalTypeFlags;

		// Unhide the selected buffs.
		unhideBuffs.ForEach(b =>
		{
			b.setInUse(true);
			_hiddenBuffs.decrementAndGet();
		});

		// Recalculate all stats
		_owner.getStat().recalculateStats(broadcast);

		if (broadcast)
		{
			// Check if there is change in AbnormalVisualEffect
			if (!abnormalVisualEffectFlags.containsAll(_abnormalVisualEffects))
			{
				_abnormalVisualEffects = abnormalVisualEffectFlags;
				_owner.updateAbnormalVisualEffects();
			}

			// Send updates to the client
			updateEffectIcons(false);
		}
	}

	/**
	 * Check if target is affected with special buff
	 * @param flag of special buff
	 * @return bool true if affected
	 */
	public bool isAffected(EffectFlag flag)
	{
		return (_effectFlags & flag.getMask()) != 0;
	}
}