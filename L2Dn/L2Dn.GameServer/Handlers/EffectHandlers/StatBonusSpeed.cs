using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class StatBonusSpeed: AbstractEffect
{
	private readonly Double _stat;
	private readonly Condition _armorTypeCondition;
	
	public StatBonusSpeed(StatSet @params)
	{
		_stat = (int)@params.getEnum("stat", BaseStat.DEX);
		
		ItemTypeMask armorTypesMask = ItemTypeMask.Zero;
		List<String> armorTypes = @params.getList<string>("armorType");
		if (armorTypes != null)
		{
			foreach (String armorType in armorTypes)
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
		
		_armorTypeCondition = armorTypesMask != ItemTypeMask.Zero ? new ConditionUsingItemType(armorTypesMask) : null;
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		if ((_armorTypeCondition == null) || _armorTypeCondition.test(effected, effected, skill))
		{
			effected.getStat().mergeAdd(Stat.STAT_BONUS_SPEED, _stat);
		}
	}
}