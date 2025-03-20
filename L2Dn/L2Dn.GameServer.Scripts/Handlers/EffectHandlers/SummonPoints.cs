using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SummonPoints(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.MAX_SUMMON_POINTS);