using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Geremy
 */
public class DefenceIgnoreRemoval: AbstractStatEffect
{
	public DefenceIgnoreRemoval(StatSet @params): base(@params, Stat.DEFENCE_IGNORE_REMOVAL, Stat.DEFENCE_IGNORE_REMOVAL_ADD)
	{
	}
}