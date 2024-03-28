using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerSouls.
 */
public class ConditionPlayerSouls : Condition
{
	private readonly int _souls;
	private readonly SoulType _type;
	
	/**
	 * Instantiates a new condition player souls.
	 * @param souls the souls
	 * @param type the soul type
	 */
	public ConditionPlayerSouls(int souls, SoulType type)
	{
		_souls = souls;
		_type = type;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return (effector.getActingPlayer() != null) && (effector.getActingPlayer().getChargedSouls(_type) >= _souls);
	}
}
