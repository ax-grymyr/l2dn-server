using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class ElixirUsageLimit: AbstractStatAddEffect
{
	public ElixirUsageLimit(StatSet @params): base(@params, Stat.ELIXIR_USAGE_LIMIT)
	{
	}
}