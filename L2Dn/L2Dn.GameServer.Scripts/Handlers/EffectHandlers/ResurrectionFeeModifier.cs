using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ResurrectionFeeModifier(StatSet @params): AbstractStatEffect(@params, Stat.RESURRECTION_FEE_MODIFIER);