using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("GetDamageLimit")]
public sealed class GetDamageLimit(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.DAMAGE_LIMIT);