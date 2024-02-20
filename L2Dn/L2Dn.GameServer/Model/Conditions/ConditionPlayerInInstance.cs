using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public class ConditionPlayerInInstance : Condition
{
	private readonly bool _inInstance;
	
	public ConditionPlayerInInstance(bool inInstance)
	{
		_inInstance = inInstance;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return false;
		}
		return (effector.getInstanceId() == 0) ? !_inInstance : _inInstance;
	}
}
