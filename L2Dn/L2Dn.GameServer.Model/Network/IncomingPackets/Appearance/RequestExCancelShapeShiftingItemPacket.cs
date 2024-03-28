using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.OutgoingPackets.Appearance;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Appearance;

public struct RequestExCancelShapeShiftingItemPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.removeRequest<ShapeShiftingItemRequest>();
        player.sendPacket(ExShapeShiftingResultPacket.FAILED);
        
        return ValueTask.CompletedTask;
    }
}