using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class ExpModifyPet: AbstractStatAddEffect
{
	public ExpModifyPet(StatSet @params): base(@params, Stat.BONUS_EXP_PET)
	{
	}
}