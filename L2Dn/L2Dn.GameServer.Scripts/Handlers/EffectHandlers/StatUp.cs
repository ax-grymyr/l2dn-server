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
public class StatUp: AbstractEffect
{
	private readonly double _amount;
	private readonly Stat? _singleStat;
	private readonly Set<Stat>? _multipleStats;
	
	public StatUp(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
		string stats = @params.getString("stat", "STR");
		if (stats.contains(","))
		{
			_singleStat = null;
			_multipleStats = new();
			foreach (string stat in stats.Split(","))
			{
				_multipleStats.add(Enum.Parse<Stat>("STAT_" + stat));
			}
		}
		else
		{
			_singleStat = Enum.Parse<Stat>("STAT_" + stats);
			_multipleStats = null;
		}
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		if (_singleStat != null)
		{
			effected.getStat().mergeAdd(_singleStat.Value, _amount);
			return;
		}

		if (_multipleStats != null)
		{
			foreach (Stat stat in _multipleStats)
			{
				effected.getStat().mergeAdd(stat, _amount);
			}
		}
	}
}
