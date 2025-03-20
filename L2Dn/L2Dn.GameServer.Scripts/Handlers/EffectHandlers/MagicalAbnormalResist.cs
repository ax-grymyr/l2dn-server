using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("MagicalAbnormalResist")]
public sealed class MagicalAbnormalResist(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.ABNORMAL_RESIST_MAGICAL);