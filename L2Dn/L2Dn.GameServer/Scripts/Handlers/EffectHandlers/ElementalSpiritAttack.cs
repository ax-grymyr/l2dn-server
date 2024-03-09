using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author JoeAlisson
 */
public class ElementalSpiritAttack: AbstractStatEffect
{
	public ElementalSpiritAttack(StatSet @params): base(@params, @params.getEnum<ElementalType>("type").getAttackStat())
	{
	}
}