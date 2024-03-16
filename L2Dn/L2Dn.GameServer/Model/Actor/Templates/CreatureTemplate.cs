using L2Dn.Events;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Templates;

/**
 * Character template.
 * @author Zoey76
 */
public class CreatureTemplate
{
	// BaseStats
	private WeaponType _baseAttackType;
	
	/** For client info use {@link #_fCollisionRadius} */
	private int _collisionRadius;
	/** For client info use {@link #_fCollisionHeight} */
	private int _collisionHeight;
	
	private float _fCollisionRadius;
	private float _fCollisionHeight;
	
	protected readonly Map<Stat, double> _baseValues = new();
	
	/** The creature's race. */
	private Race _race;

	private readonly EventContainer _eventContainer;
	
	public CreatureTemplate(StatSet set)
	{
		this.set(set);
		_eventContainer = CreateEventContainer();
	}

	public EventContainer Events => _eventContainer;
	
	public virtual void set(StatSet set)
	{
		// Base stats
		_baseValues.put(Stat.STAT_STR, set.getDouble("baseSTR", 0));
		_baseValues.put(Stat.STAT_CON, set.getDouble("baseCON", 0));
		_baseValues.put(Stat.STAT_DEX, set.getDouble("baseDEX", 0));
		_baseValues.put(Stat.STAT_INT, set.getDouble("baseINT", 0));
		_baseValues.put(Stat.STAT_WIT, set.getDouble("baseWIT", 0));
		_baseValues.put(Stat.STAT_MEN, set.getDouble("baseMEN", 0));
		
		// Max HP/MP/CP
		_baseValues.put(Stat.MAX_HP, set.getDouble("baseHpMax", 0));
		_baseValues.put(Stat.MAX_MP, set.getDouble("baseMpMax", 0));
		_baseValues.put(Stat.MAX_CP, set.getDouble("baseCpMax", 0));
		
		// Regenerate HP/MP/CP
		_baseValues.put(Stat.REGENERATE_HP_RATE, set.getDouble("baseHpReg", 0));
		_baseValues.put(Stat.REGENERATE_MP_RATE, set.getDouble("baseMpReg", 0));
		_baseValues.put(Stat.REGENERATE_CP_RATE, set.getDouble("baseCpReg", 0));
		
		// Attack and Defense
		_baseValues.put(Stat.PHYSICAL_ATTACK, set.getDouble("basePAtk", 0));
		_baseValues.put(Stat.MAGIC_ATTACK, set.getDouble("baseMAtk", 0));
		_baseValues.put(Stat.PHYSICAL_DEFENCE, set.getDouble("basePDef", 0));
		_baseValues.put(Stat.MAGICAL_DEFENCE, set.getDouble("baseMDef", 0));
		
		// Attack speed
		_baseValues.put(Stat.PHYSICAL_ATTACK_SPEED, set.getDouble("basePAtkSpd", 300));
		_baseValues.put(Stat.MAGIC_ATTACK_SPEED, set.getDouble("baseMAtkSpd", 333));
		
		// Misc
		_baseValues.put(Stat.SHIELD_DEFENCE, set.getDouble("baseShldDef", 0));
		_baseValues.put(Stat.PHYSICAL_ATTACK_RANGE, set.getDouble("baseAtkRange", 40));
		_baseValues.put(Stat.RANDOM_DAMAGE, set.getDouble("baseRndDam", 0));
		
		// Shield and critical rates
		_baseValues.put(Stat.SHIELD_DEFENCE_RATE, set.getDouble("baseShldRate", 0));
		_baseValues.put(Stat.CRITICAL_RATE, set.getDouble("baseCritRate", 4));
		_baseValues.put(Stat.MAGIC_CRITICAL_RATE, set.getDouble("baseMCritRate", 5));
		
		// Breath under water
		_baseValues.put(Stat.BREATH, set.getDouble("baseBreath", 100));
		
		// Elemental Attributes
		// Attack
		_baseValues.put(Stat.FIRE_POWER, set.getDouble("baseFire", 0));
		_baseValues.put(Stat.WIND_POWER, set.getDouble("baseWind", 0));
		_baseValues.put(Stat.WATER_POWER, set.getDouble("baseWater", 0));
		_baseValues.put(Stat.EARTH_POWER, set.getDouble("baseEarth", 0));
		_baseValues.put(Stat.HOLY_POWER, set.getDouble("baseHoly", 0));
		_baseValues.put(Stat.DARK_POWER, set.getDouble("baseDark", 0));
		
		// Defense
		_baseValues.put(Stat.FIRE_RES, set.getDouble("baseFireRes", 0));
		_baseValues.put(Stat.WIND_RES, set.getDouble("baseWindRes", 0));
		_baseValues.put(Stat.WATER_RES, set.getDouble("baseWaterRes", 0));
		_baseValues.put(Stat.EARTH_RES, set.getDouble("baseEarthRes", 0));
		_baseValues.put(Stat.HOLY_RES, set.getDouble("baseHolyRes", 0));
		_baseValues.put(Stat.DARK_RES, set.getDouble("baseDarkRes", 0));
		_baseValues.put(Stat.BASE_ATTRIBUTE_RES, set.getDouble("baseElementRes", 0));
		
		// Geometry
		_fCollisionHeight = set.getFloat("collision_height", 0);
		_fCollisionRadius = set.getFloat("collision_radius", 0);
		_collisionRadius = (int) _fCollisionRadius;
		_collisionHeight = (int) _fCollisionHeight;
		
		// Speed
		_baseValues.put(Stat.RUN_SPEED, set.getDouble("baseRunSpd", 120));
		_baseValues.put(Stat.WALK_SPEED, set.getDouble("baseWalkSpd", 50));
		
		// Swimming
		_baseValues.put(Stat.SWIM_RUN_SPEED, set.getDouble("baseSwimRunSpd", 120));
		_baseValues.put(Stat.SWIM_WALK_SPEED, set.getDouble("baseSwimWalkSpd", 50));
		
		// Flying
		_baseValues.put(Stat.FLY_RUN_SPEED, set.getDouble("baseFlyRunSpd", 120));
		_baseValues.put(Stat.FLY_WALK_SPEED, set.getDouble("baseFlyWalkSpd", 50));
		
		// Attack type
		_baseAttackType = set.getEnum("baseAtkType", WeaponType.FIST);
		
		// Basic property
		_baseValues.put(Stat.ABNORMAL_RESIST_PHYSICAL, set.getDouble("physicalAbnormalResist", 10));
		_baseValues.put(Stat.ABNORMAL_RESIST_MAGICAL, set.getDouble("magicAbnormalResist", 10));
	}
	
	/**
	 * @return the baseSTR
	 */
	public int getBaseSTR()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.STAT_STR, 0);
	}
	
	/**
	 * @return the baseCON
	 */
	public int getBaseCON()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.STAT_CON, 0);
	}
	
	/**
	 * @return the baseDEX
	 */
	public int getBaseDEX()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.STAT_DEX, 0);
	}
	
	/**
	 * @return the baseINT
	 */
	public int getBaseINT()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.STAT_INT, 0);
	}
	
	/**
	 * @return the baseWIT
	 */
	public int getBaseWIT()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.STAT_WIT, 0);
	}
	
	/**
	 * @return the baseMEN
	 */
	public int getBaseMEN()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.STAT_MEN, 0);
	}
	
	/**
	 * @return the baseHpMax
	 */
	public float getBaseHpMax()
	{
		return (float)_baseValues.GetValueOrDefault(Stat.MAX_HP, 0);
	}
	
	/**
	 * @return the baseCpMax
	 */
	public float getBaseCpMax()
	{
		return (float)_baseValues.GetValueOrDefault(Stat.MAX_CP, 0);
	}
	
	/**
	 * @return the baseMpMax
	 */
	public float getBaseMpMax()
	{
		return (float)_baseValues.GetValueOrDefault(Stat.MAX_MP, 0);
	}
	
	/**
	 * @return the baseHpReg
	 */
	public float getBaseHpReg()
	{
		return (float)_baseValues.GetValueOrDefault(Stat.REGENERATE_HP_RATE, 0);
	}
	
	/**
	 * @return the baseMpReg
	 */
	public float getBaseMpReg()
	{
		return (float)_baseValues.GetValueOrDefault(Stat.REGENERATE_MP_RATE, 0);
	}
	
	/**
	 * @return the _baseFire
	 */
	public int getBaseFire()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.FIRE_POWER, 0);
	}
	
	/**
	 * @return the _baseWind
	 */
	public int getBaseWind()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.WIND_POWER, 0);
	}
	
	/**
	 * @return the _baseWater
	 */
	public int getBaseWater()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.WATER_POWER, 0);
	}
	
	/**
	 * @return the _baseEarth
	 */
	public int getBaseEarth()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.EARTH_POWER, 0);
	}
	
	/**
	 * @return the _baseHoly
	 */
	public int getBaseHoly()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.HOLY_POWER, 0);
	}
	
	/**
	 * @return the _baseDark
	 */
	public int getBaseDark()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.DARK_POWER, 0);
	}
	
	/**
	 * @return the _baseFireRes
	 */
	public double getBaseFireRes()
	{
		return _baseValues.GetValueOrDefault(Stat.FIRE_RES, 0);
	}
	
	/**
	 * @return the _baseWindRes
	 */
	public double getBaseWindRes()
	{
		return _baseValues.GetValueOrDefault(Stat.WIND_RES, 0);
	}
	
	/**
	 * @return the _baseWaterRes
	 */
	public double getBaseWaterRes()
	{
		return _baseValues.GetValueOrDefault(Stat.WATER_RES, 0);
	}
	
	/**
	 * @return the _baseEarthRes
	 */
	public double getBaseEarthRes()
	{
		return _baseValues.GetValueOrDefault(Stat.EARTH_RES, 0);
	}
	
	/**
	 * @return the _baseHolyRes
	 */
	public double getBaseHolyRes()
	{
		return _baseValues.GetValueOrDefault(Stat.HOLY_RES, 0);
	}
	
	/**
	 * @return the _baseDarkRes
	 */
	public double getBaseDarkRes()
	{
		return _baseValues.GetValueOrDefault(Stat.DARK_RES, 0);
	}
	
	/**
	 * @return the _baseElementRes
	 */
	public double getBaseElementRes()
	{
		return _baseValues.GetValueOrDefault(Stat.BASE_ATTRIBUTE_RES, 0);
	}
	
	/**
	 * @return the basePAtk
	 */
	public virtual int getBasePAtk()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.PHYSICAL_ATTACK, 0);
	}
	
	/**
	 * @return the baseMAtk
	 */
	public virtual int getBaseMAtk()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.MAGIC_ATTACK, 0);
	}
	
	/**
	 * @return the basePDef
	 */
	public int getBasePDef()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.PHYSICAL_DEFENCE, 0);
	}
	
	/**
	 * @return the baseMDef
	 */
	public int getBaseMDef()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.MAGICAL_DEFENCE, 0);
	}
	
	/**
	 * @return the basePAtkSpd
	 */
	public int getBasePAtkSpd()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.PHYSICAL_ATTACK_SPEED, 0);
	}
	
	/**
	 * @return the baseMAtkSpd
	 */
	public int getBaseMAtkSpd()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.MAGIC_ATTACK_SPEED, 0);
	}
	
	/**
	 * @return the random damage
	 */
	public int getRandomDamage()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.RANDOM_DAMAGE, 0);
	}
	
	/**
	 * @return the baseShldDef
	 */
	public int getBaseShldDef()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.SHIELD_DEFENCE, 0);
	}
	
	/**
	 * @return the baseShldRate
	 */
	public int getBaseShldRate()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.SHIELD_DEFENCE_RATE, 0);
	}
	
	/**
	 * @return the baseCritRate
	 */
	public int getBaseCritRate()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.CRITICAL_RATE, 0);
	}
	
	/**
	 * @return the baseMCritRate
	 */
	public int getBaseMCritRate()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.MAGIC_CRITICAL_RATE, 0);
	}
	
	/**
	 * @return the baseBreath
	 */
	public int getBaseBreath()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.BREATH, 0);
	}
	
	/**
	 * @return base abnormal resist by basic property type.
	 */
	public int getBaseAbnormalResistPhysical()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.ABNORMAL_RESIST_PHYSICAL, 0);
	}
	
	/**
	 * @return base abnormal resist by basic property type.
	 */
	public int getBaseAbnormalResistMagical()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.ABNORMAL_RESIST_MAGICAL, 0);
	}
	
	/**
	 * @return the collisionRadius
	 */
	public int getCollisionRadius()
	{
		return _collisionRadius;
	}
	
	/**
	 * @return the collisionHeight
	 */
	public int getCollisionHeight()
	{
		return _collisionHeight;
	}
	
	/**
	 * @return the fCollisionRadius
	 */
	public float getFCollisionRadius()
	{
		return _fCollisionRadius;
	}
	
	/**
	 * @return the fCollisionHeight
	 */
	public float getFCollisionHeight()
	{
		return _fCollisionHeight;
	}
	
	/**
	 * @return the base attack type (Sword, Fist, Blunt, etc..)
	 */
	public WeaponType getBaseAttackType()
	{
		return _baseAttackType;
	}
	
	/**
	 * Sets base attack type.
	 * @param type
	 */
	public void setBaseAttackType(WeaponType type)
	{
		_baseAttackType = type;
	}
	
	/**
	 * @return the baseAtkRange
	 */
	public int getBaseAttackRange()
	{
		return (int)_baseValues.GetValueOrDefault(Stat.PHYSICAL_ATTACK_RANGE, 0);
	}
	
	/**
	 * Overridden in NpcTemplate
	 * @return the characters skills
	 */
	public virtual Map<int, Skill> getSkills()
	{
		return new();
	}
	
	/**
	 * Gets the craeture's race.
	 * @return the race
	 */
	public Race getRace()
	{
		return _race;
	}
	
	/**
	 * Sets the creature's race.
	 * @param race the race
	 */
	public void setRace(Race race)
	{
		_race = race;
	}
	
	/**
	 * @param stat
	 * @param defaultValue
	 * @return
	 */
	public double getBaseValue(Stat stat, double defaultValue)
	{
		return _baseValues.GetValueOrDefault(stat, defaultValue);
	}

	public virtual Creature CreateInstance()
	{
		throw new NotImplementedException();
	}

	protected virtual EventContainer CreateEventContainer()
	{
		return new EventContainer("Creature template", GlobalEvents.Global);
	}
}