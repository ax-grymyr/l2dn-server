using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw, Mobius
 */
public class HealEffect: AbstractStatEffect
{
	public HealEffect(StatSet @params): base(@params, Stat.HEAL_EFFECT, Stat.HEAL_EFFECT_ADD)
	{
	}
}