using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * This class represents Friendly Mobs lying over the world.<br>
 * These friendly mobs should only attack players with karma > 0 and it is always aggro, since it just attacks players with karma.
 */
public class FriendlyMob: Attackable
{
    public FriendlyMob(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.FriendlyMob;
    }

    public override bool isAutoAttackable(Creature attacker)
    {
        if (attacker.isPlayer())
        {
            return ((Player)attacker).getReputation() < 0;
        }

        return base.isAutoAttackable(attacker);
    }

    public override bool isAggressive()
    {
        return true;
    }
}
