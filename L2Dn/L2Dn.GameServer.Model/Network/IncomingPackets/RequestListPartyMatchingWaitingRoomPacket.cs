using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestListPartyMatchingWaitingRoomPacket: IIncomingPacket<GameSession>
{
    private int _page;
    private int _minLevel;
    private int _maxLevel;
    private List<CharacterClass> _classId; // 1 - waitlist 0 - room waitlist
    private string? _query;

    public void ReadContent(PacketBitReader reader)
    {
        _page = reader.ReadInt32();
        _minLevel = reader.ReadInt32();
        _maxLevel = reader.ReadInt32();
        int size = reader.ReadInt32();
        _classId = [];
        if (size > 0 && size < 128)
        {
            for (int i = 0; i < size; i++)
                _classId.Add((CharacterClass)reader.ReadInt32());
        }

        if (reader.Length >= 4)
            _query = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExListPartyMatchingWaitingRoomPacket(_page, _minLevel, _maxLevel, _classId, _query ?? string.Empty));

        return ValueTask.CompletedTask;
    }
}