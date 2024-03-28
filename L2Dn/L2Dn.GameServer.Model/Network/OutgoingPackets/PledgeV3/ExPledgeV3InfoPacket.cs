using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;

public readonly struct ExPledgeV3InfoPacket: IOutgoingPacket
{
    private readonly int _points;
    private readonly int _rank;
    private readonly string _announce;
    private readonly bool _isShowOnEnter;
	
    public ExPledgeV3InfoPacket(int points, int rank, string announce, bool isShowOnEnter)
    {
        _points = points;
        _rank = rank;
        _announce = announce;
        _isShowOnEnter = isShowOnEnter;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_V3_INFO);

        writer.WriteInt32(_points);
        writer.WriteInt32(_rank);
        writer.WriteSizedString(_announce);
        writer.WriteByte(_isShowOnEnter);
    }
}