using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestAskJoinPartyRoomPacket: IIncomingPacket<GameSession>
{
    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // Send PartyRoom invite request (with activeChar) name to the target
        Player? target = World.getInstance().getPlayer(_name);
        if (target != null)
        {
            if (!target.isProcessingRequest())
            {
                player.onTransactionRequest(target);
                target.sendPacket(new ExAskJoinPartyRoomPacket(player));
            }
            else
            {
                SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
                sm.Params.addPcName(target);
                player.sendPacket(sm);
            }
        }
        else
        {
            player.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
        }

        return ValueTask.CompletedTask;
    }
}