using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public sealed class ConditionTargetCheckCrtEffect(bool isCrtEffect): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        if (effected.isNpc())
            return ((Npc)effected).getTemplate().canBeCrt() == isCrtEffect;

        return true;
    }
}