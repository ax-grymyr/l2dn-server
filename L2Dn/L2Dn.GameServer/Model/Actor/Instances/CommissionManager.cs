using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class CommissionManager: Npc
{
    public CommissionManager(NpcTemplate template): base(template)
    {
        setInstanceType(InstanceType.CommissionManager);
    }

    public override bool isAutoAttackable(Creature attacker)
    {
        if (attacker.isMonster())
        {
            return true;
        }

        return base.isAutoAttackable(attacker);
    }

    public override void onBypassFeedback(Player player, String command)
    {
        if (command.equalsIgnoreCase("show_commission"))
        {
            player.sendPacket(ExShowCommission.STATIC_PACKET);
        }
        else
        {
            base.onBypassFeedback(player, command);
        }
    }
}
