using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author UnAfraid
 */
public class ConditionPlayerInsideZoneId : Condition
{
	private readonly Set<int> _zones;
	
	public ConditionPlayerInsideZoneId(Set<int> zones)
	{
		_zones = zones;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return false;
		}
		
		foreach (ZoneType zone in ZoneManager.getInstance().getZones(effector.getLocation().Location3D))
		{
			if (_zones.Contains(zone.getId()))
			{
				return true;
			}
		}
		return false;
	}
}
