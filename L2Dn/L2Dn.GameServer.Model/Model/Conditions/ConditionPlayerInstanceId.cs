using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerInstanceId.
 */
public sealed class ConditionPlayerInstanceId(Set<int> instanceIds): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (player is null)
            return false;

        Instance? instance = player.getInstanceWorld();
        return instance != null && instanceIds.Contains(instance.getTemplateId());
    }
}