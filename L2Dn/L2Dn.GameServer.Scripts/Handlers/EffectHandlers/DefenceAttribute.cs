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
public class DefenceAttribute: AbstractEffect
{
	private readonly double _amount;
	private readonly Stat? _singleStat;
	private readonly Set<Stat>? _multipleStats;
	
	public DefenceAttribute(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
		string attributes = @params.getString("attribute", "FIRE");
		if (attributes.contains(","))
		{
			_singleStat = null;
			_multipleStats = [];
			foreach (string attribute in attributes.Split(","))
			{
				_multipleStats.add(Enum.Parse<Stat>(attribute + "_RES"));
			}
		}
		else
		{
			_singleStat = Enum.Parse<Stat>(attributes + "_RES");
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
