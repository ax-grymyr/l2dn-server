using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("EnchantRate")]
public sealed class EnchantRate(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.ENCHANT_RATE);