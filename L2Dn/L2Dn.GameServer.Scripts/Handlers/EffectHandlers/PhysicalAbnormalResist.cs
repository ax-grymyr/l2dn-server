using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PhysicalAbnormalResist(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.ABNORMAL_RESIST_PHYSICAL);