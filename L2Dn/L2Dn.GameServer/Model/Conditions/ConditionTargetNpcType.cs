using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetNpcType.
 */
public class ConditionTargetNpcType: Condition
{
	private readonly InstanceType[] _npcType;
	
	/**
	 * Instantiates a new condition target npc type.
	 * @param type the type
	 */
	public ConditionTargetNpcType(InstanceType[] type)
	{
		_npcType = type;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effected == null)
		{
			return false;
		}
		return effected.getInstanceType().isTypes(_npcType);
	}
}
