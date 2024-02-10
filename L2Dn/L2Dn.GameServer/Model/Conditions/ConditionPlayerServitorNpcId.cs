using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerServitorNpcId.
 */
public class ConditionPlayerServitorNpcId : Condition
{
	private readonly List<int> _npcIds;
	
	/**
	 * Instantiates a new condition player servitor npc id.
	 * @param npcIds the npc ids
	 */
	public ConditionPlayerServitorNpcId(List<int> npcIds)
	{
		if ((npcIds.Count == 1) && (npcIds[0] == 0))
		{
			_npcIds = null;
		}
		else
		{
			_npcIds = npcIds;
		}
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if ((effector.getActingPlayer() == null) || !effector.getActingPlayer().hasSummon())
		{
			return false;
		}
		if (_npcIds == null)
		{
			return true;
		}
		foreach (Summon summon in effector.getServitors().values())
		{
			if (_npcIds.Contains(summon.getId()))
			{
				return true;
			}
		}
		return false;
	}
}
