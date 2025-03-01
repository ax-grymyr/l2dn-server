using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAskJoinPartyRoomPacket: IOutgoingPacket
{
    private readonly string _charName;
    private readonly string _roomName;

    public ExAskJoinPartyRoomPacket(Player player)
    {
        _charName = player.getName();
        _roomName = player.getMatchingRoom()?.getTitle() ?? string.Empty;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ASK_JOIN_PARTY_ROOM);

        writer.WriteString(_charName);
        writer.WriteString(_roomName);
    }
}