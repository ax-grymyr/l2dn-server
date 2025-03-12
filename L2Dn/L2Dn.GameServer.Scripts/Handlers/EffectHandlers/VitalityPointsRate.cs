using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class VitalityPointsRate(StatSet @params): AbstractStatPercentEffect(@params, Stat.VITALITY_CONSUME_RATE);