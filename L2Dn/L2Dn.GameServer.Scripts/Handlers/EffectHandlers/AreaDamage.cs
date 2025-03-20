using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("AreaDamage")]
public sealed class AreaDamage(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.DAMAGE_ZONE_VULN);