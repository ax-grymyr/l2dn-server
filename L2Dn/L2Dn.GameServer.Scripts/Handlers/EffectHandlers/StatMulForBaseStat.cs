using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class StatMulForBaseStat: AbstractEffect
{
	private readonly BaseStat _baseStat;
	private readonly int _min;
	private readonly int _max;
	private readonly Stat _mulStat;
	private readonly double _amount;
	
	public StatMulForBaseStat(StatSet @params)
	{
		_baseStat = @params.getEnum<BaseStat>("baseStat");
		_min = @params.getInt("min", 0);
		_max = @params.getInt("max", 2147483647);
		_mulStat = @params.getEnum<Stat>("mulStat");
		_amount = @params.getDouble("amount", 0);
		if (@params.getEnum("mode", StatModifierType.PER) != StatModifierType.PER)
		{
			LOGGER.Warn(GetType().Name + " can only use PER mode.");
		}
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		int currentValue = 0;
		switch (_baseStat)
		{
			case BaseStat.STR:
			{
				currentValue = effected.getSTR();
				break;
			}
			case BaseStat.INT:
			{
				currentValue = effected.getINT();
				break;
			}
			case BaseStat.DEX:
			{
				currentValue = effected.getDEX();
				break;
			}
			case BaseStat.WIT:
			{
				currentValue = effected.getWIT();
				break;
			}
			case BaseStat.CON:
			{
				currentValue = effected.getCON();
				break;
			}
			case BaseStat.MEN:
			{
				currentValue = effected.getMEN();
				break;
			}
		}
		
		if ((currentValue >= _min) && (currentValue <= _max))
		{
			effected.getStat().mergeMul(_mulStat, (_amount / 100) + 1);
		}
	}
}