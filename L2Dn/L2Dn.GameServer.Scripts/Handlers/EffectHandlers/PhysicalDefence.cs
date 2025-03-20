using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("PhysicalDefence")]
public sealed class PhysicalDefence(EffectParameterSet parameters):
    AbstractConditionalHpEffect(parameters, Stat.PHYSICAL_DEFENCE);