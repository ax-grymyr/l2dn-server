using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("SkillBonusRange")]
public sealed class SkillBonusRange(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.MAGIC_ATTACK_RANGE);