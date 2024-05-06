using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author JoeAlisson
 */
public class ElementalSpiritDefense: AbstractStatEffect
{
	public ElementalSpiritDefense(StatSet @params): base(@params, @params.getEnum<ElementalType>("type").getDefenseStat())
	{
	}
}