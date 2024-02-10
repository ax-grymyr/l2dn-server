using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestCharacterSelectPacket: IIncomingPacket<GameSession>
{
    private int _charSlot;
    private int _unk1; // new in C4
    private int _unk2;
    private int _unk3;
    private int _unk4;

    public void ReadContent(PacketBitReader reader)
    {
        _charSlot = reader.ReadInt32();
        _unk1 = reader.ReadInt16();
        _unk2 = reader.ReadInt32();
        _unk3 = reader.ReadInt32();
        _unk4 = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        GameSession session = connection.Session;
        session.State = GameSessionState.EnteringGame;

        // C4
        // SsqInfoPacket ssqInfoPacket = new();
        // connection.Send(ref ssqInfoPacket);

        if (_charSlot >= 0 && _charSlot < session.Characters.Count)
        {
            session.SelectedCharacter = session.Characters[_charSlot];
            session.SelectedCharacter.LastAccess = DateTime.UtcNow;
            session.Location = new(session.SelectedCharacter.LocationX, session.SelectedCharacter.LocationY,
                session.SelectedCharacter.LocationZ);

            CharacterSelectedPacket characterSelectedPacket = new(session.PlayKey1, session.SelectedCharacter);
            connection.Send(ref characterSelectedPacket);
        }
        else
        {
            connection.Close();
        }

        return ValueTask.CompletedTask;
    }
}
