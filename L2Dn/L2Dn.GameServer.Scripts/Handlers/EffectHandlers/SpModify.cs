using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("SpModify")]
public sealed class SpModify(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.BONUS_SP);