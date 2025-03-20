using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("BonusRaidPoints")]
public sealed class BonusRaidPoints(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.BONUS_RAID_POINTS);