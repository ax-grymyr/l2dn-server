using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ElementalSpiritDefense(StatSet @params)
    : AbstractStatEffect(@params, @params.getEnum<ElementalType>("type").getDefenseStat());