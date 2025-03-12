using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class EnchantRate(StatSet @params): AbstractStatAddEffect(@params, Stat.ENCHANT_RATE);