using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SkillPowerAdd(StatSet @params): AbstractStatAddEffect(@params, Stat.SKILL_POWER_ADD);