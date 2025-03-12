using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ElementalSpiritAttack(StatSet @params)
    : AbstractStatEffect(@params, @params.getEnum<ElementalType>("type").getAttackStat());