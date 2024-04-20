using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author quangnguyen
 */
public class AreaOfEffectDamageModify: AbstractStatPercentEffect
{
	public AreaOfEffectDamageModify(StatSet @params): base(@params, Stat.AREA_OF_EFFECT_DAMAGE_MODIFY)
	{
	}
}