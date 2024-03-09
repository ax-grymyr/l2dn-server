using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PAtk: AbstractConditionalHpEffect
{
	public PAtk(StatSet @params): base(@params, Stat.PHYSICAL_ATTACK)
	{
	}
}