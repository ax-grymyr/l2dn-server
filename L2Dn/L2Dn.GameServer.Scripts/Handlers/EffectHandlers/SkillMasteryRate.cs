using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SkillMasteryRate(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.SKILL_MASTERY_RATE);