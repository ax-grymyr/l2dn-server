﻿using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets.Commission;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class CommissionManager: Npc
{
    public CommissionManager(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.CommissionManager;
    }

    public override bool isAutoAttackable(Creature attacker)
    {
        if (attacker.isMonster())
        {
            return true;
        }

        return base.isAutoAttackable(attacker);
    }

    public override void onBypassFeedback(Player player, string command)
    {
        if (command.equalsIgnoreCase("show_commission"))
        {
            player.sendPacket(ExShowCommissionPacket.STATIC_PACKET);
        }
        else
        {
            base.onBypassFeedback(player, command);
        }
    }
}