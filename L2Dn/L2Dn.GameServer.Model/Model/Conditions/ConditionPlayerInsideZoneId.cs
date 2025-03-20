using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author UnAfraid
 */
public sealed class ConditionPlayerInsideZoneId(Set<int> zones): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effector.getActingPlayer() == null)
            return false;

        foreach (Zone zone in ZoneManager.Instance.getZones(effector.Location.Location3D))
        {
            if (zones.Contains(zone.getId()))
                return true;
        }

        return false;
    }
}