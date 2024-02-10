using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestNewCharacterPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        CharacterClass[] classes =
        [
            CharacterClass.Fighter, CharacterClass.Mage, CharacterClass.ElvenFighter, CharacterClass.ElvenMage,
            CharacterClass.DarkFighter, CharacterClass.DarkMage, CharacterClass.OrcFighter, CharacterClass.OrcMage,
            CharacterClass.DwarvenFighter
        ];

        NewCharacterSuccessPacket newCharacterSuccessPacket = new(classes);
        connection.Send(ref newCharacterSuccessPacket);

        return ValueTask.CompletedTask;
    }
}
