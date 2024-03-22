using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.ElementalSpirits;

public struct ExElementalSpiritExtractInfoPacket: IIncomingPacket<GameSession>
{
    private ElementalType _type;

    public void ReadContent(PacketBitReader reader)
    {
        _type = (ElementalType)reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        connection.Send(new ElementalSpiritExtractInfoPacket(player, _type));

        return ValueTask.CompletedTask;
    }
}