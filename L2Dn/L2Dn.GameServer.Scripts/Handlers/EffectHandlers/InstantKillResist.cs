using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class InstantKillResist: AbstractEffect
{
	private readonly Double _amount;
	
	public InstantKillResist(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		effected.getStat().mergeAdd(Stat.INSTANT_KILL_RESIST, _amount);
	}
}