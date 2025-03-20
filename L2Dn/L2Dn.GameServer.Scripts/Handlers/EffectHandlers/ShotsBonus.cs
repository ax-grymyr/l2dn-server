using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("ShotsBonus")]
public sealed class ShotsBonus(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.SHOTS_BONUS);