using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPrivateStoreManageSellPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
        // TODO: implement me properly
        // packet.readInt();
        // packet.readLong();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        // Player shouldn't be able to set stores if he/she is alike dead (dead or fake death)
        if (player.isAlikeDead() || player.isInOlympiadMode())
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
        }

        return ValueTask.CompletedTask;
    }
}