using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author quangnguyen
 */
public class AreaOfEffectDamageDefence: AbstractStatEffect
{
	public AreaOfEffectDamageDefence(StatSet @params): base(@params, Stat.AREA_OF_EFFECT_DAMAGE_DEFENCE)
	{
	}
}