using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Geremy
 */
public class ShieldDefenceIgnoreRemoval: AbstractStatEffect
{
	public ShieldDefenceIgnoreRemoval(StatSet @params)
		: base(@params, Stat.SHIELD_DEFENCE_IGNORE_REMOVAL, Stat.SHIELD_DEFENCE_IGNORE_REMOVAL_ADD)
	{
	}
}