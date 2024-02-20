using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Exist NPC condition.
 * @author UnAfraid, Zoey76
 */
public class ConditionPlayerRangeFromNpc : Condition
{
	/** NPC Ids. */
	private readonly Set<int> _npcIds;
	/** Radius to check. */
	private readonly int _radius;
	/** Expected value. */
	private readonly bool _value;
	
	public ConditionPlayerRangeFromNpc(Set<int> npcIds, int radius, bool value)
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
				if (_npcIds.Contains(target.getId()))
				{
					existNpc = true;
					break;
				}
			}
		}
		return existNpc == _value;
	}
}
