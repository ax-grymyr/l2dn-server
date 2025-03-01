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
public class StatMulForLevel: AbstractEffect
{
	private readonly Stat _stat;
	private readonly Map<int, double> _values;

	public StatMulForLevel(StatSet @params)
	{
		_stat = @params.getEnum<Stat>("stat");

		List<int> amount = @params.getIntegerList("amount");
		_values = new();
		int index = 0;
		foreach (int level in @params.getIntegerList("level"))
		{
			_values.put(level,amount[index++]);
		}

		if (@params.getEnum("mode", StatModifierType.PER) != StatModifierType.PER)
		{
			LOGGER.Warn(GetType().Name + " can only use PER mode.");
		}
	}

	public override void pump(Creature effected, Skill skill)
	{
		if (_values.TryGetValue(effected.getLevel(), out double amount))
		{
			effected.getStat().mergeMul(_stat, amount / 100 + 1);
		}
	}
}