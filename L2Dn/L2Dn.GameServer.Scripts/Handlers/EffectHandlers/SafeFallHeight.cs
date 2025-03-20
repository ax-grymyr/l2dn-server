using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("SafeFallHeight")]
public sealed class SafeFallHeight(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.FALL);