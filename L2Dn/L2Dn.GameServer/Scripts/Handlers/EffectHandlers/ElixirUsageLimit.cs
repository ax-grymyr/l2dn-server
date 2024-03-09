using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class ElixirUsageLimit: AbstractStatAddEffect
{
	public ElixirUsageLimit(StatSet @params): base(@params, Stat.ELIXIR_USAGE_LIMIT)
	{
	}
}