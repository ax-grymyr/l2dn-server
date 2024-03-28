using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class BonusDropAdena: AbstractStatPercentEffect
{
	public BonusDropAdena(StatSet @params): base(@params, Stat.BONUS_DROP_ADENA)
	{
	}
}