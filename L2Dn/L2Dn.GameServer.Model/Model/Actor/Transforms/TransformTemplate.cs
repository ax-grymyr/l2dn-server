using System.Collections.Immutable;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Actor.Transforms;

public sealed class TransformTemplate
{
	private readonly float? _collisionRadius;
	private readonly float? _collisionHeight;
	private readonly WeaponType? _baseAttackType;
	private List<SkillHolder> _skills;
	private List<AdditionalSkillHolder> _additionalSkills;
	private List<AdditionalItemHolder> _additionalItems;
	private Map<int, int> _baseDefense;
	private Map<Stat, double> _baseStats;
	private ImmutableArray<int> _actions = ImmutableArray<int>.Empty;
	private readonly Map<int, TransformLevelData> _data = new();

	public TransformTemplate(StatSet set)
	{
		_collisionRadius = set.Contains("radius") ? set.getFloat("radius") : null;
		_collisionHeight = set.Contains("height") ? set.getFloat("height") : null;
		_baseAttackType = set.getEnum<WeaponType>("attackType", WeaponType.NONE);
		if (set.contains("range"))
		{
			addStats(Stat.PHYSICAL_ATTACK_RANGE, set.getDouble("range", 0));
		}
		if (set.contains("randomDamage"))
		{
			addStats(Stat.RANDOM_DAMAGE, set.getDouble("randomDamage", 0));
		}
		if (set.contains("walk"))
		{
			addStats(Stat.WALK_SPEED, set.getDouble("walk", 0));
		}
		if (set.contains("run"))
		{
			addStats(Stat.RUN_SPEED, set.getDouble("run", 0));
		}
		if (set.contains("waterWalk"))
		{
			addStats(Stat.SWIM_WALK_SPEED, set.getDouble("waterWalk", 0));
		}
		if (set.contains("waterRun"))
		{
			addStats(Stat.SWIM_RUN_SPEED, set.getDouble("waterRun", 0));
		}
		if (set.contains("flyWalk"))
		{
			addStats(Stat.FLY_WALK_SPEED, set.getDouble("flyWalk", 0));
		}
		if (set.contains("flyRun"))
		{
			addStats(Stat.FLY_RUN_SPEED, set.getDouble("flyRun", 0));
		}
		if (set.contains("pAtk"))
		{
			addStats(Stat.PHYSICAL_ATTACK, set.getDouble("pAtk", 0));
		}
		if (set.contains("mAtk"))
		{
			addStats(Stat.MAGIC_ATTACK, set.getDouble("mAtk", 0));
		}
		if (set.contains("range"))
		{
			addStats(Stat.PHYSICAL_ATTACK_RANGE, set.getInt("range", 0));
		}
		if (set.contains("attackSpeed"))
		{
			addStats(Stat.PHYSICAL_ATTACK_SPEED, set.getInt("attackSpeed", 0));
		}
		if (set.contains("critRate"))
		{
			addStats(Stat.CRITICAL_RATE, set.getInt("critRate", 0));
		}
		if (set.contains("str"))
		{
			addStats(Stat.STAT_STR, set.getInt("str", 0));
		}
		if (set.contains("int"))
		{
			addStats(Stat.STAT_INT, set.getInt("int", 0));
		}
		if (set.contains("con"))
		{
			addStats(Stat.STAT_CON, set.getInt("con", 0));
		}
		if (set.contains("dex"))
		{
			addStats(Stat.STAT_DEX, set.getInt("dex", 0));
		}
		if (set.contains("wit"))
		{
			addStats(Stat.STAT_WIT, set.getInt("wit", 0));
		}
		if (set.contains("men"))
		{
			addStats(Stat.STAT_MEN, set.getInt("men", 0));
		}

		if (set.contains("chest"))
		{
			addDefense(Inventory.PAPERDOLL_CHEST, set.getInt("chest", 0));
		}
		if (set.contains("legs"))
		{
			addDefense(Inventory.PAPERDOLL_LEGS, set.getInt("legs", 0));
		}
		if (set.contains("head"))
		{
			addDefense(Inventory.PAPERDOLL_HEAD, set.getInt("head", 0));
		}
		if (set.contains("feet"))
		{
			addDefense(Inventory.PAPERDOLL_FEET, set.getInt("feet", 0));
		}
		if (set.contains("gloves"))
		{
			addDefense(Inventory.PAPERDOLL_GLOVES, set.getInt("gloves", 0));
		}
		if (set.contains("underwear"))
		{
			addDefense(Inventory.PAPERDOLL_UNDER, set.getInt("underwear", 0));
		}
		if (set.contains("cloak"))
		{
			addDefense(Inventory.PAPERDOLL_CLOAK, set.getInt("cloak", 0));
		}
		if (set.contains("rear"))
		{
			addDefense(Inventory.PAPERDOLL_REAR, set.getInt("rear", 0));
		}
		if (set.contains("lear"))
		{
			addDefense(Inventory.PAPERDOLL_LEAR, set.getInt("lear", 0));
		}
		if (set.contains("rfinger"))
		{
			addDefense(Inventory.PAPERDOLL_RFINGER, set.getInt("rfinger", 0));
		}
		if (set.contains("lfinger"))
		{
			addDefense(Inventory.PAPERDOLL_LFINGER, set.getInt("lfinger", 0));
		}
		if (set.contains("neck"))
		{
			addDefense(Inventory.PAPERDOLL_NECK, set.getInt("neck", 0));
		}
	}

	private void addDefense(int type, int value)
	{
		if (_baseDefense == null)
		{
			_baseDefense = new();
		}

		_baseDefense.put(type, value);
	}

	/**
	 * @param type the slot type for where to search defense.
	 * @param defaultValue value to be used if no value for the type is found.
	 * @return altered value if its present, or {@code defaultValue} if no such type is assigned to this template.
	 */
	public int getDefense(int type, int defaultValue)
	{
		return _baseDefense == null ? defaultValue : _baseDefense.GetValueOrDefault(type, defaultValue);
	}

	private void addStats(Stat stat, double value)
	{
		if (_baseStats == null)
		{
			_baseStats = new();
		}
		_baseStats.put(stat, value);
	}

	/**
	 * @param stat the Stat value to search for.
	 * @param defaultValue value to be used if no such stat is found.
	 * @return altered stat if its present, or {@code defaultValue} if no such stat is assigned to this template.
	 */
	public double getStats(Stat stat, double defaultValue)
	{
		return _baseStats == null ? defaultValue : _baseStats.GetValueOrDefault(stat, defaultValue);
	}

	/**
	 * @return collision radius if set, {@code null} otherwise.
	 */
	public float? getCollisionRadius()
	{
		return _collisionRadius;
	}

	/**
	 * @return collision height if set, {@code null} otherwise.
	 */
	public float? getCollisionHeight()
	{
		return _collisionHeight;
	}

	public WeaponType? getBaseAttackType()
	{
		return _baseAttackType;
	}

	public void addSkill(SkillHolder holder)
	{
		if (_skills == null)
		{
			_skills = new();
		}
		_skills.Add(holder);
	}

	public List<SkillHolder> getSkills()
	{
		return _skills != null ? _skills : new();
	}

	public void addAdditionalSkill(AdditionalSkillHolder holder)
	{
		if (_additionalSkills == null)
		{
			_additionalSkills = new();
		}
		_additionalSkills.Add(holder);
	}

	public List<AdditionalSkillHolder> getAdditionalSkills()
	{
		return _additionalSkills != null ? _additionalSkills : new();
	}

	public void addAdditionalItem(AdditionalItemHolder holder)
	{
		if (_additionalItems == null)
		{
			_additionalItems = new();
		}
		_additionalItems.Add(holder);
	}

	public List<AdditionalItemHolder> getAdditionalItems()
	{
		return _additionalItems != null ? _additionalItems : new();
	}

	public void setBasicActionList(ImmutableArray<int> actions)
	{
		_actions = actions;
	}

	public ImmutableArray<int> getBasicActionList()
	{
		return _actions;
	}

	public bool hasBasicActionList()
	{
		return !_actions.IsDefaultOrEmpty;
	}

	public void addLevelData(TransformLevelData data)
	{
		_data.put(data.getLevel(), data);
	}

	public TransformLevelData getData(int level)
	{
		return _data.get(level);
	}
}