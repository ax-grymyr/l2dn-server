using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.OutgoingPackets.Variations;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Variations;

public struct ExVariationCloseUiPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
        //reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        //player.sendPacket(ExShowVariationCancelWindowPacket.STATIC_PACKET);
		
        if (player.getRequest<VariationRequest>() != null)
        {
            player.removeRequest<VariationRequest>();
        }
        
        return ValueTask.CompletedTask;
    }
}