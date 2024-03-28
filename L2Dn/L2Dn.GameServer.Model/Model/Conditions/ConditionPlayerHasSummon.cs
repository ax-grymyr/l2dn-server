using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public class ConditionPlayerHasSummon : Condition
{
	private readonly bool _hasSummon;
	
	public ConditionPlayerHasSummon(bool hasSummon)
	{
		_hasSummon = hasSummon;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Player player = effector.getActingPlayer();
		if (player == null)
		{
			return false;
		}
		return _hasSummon == player.hasSummon();
	}
}
