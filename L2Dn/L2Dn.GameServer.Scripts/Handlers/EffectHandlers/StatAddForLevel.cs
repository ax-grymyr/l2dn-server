using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class StatAddForLevel: AbstractEffect
{
	private readonly Stat _stat;
	private readonly Map<int, double> _values;
	
	public StatAddForLevel(StatSet @params)
	{
		_stat = @params.getEnum<Stat>("stat");
		
		List<int> amount = @params.getIntegerList("amount");
		_values = new();
		int index = 0;
		foreach (int level in @params.getIntegerList("level"))
		{
			_values.put(level, amount.get(index++));
		}
		
		if (@params.getEnum("mode", StatModifierType.DIFF) != StatModifierType.DIFF)
		{
			LOGGER.Warn(GetType().Name + " can only use DIFF mode.");
		}
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		Double amount = _values.get(effected.getLevel());
		if (amount != null)
		{
			effected.getStat().mergeAdd(_stat, amount);
		}
	}
}