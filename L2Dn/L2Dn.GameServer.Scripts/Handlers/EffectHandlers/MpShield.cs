using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MpShield(StatSet @params): AbstractStatAddEffect(@params, Stat.MANA_SHIELD_PERCENT);