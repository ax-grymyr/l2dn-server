using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class MpShield: AbstractStatAddEffect
{
	public MpShield(StatSet @params): base(@params, Stat.MANA_SHIELD_PERCENT)
	{
	}
}