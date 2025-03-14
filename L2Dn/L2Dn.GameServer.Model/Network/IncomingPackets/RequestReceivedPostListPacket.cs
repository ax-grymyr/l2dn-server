using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestReceivedPostListPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || !Config.General.ALLOW_MAIL)
            return ValueTask.CompletedTask;

        // if (!activeChar.isInsideZone(ZoneId.PEACE))
        // {
        // player.sendPacket(SystemMessageId.YOU_CANNOT_RECEIVE_OR_SEND_MAIL_WITH_ATTACHED_ITEMS_IN_NON_PEACE_ZONE_REGIONS);
        // return;
        // }

        connection.Send(new ExShowReceivedPostListPacket(player.ObjectId));
        return ValueTask.CompletedTask;
    }
}