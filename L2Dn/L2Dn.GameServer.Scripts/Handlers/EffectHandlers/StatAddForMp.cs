using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class StatAddForMp: AbstractEffect
{
	private readonly int _mp;
	private readonly Stat _stat;
	private readonly double _amount;
	
	public StatAddForMp(StatSet @params)
	{
		_mp = @params.getInt("mp", 0);
		_stat = @params.getEnum<Stat>("stat");
		_amount = @params.getDouble("amount", 0);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		if (effected.getMaxMp() >= _mp)
		{
			effected.getStat().mergeAdd(_stat, _amount);
		}
	}
}