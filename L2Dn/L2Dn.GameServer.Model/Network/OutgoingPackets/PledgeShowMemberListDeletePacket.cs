using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeShowMemberListDeletePacket: IOutgoingPacket
{
    private readonly string _player;

    public PledgeShowMemberListDeletePacket(string playerName)
    {
        _player = playerName;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_SHOW_MEMBER_LIST_DELETE);
        
        writer.WriteString(_player);
    }
}