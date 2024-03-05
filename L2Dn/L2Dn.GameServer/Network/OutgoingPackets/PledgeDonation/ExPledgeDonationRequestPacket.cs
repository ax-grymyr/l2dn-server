using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeDonation;

public readonly struct ExPledgeDonationRequestPacket: IOutgoingPacket
{
    private readonly bool _success;
    private readonly int _type;
    private readonly int _curPoints;
	
    public ExPledgeDonationRequestPacket(bool success, int type, int curPoints)
    {
        _success = success;
        _type = type;
        _curPoints = curPoints;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_DONATION_REQUEST);
        
        writer.WriteByte((byte)_type);
        writer.WriteInt32(_success);
        writer.WriteInt16(0);
        writer.WriteInt32(3);
        writer.WriteInt32(14);
        writer.WriteInt64(0);
        writer.WriteInt16(0);
        writer.WriteInt32(_curPoints);
    }
}