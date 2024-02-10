using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Condition which checks if you are within the given range of a summoned by you npc.
 * @author Nik
 */
public class ConditionPlayerRangeFromSummonedNpc : Condition
{
	/** NPC Ids. */
	private readonly Set<int> _npcIds;
	/** Radius to check. */
	private readonly int _radius;
	/** Expected value. */
	private readonly bool _value;
	
	public ConditionPlayerRangeFromSummonedNpc(Set<int> npcIds, int radius, bool value)
	{
		_npcIds = npcIds;
		_radius = radius;
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		bool existNpc = false;
		if (!_npcIds.isEmpty() && (_radius > 0))
		{
			foreach (Npc target in World.getInstance().getVisibleObjectsInRange<Npc>(effector, _radius))
			{
				if (_npcIds.contains(target.getId()) && (effector == target.getSummoner()))
				{
					existNpc = true;
					break;
				}
			}
		}
		return existNpc == _value;
	}
}
