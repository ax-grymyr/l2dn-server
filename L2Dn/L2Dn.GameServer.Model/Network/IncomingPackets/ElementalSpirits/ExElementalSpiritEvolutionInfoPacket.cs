using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.ElementalSpirits;

public struct ExElementalSpiritEvolutionInfoPacket: IIncomingPacket<GameSession>
{
    private ElementalType _id;

    public void ReadContent(PacketBitReader reader)
    {
        _id = (ElementalType)reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        connection.Send(new ElementalSpiritEvolutionInfoPacket(player, _id));
        
        return ValueTask.CompletedTask;
    }
}