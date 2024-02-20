using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerInstanceId.
 */
public class ConditionPlayerInstanceId : Condition
{
	private readonly Set<int> _instanceIds;
	
	/**
	 * Instantiates a new condition player instance id.
	 * @param instanceIds the instance ids
	 */
	public ConditionPlayerInstanceId(Set<int> instanceIds)
	{
		_instanceIds = instanceIds;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Player player = effector.getActingPlayer();
		if (player == null)
		{
			return false;
		}
		
		Instance instance = player.getInstanceWorld();
		return (instance != null) && _instanceIds.Contains(instance.getTemplateId());
	}
}
