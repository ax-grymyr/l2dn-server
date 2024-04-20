using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author NasSeKa
 */
public class TwoHandedStance: AbstractEffect
{
	private readonly double _amount;
	
	public TwoHandedStance(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		effected.getStat().mergeAdd(Stat.PHYSICAL_ATTACK, (_amount * effected.getShldDef()) / 100);
	}
}