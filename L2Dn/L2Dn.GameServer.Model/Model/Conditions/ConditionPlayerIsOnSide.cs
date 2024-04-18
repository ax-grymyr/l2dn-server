using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerIsOnSide.
 * @author St3eT
 */
public class ConditionPlayerIsOnSide : Condition
{
	private readonly CastleSide _side;
	
	/**
	 * Instantiates a new condition player race.
	 * @param side the allowed Castle side.
	 */
	public ConditionPlayerIsOnSide(CastleSide side)
	{
		_side = side;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if ((effector == null) || !effector.isPlayer())
		{
			return false;
		}
		return effector.getActingPlayer().getPlayerSide() == _side;
	}
}
