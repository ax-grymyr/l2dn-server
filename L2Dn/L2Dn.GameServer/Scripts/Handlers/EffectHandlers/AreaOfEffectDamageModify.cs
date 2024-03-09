using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author quangnguyen
 */
public class AreaOfEffectDamageModify: AbstractStatPercentEffect
{
	public AreaOfEffectDamageModify(StatSet @params): base(@params, Stat.AREA_OF_EFFECT_DAMAGE_MODIFY)
	{
	}
}