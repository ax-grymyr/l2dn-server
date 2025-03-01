using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class Speed: AbstractEffect
{
	private readonly double _amount;
	private readonly StatModifierType _mode;
	private ConditionUsingItemType? _condition;

	public Speed(StatSet @params)
	{
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
					throw new ArgumentException("weaponType should contain WeaponType enum value but found " + weaponType, e);
				}
			}
		}
		if (weaponTypesMask != ItemTypeMask.Zero)
		{
			_condition = new ConditionUsingItemType(weaponTypesMask);
		}
	}

	public override void pump(Creature effected, Skill skill)
	{
		if (_condition == null || _condition.test(effected, effected, skill))
		{
			switch (_mode)
			{
				case StatModifierType.DIFF:
				{
					effected.getStat().mergeAdd(Stat.RUN_SPEED, _amount);
					effected.getStat().mergeAdd(Stat.WALK_SPEED, _amount);
					effected.getStat().mergeAdd(Stat.SWIM_RUN_SPEED, _amount);
					effected.getStat().mergeAdd(Stat.SWIM_WALK_SPEED, _amount);
					effected.getStat().mergeAdd(Stat.FLY_RUN_SPEED, _amount);
					effected.getStat().mergeAdd(Stat.FLY_WALK_SPEED, _amount);
					break;
				}
				case StatModifierType.PER:
				{
					effected.getStat().mergeMul(Stat.RUN_SPEED, _amount / 100 + 1);
					effected.getStat().mergeMul(Stat.WALK_SPEED, _amount / 100 + 1);
					effected.getStat().mergeMul(Stat.SWIM_RUN_SPEED, _amount / 100 + 1);
					effected.getStat().mergeMul(Stat.SWIM_WALK_SPEED, _amount / 100 + 1);
					effected.getStat().mergeMul(Stat.FLY_RUN_SPEED, _amount / 100 + 1);
					effected.getStat().mergeMul(Stat.FLY_WALK_SPEED, _amount / 100 + 1);
					break;
				}
			}
		}
	}
}