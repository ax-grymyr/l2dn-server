using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PhysicalSkillCriticalRate(StatSet @params): AbstractStatPercentEffect(@params, Stat.CRITICAL_RATE_SKILL);