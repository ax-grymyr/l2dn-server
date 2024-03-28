using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.ElementalSpirits;

public struct ExElementalSpiritInfoPacket: IIncomingPacket<GameSession>
{
    private byte _type;

    public void ReadContent(PacketBitReader reader)
    {
        _type = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        connection.Send(new ElementalSpiritInfoPacket(player, _type));
        
        return ValueTask.CompletedTask;
    }
}