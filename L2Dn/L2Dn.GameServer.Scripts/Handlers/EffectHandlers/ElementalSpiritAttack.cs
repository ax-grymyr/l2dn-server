using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author JoeAlisson
 */
public class ElementalSpiritAttack: AbstractStatEffect
{
	public ElementalSpiritAttack(StatSet @params): base(@params, @params.getEnum<ElementalType>("type").getAttackStat())
	{
	}
}