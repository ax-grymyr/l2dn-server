using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw, Mobius
 */
public abstract class AbstractStatEffect: AbstractEffect
{
	protected Stat _addStat;
	protected Stat _mulStat;
	protected double _amount;
	protected StatModifierType _mode;
	protected List<Condition> _conditions = [];

	public AbstractStatEffect(StatSet @params, Stat stat): this(@params, stat, stat)
	{
	}

	public AbstractStatEffect(StatSet @params, Stat mulStat, Stat addStat)
	{
		_addStat = addStat;
		_mulStat = mulStat;
		_amount = @params.getDouble("amount", 0);
		_mode = @params.getEnum("mode", StatModifierType.DIFF);

		ItemTypeMask weaponTypesMask = ItemTypeMask.Zero;
		List<string>? weaponTypes = @params.getList<string>("weaponType");
		if (weaponTypes != null)
		{
			foreach (string weaponType in weaponTypes)
			{
				try
				{
					weaponTypesMask |= Enum.Parse<WeaponType>(weaponType);
				}
				catch (ArgumentException e)
				{
					throw new ArgumentException("weaponType should contain WeaponType enum value but found " +
					                            weaponType, e);
				}
			}
		}

		ItemTypeMask armorTypesMask = ItemTypeMask.Zero;
		List<string>? armorTypes = @params.getList<string>("armorType");
		if (armorTypes != null)
		{
			foreach (string armorType in armorTypes)
			{
				try
				{
					armorTypesMask |= Enum.Parse<ArmorType>(armorType);
				}
				catch (ArgumentException e)
				{
					throw new ArgumentException("armorTypes should contain ArmorType enum value but found " + armorType, e);
				}
			}
		}

		if (weaponTypesMask != ItemTypeMask.Zero)
		{
			_conditions.Add(new ConditionUsingItemType(weaponTypesMask));
		}

		if (armorTypesMask != ItemTypeMask.Zero)
		{
			_conditions.Add(new ConditionUsingItemType(armorTypesMask));
		}

		if (@params.contains("inCombat"))
		{
			_conditions.Add(new ConditionPlayerIsInCombat(@params.getBoolean("inCombat")));
		}

		if (@params.contains("magicWeapon"))
		{
			_conditions.Add(new ConditionUsingMagicWeapon(@params.getBoolean("magicWeapon")));
		}

		if (@params.contains("twoHandWeapon"))
		{
			_conditions.Add(new ConditionUsingTwoHandWeapon(@params.getBoolean("twoHandWeapon")));
		}
	}

	public override void pump(Creature effected, Skill skill)
	{
		foreach (Condition cond in _conditions)
		{
			if (!cond.test(effected, effected, skill))
			{
				return;
			}
		}

		switch (_mode)
		{
			case StatModifierType.DIFF:
			{
				effected.getStat().mergeAdd(_addStat, _amount);
				break;
			}
			case StatModifierType.PER:
			{
				effected.getStat().mergeMul(_mulStat, _amount / 100 + 1);
				break;
			}
		}
	}
}