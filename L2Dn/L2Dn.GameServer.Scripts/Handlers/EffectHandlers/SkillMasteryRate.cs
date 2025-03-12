using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SkillMasteryRate(StatSet @params): AbstractStatPercentEffect(@params, Stat.SKILL_MASTERY_RATE);