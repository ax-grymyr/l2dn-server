using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class VampiricAttack: AbstractEffect
{
	private readonly double _amount;
	private readonly double _sum;
	
	public VampiricAttack(StatSet @params)
	{
		_amount = @params.getDouble("amount");
		_sum = _amount * @params.getDouble("chance");
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		effected.getStat().mergeAdd(Stat.ABSORB_DAMAGE_PERCENT, _amount / 100);
		effected.getStat().addToVampiricSum(_sum);
	}
}