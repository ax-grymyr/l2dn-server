using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetNpcId.
 */
public class ConditionTargetNpcId: Condition
{
	private readonly Set<int> _npcIds;
	
	/**
	 * Instantiates a new condition target npc id.
	 * @param npcIds the npc ids
	 */
	public ConditionTargetNpcId(Set<int> npcIds)
	{
		_npcIds = npcIds;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if ((effected != null) && (effected.isNpc() || effected.isDoor()))
		{
			return _npcIds.Contains(effected.getId());
		}
		return false;
	}
}
