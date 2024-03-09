using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class MAtkByPAtk: AbstractStatPercentEffect
{
	public MAtkByPAtk(StatSet @params): base(@params, Stat.MAGIC_ATTACK_BY_PHYSICAL_ATTACK)
	{
	}
}