using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestGoToLobbyPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        session.Characters = CharacterPacketHelper.LoadCharacterSelectInfo(session.AccountId);

        CharacterListPacket characterListPacket = new(session.PlayKey1, session.AccountName, session.Characters,
            session.SelectedCharacterIndex);
        
        connection.Send(ref characterListPacket);

        return ValueTask.CompletedTask;
    }
}