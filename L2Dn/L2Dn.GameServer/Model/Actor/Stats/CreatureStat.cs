using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Utilities;

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
	private readonly Map<Stat, Map<MoveType, Double>> _moveTypeStats = new();
	private readonly Map<int, Double> _reuseStat = new();
	private readonly Map<int, Double> _mpConsumeStat = new();
	private readonly Map<int, LinkedList<Double>> _skillEvasionStat = new();
	private readonly Map<Stat, Map<Position, Double>> _positionStats = new();
	private readonly Deque<StatHolder> _additionalAdd = new();
	private readonly Deque<StatHolder> _additionalMul = new();
	private readonly Map<Stat, Double> _fixedValue = new();
	
	private readonly float[] _attackTraitValues = new float[Enum.GetValues<TraitType>().Length];
	private readonly float[] _defenceTraitValues = new float[Enum.GetValues<TraitType>().Length];
	private readonly Set<TraitType> _attackTraits = new();
	private readonly Set<TraitType> _defenceTraits = new();
	private readonly Set<TraitType> _invulnerableTraits = new();
	
	/** Values to be recalculated after every stat update */
	private double _attackSpeedMultiplier = 1;
	private double _mAttackSpeedMultiplier = 1;
	
	private readonly ReentrantReadWriteLock _lock = new ReentrantReadWriteLock();
	
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
		double nextDanceMpCost = Math.ceil(skill.getMpConsume() / 2.0);
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
				returnVal = AttributeType.findByClientId(x);
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
		_lock.readLock().@lock();
		try
		{
			_attackTraitValues[(int)traitType] += value;
			_attackTraits.add(traitType);
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public void removeAttackTrait(TraitType traitType, float value)
	{
		_lock.readLock().@lock();
		try
		{
			_attackTraitValues[(int)traitType] -= value;
			if (_attackTraitValues[(int)traitType] == 1.0f)
			{
				_attackTraits.remove(traitType);
			}
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public float getAttackTrait(TraitType traitType)
	{
		_lock.readLock().@lock();
		try
		{
			return _attackTraitValues[(int)traitType];
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public bool hasAttackTrait(TraitType traitType)
	{
		_lock.readLock().@lock();
		try
		{
			return _attackTraits.Contains(traitType);
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public void mergeDefenceTrait(TraitType traitType, float value)
	{
		_lock.readLock().@lock();
		try
		{
			_defenceTraitValues[(int)traitType] += value;
			_defenceTraits.add(traitType);
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public void removeDefenceTrait(TraitType traitType, float value)
	{
		_lock.readLock().@lock();
		try
		{
			_defenceTraitValues[(int)traitType] -= value;
			if (_defenceTraitValues[(int)traitType] == 0)
			{
				_defenceTraits.remove(traitType);
			}
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public float getDefenceTrait(TraitType traitType)
	{
		_lock.readLock().@lock();
		try
		{
			return _defenceTraitValues[(int)traitType];
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public bool hasDefenceTrait(TraitType traitType)
	{
		_lock.readLock().@lock();
		try
		{
			return _defenceTraits.Contains(traitType);
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public void mergeInvulnerableTrait(TraitType traitType)
	{
		_lock.readLock().@lock();
		try
		{
			_invulnerableTraits.add(traitType);
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public void removeInvulnerableTrait(TraitType traitType)
	{
		_lock.readLock().@lock();
		try
		{
			_invulnerableTraits.remove(traitType);
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public bool isInvulnerableTrait(TraitType traitType)
	{
		_lock.readLock().@lock();
		try
		{
			return _invulnerableTraits.Contains(traitType);
		}
		finally
		{
			_lock.readLock().unlock();
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
	public void setMaxBuffCount(int buffCount)
	{
		_maxBuffCount = buffCount;
	}
	
	/**
	 * Merges the stat's value with the values within the map of adds
	 * @param stat
	 * @param value
	 */
	public void mergeAdd(Stat stat, Double value)
	{
		_statsAdd.merge(stat, value, stat::functionAdd);
	}
	
	/**
	 * Merges the stat's value with the values within the map of muls
	 * @param stat
	 * @param value
	 */
	public void mergeMul(Stat stat, Double value)
	{
		_statsMul.merge(stat, value, stat::functionMul);
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
		_lock.readLock().@lock();
		try
		{
			double val = _statsAdd.get(stat);
			return val != null ? val.doubleValue() : defaultValue;
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	/**
	 * @param stat
	 * @return the mul value
	 */
	public double getMul(Stat stat)
	{
		return getMul(stat, 1d);
	}
	
	/**
	 * @param stat
	 * @param defaultValue
	 * @return the mul value
	 */
	public double getMul(Stat stat, double defaultValue)
	{
		_lock.readLock().@lock();
		try
		{
			Double val = _statsMul.get(stat);
			return val != null ? val.doubleValue() : defaultValue;
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	/**
	 * @param stat
	 * @param baseValue
	 * @return the final value of the stat
	 */
	public double getValue(Stat stat, double baseValue)
	{
		Double val = _fixedValue.get(stat);
		return val != null ? val.doubleValue() : stat.finalize(_creature, OptionalDouble.of(baseValue));
	}
	
	/**
	 * @param stat
	 * @return the final value of the stat
	 */
	public double getValue(Stat stat)
	{
		Double val = _fixedValue.get(stat);
		return val != null ? val.doubleValue() : stat.finalize(_creature, OptionalDouble.empty());
	}
	
	protected void resetStats()
	{
		_statsAdd.clear();
		_statsMul.clear();
		_vampiricSum = 0;
		_mpVampiricSum = 0;
		
		// Initialize default values
		foreach (Stat stat in Stat.values())
		{
			if (stat.getResetAddValue() != 0)
			{
				_statsAdd.put(stat, stat.getResetAddValue());
			}
			if (stat.getResetMulValue() != 0)
			{
				_statsMul.put(stat, stat.getResetMulValue());
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
		Map<Stat, Double> adds = !broadcast ? Collections.emptyMap() : new(_statsAdd);
		Map<Stat, Double> muls = !broadcast ? Collections.emptyMap() : new(_statsMul);
		
		_lock.writeLock().lock();
		
		try
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
		finally
		{
			_lock.writeLock().unlock();
		}
		
		// Notify recalculation to child classes.
		onRecalculateStats(broadcast);
		
		if (broadcast)
		{
			// Calculate the difference between old and new stats
			Set<Stat> changed = new();
			foreach (Stat stat in Enum.GetValues<Stat>())
			{
				if (_statsAdd.getOrDefault(stat, stat.getResetAddValue()).equals(adds.getOrDefault(stat, stat.getResetAddValue())) || _statsMul.getOrDefault(stat, stat.getResetMulValue()).equals(muls.getOrDefault(stat, stat.getResetMulValue())))
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
		Map<Position, Double> map = _positionStats.get(stat);
		if (map != null)
		{
			Double val = map.get(position);
			if (val != null)
			{
				return val.doubleValue();
			}
		}
		return 1d;
	}
	
	public void mergePositionTypeValue(Stat stat, Position position, double value, BiFunction<? super Double, ? super Double, ? extends Double> func)
	{
		_positionStats.computeIfAbsent(stat, key => new()).merge(position, value, func);
	}
	
	public double getMoveTypeValue(Stat stat, MoveType type)
	{
		Map<MoveType, Double> map = _moveTypeStats.get(stat);
		if (map != null)
		{
			Double val = map.get(type);
			if (val != null)
			{
				return val.doubleValue();
			}
		}
		return 0d;
	}
	
	public void mergeMoveTypeValue(Stat stat, MoveType type, double value)
	{
		_moveTypeStats.computeIfAbsent(stat, key => new()).merge(type, value, MathUtil::add);
	}
	
	public double getReuseTypeValue(int magicType)
	{
		Double val = _reuseStat.get(magicType);
		return val != null ? val.doubleValue() : 1d;
	}
	
	public void mergeReuseTypeValue(int magicType, double value, BiFunction<? super Double, ? super Double, ? extends Double> func)
	{
		_reuseStat.merge(magicType, value, func);
	}
	
	public double getMpConsumeTypeValue(int magicType)
	{
		Double val = _mpConsumeStat.get(magicType);
		return val != null ? val.doubleValue() : 1d;
	}
	
	public void mergeMpConsumeTypeValue(int magicType, double value, BiFunction<? super Double, ? super Double, ? extends Double> func)
	{
		_mpConsumeStat.merge(magicType, value, func);
	}
	
	public double getSkillEvasionTypeValue(int magicType)
	{
		LinkedList<Double> skillEvasions = _skillEvasionStat.get(magicType);
		if ((skillEvasions != null) && !skillEvasions.isEmpty())
		{
			return skillEvasions.peekLast().doubleValue();
		}
		return 0d;
	}
	
	public void addSkillEvasionTypeValue(int magicType, double value)
	{
		_skillEvasionStat.computeIfAbsent(magicType, k => new LinkedList<>()).add(value);
	}
	
	public void removeSkillEvasionTypeValue(int magicType, double value)
	{
		_skillEvasionStat.computeIfPresent(magicType, (k, v) =>
		{
			v.remove(value);
			return !v.isEmpty() ? v : null;
		});
	}
	
	public void addToVampiricSum(double sum)
	{
		_vampiricSum += sum;
	}
	
	public double getVampiricSum()
	{
		_lock.readLock().@lock();
		try
		{
			return _vampiricSum;
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	public void addToMpVampiricSum(double sum)
	{
		_mpVampiricSum += sum;
	}
	
	public double getMpVampiricSum()
	{
		_lock.readLock().@lock();
		try
		{
			return _mpVampiricSum;
		}
		finally
		{
			_lock.readLock().unlock();
		}
	}
	
	/**
	 * Calculates the time required for this skill to be used again.
	 * @param skill the skill from which reuse time will be calculated.
	 * @return the time in milliseconds this skill is being under reuse.
	 */
	public int getReuseTime(Skill skill)
	{
		return (skill.isStaticReuse() || skill.isStatic()) ? skill.getReuseDelay() : (int) (skill.getReuseDelay() * getReuseTypeValue(skill.getMagicType()));
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
		return _additionalAdd.add(new StatHolder(stat, value, condition));
	}
	
	/**
	 * Adds static value to the 'add' map of the stat everytime recalculation happens
	 * @param stat
	 * @param value
	 * @return
	 */
	public bool addAdditionalStat(Stat stat, double value)
	{
		return _additionalAdd.add(new StatHolder(stat, value));
	}
	
	/**
	 * @param stat
	 * @param value
	 * @return {@code true} if 'add' was removed, {@code false} in case there wasn't such stat and value
	 */
	public bool removeAddAdditionalStat(Stat stat, double value)
	{
		Iterator<StatHolder> it = _additionalAdd.iterator();
		while (it.hasNext())
		{
			StatHolder holder = it.next();
			if ((holder.getStat() == stat) && (holder.getValue() == value))
			{
				it.remove();
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
		return _additionalMul.add(new StatHolder(stat, value, condition));
	}
	
	/**
	 * Adds static multiplier to the 'mul' map of the stat everytime recalculation happens
	 * @param stat
	 * @param value
	 * @return {@code true}
	 */
	public bool mulAdditionalStat(Stat stat, double value)
	{
		return _additionalMul.add(new StatHolder(stat, value));
	}
	
	/**
	 * @param stat
	 * @param value
	 * @return {@code true} if 'mul' was removed, {@code false} in case there wasn't such stat and value
	 */
	public bool removeMulAdditionalStat(Stat stat, double value)
	{
		Iterator<StatHolder> it = _additionalMul.iterator();
		while (it.hasNext())
		{
			StatHolder holder = it.next();
			if ((holder.getStat() == stat) && (holder.getValue() == value))
			{
				it.remove();
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
	public bool addFixedValue(Stat stat, Double value)
	{
		return _fixedValue.put(stat, value) == null;
	}
	
	/**
	 * @param stat
	 * @return {@code true} if fixed value is removed, {@code false} otherwise
	 */
	public bool removeFixedValue(Stat stat)
	{
		return _fixedValue.remove(stat) != null;
	}
}
