using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class CounterPhysicalSkill(StatSet @params)
    : AbstractStatAddEffect(@params, Stat.VENGEANCE_SKILL_PHYSICAL_DAMAGE);