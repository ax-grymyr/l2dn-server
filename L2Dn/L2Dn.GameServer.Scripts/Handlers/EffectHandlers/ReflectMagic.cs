using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("ReflectMagic")]
public sealed class ReflectMagic(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.VENGEANCE_SKILL_MAGIC_DAMAGE);