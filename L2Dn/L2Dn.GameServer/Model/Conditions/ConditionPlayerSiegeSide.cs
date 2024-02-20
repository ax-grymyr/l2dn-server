using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerSiegeSide.
 */
public class ConditionPlayerSiegeSide : Condition
{
	private readonly int _siegeSide;
	
	/**
	 * Instantiates a new condition player siege side.
	 * @param side the side
	 */
	public ConditionPlayerSiegeSide(int side)
	{
		_siegeSide = side;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return false;
		}
		return effector.getActingPlayer().getSiegeSide() == _siegeSide;
	}
}
