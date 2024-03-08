using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author JoeAlisson
 */
public class ElementalSpiritDefense: AbstractStatEffect
{
	public ElementalSpiritDefense(StatSet @params): base(@params, @params.getEnum<ElementalType>("type").getDefenseStat())
	{
	}
}