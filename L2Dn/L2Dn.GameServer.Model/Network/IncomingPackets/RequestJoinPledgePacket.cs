using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestJoinPledgePacket: IIncomingPacket<GameSession>
{
    private int _target;
    private int _pledgeType;

    public void ReadContent(PacketBitReader reader)
    {
        _target = reader.ReadInt32();
        _pledgeType = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;

        WorldObject? playerTarget = player.getTarget();
        if (playerTarget != null && FakePlayerData.getInstance().isTalkable(playerTarget.getName()))
        {
            if (FakePlayerData.getInstance().getInfo(playerTarget.getId()).getClanId() > 0)
            {
                player.sendPacket(SystemMessageId.THAT_PLAYER_ALREADY_BELONGS_TO_ANOTHER_CLAN);
            }
            else
            {
                if (!player.isProcessingRequest())
                {
                    ThreadPool.schedule(() => scheduleDeny(player, playerTarget.getName()), 10000);
                    player.blockRequest();
                }
                else
                {
                    SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
                    msg.Params.addString(playerTarget.getName());
                    player.sendPacket(msg);
                }
            }

            return ValueTask.CompletedTask;
        }

        Player? target = World.getInstance().getPlayer(_target);
        if (target == null)
        {
            player.sendPacket(SystemMessageId.THE_TARGET_CANNOT_BE_INVITED);
            return ValueTask.CompletedTask;
        }

        if (!clan.checkClanJoinCondition(player, target, _pledgeType))
            return ValueTask.CompletedTask;

        if (!player.getRequest().setRequest(target, this))
            return ValueTask.CompletedTask;

        string pledgeName = clan.getName();
        target.sendPacket(new AskJoinPledgePacket(player, pledgeName));
        return ValueTask.CompletedTask;
    }

    public int getPledgeType()
    {
        return _pledgeType;
    }

    private static void scheduleDeny(Player player, string name)
    {
        if (player != null)
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DID_NOT_RESPOND_INVITATION_TO_THE_CLAN_HAS_BEEN_CANCELLED);
            sm.Params.addString(name);
            player.sendPacket(sm);
            player.onTransactionResponse();
        }
    }
}