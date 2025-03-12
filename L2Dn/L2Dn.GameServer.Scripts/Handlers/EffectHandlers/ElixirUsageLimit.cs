using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ElixirUsageLimit(StatSet @params): AbstractStatAddEffect(@params, Stat.ELIXIR_USAGE_LIMIT);