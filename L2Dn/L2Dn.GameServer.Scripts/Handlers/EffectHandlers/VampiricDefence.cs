using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author NasSeKa
 */
public class VampiricDefence: AbstractStatPercentEffect
{
	public VampiricDefence(StatSet @params): base(@params, Stat.ABSORB_DAMAGE_DEFENCE)
	{
	}
}