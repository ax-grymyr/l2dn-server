using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Actor.Stats;

public class CreatureStat
{
	private readonly Creature _creature;
	private long _exp = 0;
	private long _sp = 0;
	private int _level = 1;
	/** Creature's maximum buff count. */
	private int _maxBuffCount = Config.BUFFS_MAX_AMOUNT;
	private double _vampiricSum = 0;
	private double _mpVampiricSum = 0;
	
	private readonly Map<Stat, double> _statsAdd = new();
	private readonly Map<Stat, double> _statsMul = new();
	private readonly Map<Stat, Map<MoveType, double>> _moveTypeStats = new();
	private readonly Map<int, double> _reuseStat = new();
	private readonly Map<int, double> _mpConsumeStat = new();
	private readonly Map<int, LinkedList<double>> _skillEvasionStat = new();
	private readonly Map<Stat, Map<Position, double>> _positionStats = new();
	private readonly List<StatHolder> _additionalAdd = new();
	private readonly List<StatHolder> _additionalMul = new();
	private readonly Map<Stat, double> _fixedValue = new();
	
	private readonly float[] _attackTraitValues = new float[EnumUtil.GetValues<TraitType>().Length];
	private readonly float[] _defenceTraitValues = new float[EnumUtil.GetValues<TraitType>().Length];
	private readonly Set<TraitType> _attackTraits = new();
	private readonly Set<TraitType> _defenceTraits = new();
	private readonly Set<TraitType> _invulnerableTraits = new();
	
	/** Values to be recalculated after every stat update */
	private double _attackSpeedMultiplier = 1;
	private double _mAttackSpeedMultiplier = 1;
	
	private readonly object _lock = new();
	
	public CreatureStat(Creature creature)
	{
		_creature = creature;
		for (int i = 0; i < _attackTraitValues.Length; i++)
		{
			_attackTraitValues[i] = 1;
			_defenceTraitValues[i] = 0;
		}
	}
	
	/**
	 * @return the Accuracy (base+modifier) of the Creature in function of the Weapon Expertise Penalty.
	 */
	public int getAccuracy()
	{
		return (int) getValue(Stat.ACCURACY_COMBAT);
	}
	
	public int getCpRegen()
	{
		return (int) getValue(Stat.REGENERATE_CP_RATE);
	}
	
	public int getHpRegen()
	{
		return (int) getValue(Stat.REGENERATE_HP_RATE);
	}
	
	public int getMpRegen()
	{
		return (int) getValue(Stat.REGENERATE_MP_RATE);
	}
	
	/**
	 * @return the Magic Accuracy (base+modifier) of the Creature
	 */
	public int getMagicAccuracy()
	{
		return (int) getValue(Stat.ACCURACY_MAGIC);
	}
	
	public virtual Creature getActiveChar()
	{
		return _creature;
	}
	
	/**
	 * @return the Attack Speed multiplier (base+modifier) of the Creature to get proper animations.
	 */
	public double getAttackSpeedMultiplier()
	{
		return _attackSpeedMultiplier;
	}
	
	public double getMAttackSpeedMultiplier()
	{
		return _mAttackSpeedMultiplier;
	}
	
	/**
	 * @return the CON of the Creature (base+modifier).
	 */
	public int getCON()
	{
		return (int) getValue(Stat.STAT_CON);
	}
	
	/**
	 * @param init
	 * @return the Critical Damage rate (base+modifier) of the Creature.
	 */
	public double getCriticalDmg(double init)
	{
		return getValue(Stat.CRITICAL_DAMAGE, init);
	}
	
	/**
	 * @return the Critical Hit rate (base+modifier) of the Creature.
	 */
	public int getCriticalHit()
	{
		return (int) getValue(Stat.CRITICAL_RATE);
	}
	
	/**
	 * @return the DEX of the Creature (base+modifier).
	 */
	public int getDEX()
	{
		return (int) getValue(Stat.STAT_DEX);
	}
	
	/**
	 * @return the Attack Evasion rate (base+modifier) of the Creature.
	 */
	public int getEvasionRate()
	{
		return (int) getValue(Stat.EVASION_RATE);
	}
	
	/**
	 * @return the Attack Evasion rate (base+modifier) of the Creature.
	 */
	public int getMagicEvasionRate()
	{
		return (int) getValue(Stat.MAGIC_EVASION_RATE);
	}
	
	public virtual long getExp()
	{
		return _exp;
	}
	
	public virtual void setExp(long value)
	{
		_exp = value;
	}
	
	/**
	 * @return the INT of the Creature (base+modifier).
	 */
	public int getINT()
	{
		return (int) getValue(Stat.STAT_INT);
	}
	
	public virtual int getLevel()
	{
		return _level;
	}
	
	public virtual void setLevel(int value)
	{
		_level = value;
	}
	
	/**
	 * @param skill
	 * @return the Magical Attack range (base+modifier) of the Creature.
	 */
	public int getMagicalAttackRange(Skill skill)
	{
		if (skill != null)
		{
			return skill.getCastRange() + (int) getValue(Stat.MAGIC_ATTACK_RANGE, 0);
		}
		return _creature.getTemplate().getBaseAttackRange();
	}
	
	public int getMaxCp()
	{
		return (int) getValue(Stat.MAX_CP);
	}
	
	public int getMaxRecoverableCp()
	{
		return (int) getValue(Stat.MAX_RECOVERABLE_CP, getMaxCp());
	}
	
	public virtual int getMaxHp()
	{
		return (int) getValue(Stat.MAX_HP);
	}
	
	public int getMaxRecoverableHp()
	{
		return (int) getValue(Stat.MAX_RECOVERABLE_HP, getMaxHp());
	}
	
	public int getMaxMp()
	{
		return (int) getValue(Stat.MAX_MP);
	}
	
	public int getMaxRecoverableMp()
	{
		return (int) getValue(Stat.MAX_RECOVERABLE_MP, getMaxMp());
	}
	
	/**
	 * Return the MAtk (base+modifier) of the Creature.<br>
	 * <br>
	 * <b><u>Example of use</u>: Calculate Magic damage
	 * @return
	 */
	public int getMAtk()
	{
		return (int) getValue(Stat.MAGIC_ATTACK);
	}
	
	public int getWeaponBonusMAtk()
	{
		return (int) getValue(Stat.WEAPON_BONUS_MAGIC_ATTACK);
	}
	
	/**
	 * @return the MAtk Speed (base+modifier) of the Creature in function of the Armour Expertise Penalty.
	 */
	public virtual int getMAtkSpd()
	{
		return (int) getValue(Stat.MAGIC_ATTACK_SPEED);
	}
	
	/**
	 * @return the Magic Critical Hit rate (base+modifier) of the Creature.
	 */
	public int getMCriticalHit()
	{
		return (int) getValue(Stat.MAGIC_CRITICAL_RATE);
	}
	
	/**
	 * <b><u>Example of use </u>: Calculate Magic damage.
	 * @return the MDef (base+modifier) of the Creature against a skill in function of abnormal effects in progress.
	 */
	public int getMDef()
	{
		return (int) getValue(Stat.MAGICAL_DEFENCE);
	}
	
	/**
	 * @return the MEN of the Creature (base+modifier).
	 */
	public int getMEN()
	{
		return (int) getValue(Stat.STAT_MEN);
	}
	
	public double getMovementSpeedMultiplier()
	{
		double baseSpeed;
		if (_creature.isInsideZone(ZoneId.WATER))
		{
			baseSpeed = _creature.getTemplate().getBaseValue(_creature.isRunning() ? Stat.SWIM_RUN_SPEED : Stat.SWIM_WALK_SPEED, 0);
		}
		else
		{
			baseSpeed = _creature.getTemplate().getBaseValue(_creature.isRunning() ? Stat.RUN_SPEED : Stat.WALK_SPEED, 0);
		}
		return getMoveSpeed() * (1.0 / baseSpeed);
	}
	
	/**
	 * @return the RunSpeed (base+modifier) of the Creature in function of the Armour Expertise Penalty.
	 */
	public virtual double getRunSpeed()
	{
		return getValue(_creature.isInsideZone(ZoneId.WATER) ? Stat.SWIM_RUN_SPEED : Stat.RUN_SPEED);
	}
	
	/**
	 * @return the WalkSpeed (base+modifier) of the Creature.
	 */
	public virtual double getWalkSpeed()
	{
		return getValue(_creature.isInsideZone(ZoneId.WATER) ? Stat.SWIM_WALK_SPEED : Stat.WALK_SPEED);
	}
	
	/**
	 * @return the SwimRunSpeed (base+modifier) of the Creature.
	 */
	public double getSwimRunSpeed()
	{
		return getValue(Stat.SWIM_RUN_SPEED);
	}
	
	/**
	 * @return the SwimWalkSpeed (base+modifier) of the Creature.
	 */
	public double getSwimWalkSpeed()
	{
		return getValue(Stat.SWIM_WALK_SPEED);
	}
	
	/**
	 * @return the RunSpeed (base+modifier) or WalkSpeed (base+modifier) of the Creature in function of the movement type.
	 */
	public virtual double getMoveSpeed()
	{
		if (_creature.isInsideZone(ZoneId.WATER))
		{
			return _creature.isRunning() ? getSwimRunSpeed() : getSwimWalkSpeed();
		}
		return _creature.isRunning() ? getRunSpeed() : getWalkSpeed();
	}
	
	/**
	 * @return the PAtk (base+modifier) of the Creature.
	 */
	public int getPAtk()
	{
		return (int) getValue(Stat.PHYSICAL_ATTACK);
	}
	
	public int getWeaponBonusPAtk()
	{
		return (int) getValue(Stat.WEAPON_BONUS_PHYSICAL_ATTACK);
	}
	
	/**
	 * @return the PAtk Speed (base+modifier) of the Creature in function of the Armour Expertise Penalty.
	 */
	public virtual int getPAtkSpd()
	{
		return (int) getValue(Stat.PHYSICAL_ATTACK_SPEED);
	}
	
	/**
	 * @return the PDef (base+modifier) of the Creature.
	 */
	public int getPDef()
	{
		return (int) getValue(Stat.PHYSICAL_DEFENCE);
	}
	
	/**
	 * @return the Physical Attack range (base+modifier) of the Creature.
	 */
	public int getPhysicalAttackRange()
	{
		return (int) getValue(Stat.PHYSICAL_ATTACK_RANGE);
	}
	
	public virtual int getPhysicalAttackRadius()
	{
		return 40;
	}
	
	public virtual int getPhysicalAttackAngle()
	{
		return 0;
	}
	
	/**
	 * @return the weapon reuse modifier.
	 */
	public double getWeaponReuseModifier()
	{
		return getValue(Stat.ATK_REUSE, 1);
	}
	
	/**
	 * @return the ShieldDef rate (base+modifier) of the Creature.
	 */
	public int getShldDef()
	{
		return (int) getValue(Stat.SHIELD_DEFENCE);
	}
	
	public virtual long getSp()
	{
		return _sp;
	}
	
	public virtual void setSp(long value)
	{
		_sp = value;
	}
	
	/**
	 * @return the STR of the Creature (base+modifier).
	 */
	public int getSTR()
	{
		return (int) getValue(Stat.STAT_STR);
	}
	
	/**
	 * @return the WIT of the Creature (base+modifier).
	 */
	public int getWIT()
	{
		return (int) getValue(Stat.STAT_WIT);
	}
	
	/**
	 * @param skill
	 * @return the mpConsume.
	 */
	public int getMpConsume(Skill skill)
	{
		if (skill == null)
		{
			return 1;
		}
		double mpConsume = skill.getMpConsume();
		double nextDanceMpCost = Math.Ceiling(skill.getMpConsume() / 2.0);
		if (skill.isDance() && Config.DANCE_CONSUME_ADDITIONAL_MP && (_creature != null) && (_creature.getDanceCount() > 0))
		{
			mpConsume += _creature.getDanceCount() * nextDanceMpCost;
		}
		return (int) (mpConsume * getMpConsumeTypeValue(skill.getMagicType()));
	}
	
	/**
	 * @param skill
	 * @return the mpInitialConsume.
	 */
	public int getMpInitialConsume(Skill skill)
	{
		if (skill == null)
		{
			return 1;
		}
		return skill.getMpInitialConsume();
	}
	
	public AttributeType getAttackElement()
	{
		Item weaponInstance = _creature.getActiveWeaponInstance();
		// 1st order - weapon element
		if ((weaponInstance != null) && (weaponInstance.getAttackAttributeType() != AttributeType.NONE))
		{
			return weaponInstance.getAttackAttributeType();
		}
		
		// temp fix starts
		int tempVal = 0;
		int[] stats =
		{
			getAttackElementValue(AttributeType.FIRE),
			getAttackElementValue(AttributeType.WATER),
			getAttackElementValue(AttributeType.WIND),
			getAttackElementValue(AttributeType.EARTH),
			getAttackElementValue(AttributeType.HOLY),
			getAttackElementValue(AttributeType.DARK)
		};
		
		AttributeType returnVal = AttributeType.NONE;
		for (byte x = 0; x < stats.Length; x++)
		{
			if (stats[x] > tempVal)
			{
				returnVal = (AttributeType)x;
				tempVal = stats[x];
			}
		}
		
		return returnVal;
	}
	
	public int getAttackElementValue(AttributeType attackAttribute)
	{
		switch (attackAttribute)
		{
			case AttributeType.FIRE:
			{
				return (int) getValue(Stat.FIRE_POWER);
			}
			case AttributeType.WATER:
			{
				return (int) getValue(Stat.WATER_POWER);
			}
			case AttributeType.WIND:
			{
				return (int) getValue(Stat.WIND_POWER);
			}
			case AttributeType.EARTH:
			{
				return (int) getValue(Stat.EARTH_POWER);
			}
			case AttributeType.HOLY:
			{
				return (int) getValue(Stat.HOLY_POWER);
			}
			case AttributeType.DARK:
			{
				return (int) getValue(Stat.DARK_POWER);
			}
			default:
			{
				return 0;
			}
		}
	}
	
	public int getDefenseElementValue(AttributeType defenseAttribute)
	{
		switch (defenseAttribute)
		{
			case AttributeType.FIRE:
			{
				return (int) getValue(Stat.FIRE_RES);
			}
			case AttributeType.WATER:
			{
				return (int) getValue(Stat.WATER_RES);
			}
			case AttributeType.WIND:
			{
				return (int) getValue(Stat.WIND_RES);
			}
			case AttributeType.EARTH:
			{
				return (int) getValue(Stat.EARTH_RES);
			}
			case AttributeType.HOLY:
			{
				return (int) getValue(Stat.HOLY_RES);
			}
			case AttributeType.DARK:
			{
				return (int) getValue(Stat.DARK_RES);
			}
			default:
			{
				return (int) getValue(Stat.BASE_ATTRIBUTE_RES);
			}
		}
	}
	
	public void mergeAttackTrait(TraitType traitType, float value)
	{
		lock (_lock)
		{
			_attackTraitValues[(int)traitType] += value;
			_attackTraits.add(traitType);
		}
	}
	
	public void removeAttackTrait(TraitType traitType, float value)
	{
		lock (_lock)
		{
			_attackTraitValues[(int)traitType] -= value;
			if (_attackTraitValues[(int)traitType] == 1.0f)
			{
				_attackTraits.remove(traitType);
			}
		}
	}
	
	public float getAttackTrait(TraitType traitType)
	{
		lock (_lock)
		{
			return _attackTraitValues[(int)traitType];
		}
	}
	
	public bool hasAttackTrait(TraitType traitType)
	{
		lock (_lock)
		{
			return _attackTraits.Contains(traitType);
		}
	}
	
	public void mergeDefenceTrait(TraitType traitType, float value)
	{
		lock (_lock)
		{
			_defenceTraitValues[(int)traitType] += value;
			_defenceTraits.add(traitType);
		}
	}
	
	public void removeDefenceTrait(TraitType traitType, float value)
	{
		lock (_lock)
		{
			_defenceTraitValues[(int)traitType] -= value;
			if (_defenceTraitValues[(int)traitType] == 0)
			{
				_defenceTraits.remove(traitType);
			}
		}
	}
	
	public float getDefenceTrait(TraitType traitType)
	{
		lock (_lock)
		{
			return _defenceTraitValues[(int)traitType];
		}
	}
	
	public bool hasDefenceTrait(TraitType traitType)
	{
		lock (_lock)
		{
			return _defenceTraits.Contains(traitType);
		}
	}
	
	public void mergeInvulnerableTrait(TraitType traitType)
	{
		lock (_lock)
		{
			_invulnerableTraits.add(traitType);
		}
	}
	
	public void removeInvulnerableTrait(TraitType traitType)
	{
		lock (_lock)
		{
			_invulnerableTraits.remove(traitType);
		}
	}
	
	public bool isInvulnerableTrait(TraitType traitType)
	{
		lock (_lock)
		{
			return _invulnerableTraits.Contains(traitType);
		}
	}
	
	/**
	 * Gets the maximum buff count.
	 * @return the maximum buff count
	 */
	public int getMaxBuffCount()
	{
		return _maxBuffCount;
	}
	
	/**
	 * Sets the maximum buff count.
	 * @param buffCount the buff count
	 */
	public void setMaxBuffCount(int value)
	{
		_maxBuffCount = value;
	}
	
	/**
	 * Merges the stat's value with the values within the map of adds
	 * @param stat
	 * @param value
	 */
	public void mergeAdd(Stat stat, double value)
	{
		_statsAdd.merge(stat, value, (x, y) => stat.GetInfo().AddFunction(x, y));
	}
	
	/**
	 * Merges the stat's value with the values within the map of muls
	 * @param stat
	 * @param value
	 */
	public void mergeMul(Stat stat, double value)
	{
		_statsMul.merge(stat, value, (x, y) => stat.GetInfo().MulFunction(x, y));
	}
	
	/**
	 * @param stat
	 * @return the add value
	 */
	public double getAdd(Stat stat)
	{
		return getAdd(stat, 0d);
	}
	
	/**
	 * @param stat
	 * @param defaultValue
	 * @return the add value
	 */
	public double getAdd(Stat stat, double defaultValue)
	{
		lock (_lock)
		{
			return _statsAdd.GetValueOrDefault(stat, defaultValue);
		}
	}
	
	/**
	 * Non blocking stat ADD getter.<br>
	 * WARNING: Only use with effect handlers!
	 * @param stat
	 * @return the add value
	 */
	public double getAddValue(Stat stat)
	{
		return getAddValue(stat, 0d);
	}
	
	/**
	 * Non blocking stat ADD getter.<br>
	 * WARNING: Only use with effect handlers!
	 * @param stat
	 * @param defaultValue
	 * @return the add value
	 */
	public double getAddValue(Stat stat, double defaultValue)
	{
		return _statsAdd.GetValueOrDefault(stat, defaultValue);
	}
	
	/**
	 * @param stat
	 * @return the mul value
	 */
	public double getMul(Stat stat)
	{
		return getMul(stat, 1);
	}
	
	/**
	 * @param stat
	 * @param defaultValue
	 * @return the mul value
	 */
	public double getMul(Stat stat, double defaultValue)
	{
		lock (_lock)
		{
			return _statsMul.GetValueOrDefault(stat, defaultValue);
		}
	}
	/**
	 * Non blocking stat MUL getter.<br>
	 * WARNING: Only use with effect handlers!
	 * @param stat
	 * @return the mul value
	 */
	public double getMulValue(Stat stat)
	{
		return getMulValue(stat, 1d);
	}
	
	/**
	 * Non blocking stat MUL getter.<br>
	 * WARNING: Only use with effect handlers!
	 * @param stat
	 * @param defaultValue
	 * @return the mul value
	 */
	public double getMulValue(Stat stat, double defaultValue)
	{
		return _statsMul.GetValueOrDefault(stat, defaultValue);
	}
	
	/**
	 * @param stat
	 * @param baseValue
	 * @return the final value of the stat
	 */
	public double getValue(Stat stat, double baseValue)
	{
		return _fixedValue.TryGetValue(stat, out double value) ? value : stat.DoFinalize(_creature, baseValue);
	}
	
	/**
	 * @param stat
	 * @return the final value of the stat
	 */
	public double getValue(Stat stat)
	{
		return _fixedValue.TryGetValue(stat, out double value) ? value : stat.DoFinalize(_creature, null);
	}
	
	protected void resetStats()
	{
		_statsAdd.clear();
		_statsMul.clear();
		_vampiricSum = 0;
		_mpVampiricSum = 0;
		
		// Initialize default values
		foreach (Stat stat in EnumUtil.GetValues<Stat>())
		{
			StatInfo? info = stat.GetInfo();
			if (info != null)
			{
				if (info.ResetAddValue != 0)
				{
					_statsAdd.put(stat, info.ResetAddValue);
				}

				if (info.ResetMulValue != 0)
				{
					_statsMul.put(stat, info.ResetMulValue);
				}
			}
		}
	}
	
	/**
	 * Locks and resets all stats and recalculates all
	 * @param broadcast
	 */
	public virtual void recalculateStats(bool broadcast)
	{
		// Copy old data before wiping it out.
		Map<Stat, double> adds = new();
		Map<Stat, double> muls = new();
		if (broadcast)
		{
			foreach (var kvp in _statsAdd)
				adds[kvp.Key] = kvp.Value;

			foreach (var kvp in _statsMul)
				muls[kvp.Key] = kvp.Value;
		}
		
		lock (_lock)
		{
			// Wipe all the data.
			resetStats();
			
			// Call pump to each effect.
			foreach (BuffInfo info in _creature.getEffectList().getPassives())
			{
				if (info.isInUse() && info.getSkill().checkConditions(SkillConditionScope.PASSIVE, _creature, _creature.getTarget()))
				{
					foreach (AbstractEffect effect in info.getEffects())
					{
						if (effect.canStart(info.getEffector(), info.getEffected(), info.getSkill()) && effect.canPump(info.getEffector(), info.getEffected(), info.getSkill()))
						{
							effect.pump(info.getEffected(), info.getSkill());
						}
					}
				}
			}
			foreach (BuffInfo info in _creature.getEffectList().getOptions())
			{
				if (info.isInUse())
				{
					foreach (AbstractEffect effect in info.getEffects())
					{
						if (effect.canStart(info.getEffector(), info.getEffected(), info.getSkill()) && effect.canPump(info.getEffector(), info.getEffected(), info.getSkill()))
						{
							effect.pump(info.getEffected(), info.getSkill());
						}
					}
				}
			}
			foreach (BuffInfo info in _creature.getEffectList().getEffects())
			{
				if (info.isInUse())
				{
					foreach (AbstractEffect effect in info.getEffects())
					{
						if (effect.canStart(info.getEffector(), info.getEffected(), info.getSkill()) && effect.canPump(info.getEffector(), info.getEffected(), info.getSkill()))
						{
							effect.pump(info.getEffected(), info.getSkill());
						}
					}
				}
			}
			
			// Pump for summon ABILITY_CHANGE abnormal type.
			if (_creature.isSummon() && (_creature.getActingPlayer() != null) && _creature.getActingPlayer().hasAbnormalType(AbnormalType.ABILITY_CHANGE))
			{
				foreach (BuffInfo info in _creature.getActingPlayer().getEffectList().getEffects())
				{
					if (info.isInUse() && info.isAbnormalType(AbnormalType.ABILITY_CHANGE))
					{
						foreach (AbstractEffect effect in info.getEffects())
						{
							if (effect.canStart(info.getEffector(), info.getEffected(), info.getSkill()) && effect.canPump(_creature, _creature, info.getSkill()))
							{
								effect.pump(_creature, info.getSkill());
							}
						}
					}
				}
			}
			
			// Merge with additional stats.
			foreach (StatHolder holder in _additionalAdd)
			{
				if (holder.verifyCondition(_creature))
				{
					mergeAdd(holder.getStat(), holder.getValue());
				}
			}
			foreach (StatHolder holder in _additionalMul)
			{
				if (holder.verifyCondition(_creature))
				{
					mergeMul(holder.getStat(), holder.getValue());
				}
			}
			_attackSpeedMultiplier = Formulas.calcAtkSpdMultiplier(_creature);
			_mAttackSpeedMultiplier = Formulas.calcMAtkSpdMultiplier(_creature);
		}
		
		// Notify recalculation to child classes.
		onRecalculateStats(broadcast);
		
		if (broadcast)
		{
			// Calculate the difference between old and new stats.
			Set<Stat> changed = new();
			double statAddResetValue;
			double statMulResetValue;
			double statAddValue;
			double statMulValue;
			double addsValue;
			double mulsValue;
			foreach (Stat stat in EnumUtil.GetValues<Stat>())
			{
				statAddResetValue = stat.GetInfo().ResetAddValue;
				statMulResetValue = stat.GetInfo().ResetMulValue;
				addsValue = adds.getOrDefault(stat, statAddResetValue);
				mulsValue = muls.getOrDefault(stat, statMulResetValue);
				statAddValue = _statsAdd.getOrDefault(stat, statAddResetValue);
				statMulValue = _statsMul.getOrDefault(stat, statMulResetValue);
				if (addsValue.Equals(statAddResetValue) || mulsValue.Equals(statMulResetValue) ||
				    !addsValue.Equals(statAddValue) || !mulsValue.Equals(statMulValue))
				{
					changed.add(stat);
				}
			}
			_creature.broadcastModifiedStats(changed);
		}
	}
	
	protected virtual void onRecalculateStats(bool broadcast)
	{
		// Check if Max HP/MP/CP is lower than current due to new stats.
		if (_creature.getCurrentCp() > getMaxCp())
		{
			_creature.setCurrentCp(getMaxCp());
		}
		if (_creature.getCurrentHp() > getMaxHp())
		{
			_creature.setCurrentHp(getMaxHp());
		}
		if (_creature.getCurrentMp() > getMaxMp())
		{
			_creature.setCurrentMp(getMaxMp());
		}
	}
	
	public double getPositionTypeValue(Stat stat, Position position)
	{
		Map<Position, double>? map = _positionStats.get(stat);
		return map != null && map.TryGetValue(position, out double value) ? value : 1;
	}
	
	public void mergePositionTypeValue(Stat stat, Position position, double value, Func<double, double, double> func)
	{
		_positionStats.computeIfAbsent(stat, key => new()).merge(position, value, func);
	}
	
	public double getMoveTypeValue(Stat stat, MoveType type)
	{
		Map<MoveType, double> map = _moveTypeStats.get(stat);
		return map != null && map.TryGetValue(type, out double value) ? value : 0;
	}
	
	public void mergeMoveTypeValue(Stat stat, MoveType type, double value)
	{
		_moveTypeStats.computeIfAbsent(stat, key => new()).merge(type, value, StatInfo.DefaultAddFunction);
	}
	
	public double getReuseTypeValue(int magicType)
	{
		return _reuseStat.GetValueOrDefault(magicType, 1);
	}
	
	public void mergeReuseTypeValue(int magicType, double value, Func<double, double, double> func)
	{
		_reuseStat.merge(magicType, value, func);
	}
	
	public double getMpConsumeTypeValue(int magicType)
	{
		return _mpConsumeStat.GetValueOrDefault(magicType, 1);
	}
	
	public void mergeMpConsumeTypeValue(int magicType, double value, Func<double, double, double> func)
	{
		_mpConsumeStat.merge(magicType, value, func);
	}
	
	public double getSkillEvasionTypeValue(int magicType)
	{
		LinkedList<double> skillEvasions = _skillEvasionStat.get(magicType);
		if ((skillEvasions != null) && !skillEvasions.isEmpty())
		{
			return skillEvasions.Last.Value;
		}
		
		return 0;
	}
	
	public void addSkillEvasionTypeValue(int magicType, double value)
	{
		_skillEvasionStat.computeIfAbsent(magicType, k => new()).AddLast(value);
	}
	
	public void removeSkillEvasionTypeValue(int magicType, double value)
	{
		_skillEvasionStat.computeIfPresent(magicType, (k, v) =>
		{
			v.Remove(value);
			return !v.isEmpty() ? v : null;
		});
	}
	
	public void addToVampiricSum(double sum)
	{
		_vampiricSum += sum;
	}
	
	public double getVampiricSum()
	{
		lock (_lock)
		{
			return _vampiricSum;
		}
	}
	
	public void addToMpVampiricSum(double sum)
	{
		_mpVampiricSum += sum;
	}
	
	public double getMpVampiricSum()
	{
		lock (_lock)
		{
			return _mpVampiricSum;
		}
	}
	
	/**
	 * Calculates the time required for this skill to be used again.
	 * @param skill the skill from which reuse time will be calculated.
	 * @return the time in milliseconds this skill is being under reuse.
	 */
	public virtual TimeSpan getReuseTime(Skill skill)
	{
		return (skill.isStaticReuse() || skill.isStatic())
			? skill.getReuseDelay()
			: skill.getReuseDelay() * getReuseTypeValue(skill.getMagicType());
	}
	
	/**
	 * Adds static value to the 'add' map of the stat everytime recalculation happens
	 * @param stat
	 * @param value
	 * @param condition
	 * @return
	 */
	public bool addAdditionalStat(Stat stat, double value, Func<Creature, StatHolder, bool> condition)
	{
		_additionalAdd.add(new StatHolder(stat, value, condition));
		return true;
	}
	
	/**
	 * Adds static value to the 'add' map of the stat everytime recalculation happens
	 * @param stat
	 * @param value
	 * @return
	 */
	public bool addAdditionalStat(Stat stat, double value)
	{
		_additionalAdd.add(new StatHolder(stat, value));
		return true;
	}
	
	/**
	 * @param stat
	 * @param value
	 * @return {@code true} if 'add' was removed, {@code false} in case there wasn't such stat and value
	 */
	public bool removeAddAdditionalStat(Stat stat, double value)
	{
		List<StatHolder> copy = _additionalAdd.ToList();
		foreach (StatHolder holder in copy)
		{
			if ((holder.getStat() == stat) && (holder.getValue() == value))
			{
				_additionalAdd.Remove(holder);
				return true;
			}
		}

		return false;
	}
	
	/**
	 * Adds static multiplier to the 'mul' map of the stat everytime recalculation happens
	 * @param stat
	 * @param value
	 * @param condition
	 * @return
	 */
	public bool mulAdditionalStat(Stat stat, double value, Func<Creature, StatHolder, bool> condition)
	{
		_additionalMul.add(new StatHolder(stat, value, condition));
		return true;
	}
	
	/**
	 * Adds static multiplier to the 'mul' map of the stat everytime recalculation happens
	 * @param stat
	 * @param value
	 * @return {@code true}
	 */
	public bool mulAdditionalStat(Stat stat, double value)
	{
		_additionalMul.add(new StatHolder(stat, value));
		return true;
	}
	
	/**
	 * @param stat
	 * @param value
	 * @return {@code true} if 'mul' was removed, {@code false} in case there wasn't such stat and value
	 */
	public bool removeMulAdditionalStat(Stat stat, double value)
	{
		List<StatHolder> copy = _additionalMul.ToList();
		foreach (StatHolder holder in copy)
		{
			if ((holder.getStat() == stat) && (holder.getValue() == value))
			{
				_additionalMul.Remove(holder);
				return true;
			}
		}

		return false;
	}
	
	/**
	 * @param stat
	 * @param value
	 * @return true if the there wasn't previously set fixed value, {@code false} otherwise
	 */
	public bool addFixedValue(Stat stat, double value)
	{
		bool result = !_fixedValue.ContainsKey(stat);
		_fixedValue[stat] = value;
		return result;
	}
	
	/**
	 * @param stat
	 * @return {@code true} if fixed value is removed, {@code false} otherwise
	 */
	public bool removeFixedValue(Stat stat)
	{
		return _fixedValue.TryRemove(stat, out _);
	}
}