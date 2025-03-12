using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class AdditionalPotionHp(StatSet @params): AbstractStatAddEffect(@params, Stat.ADDITIONAL_POTION_HP);