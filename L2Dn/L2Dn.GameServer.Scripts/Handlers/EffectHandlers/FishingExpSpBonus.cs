using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class FishingExpSpBonus(StatSet @params): AbstractStatPercentEffect(@params, Stat.FISHING_EXP_SP_BONUS);