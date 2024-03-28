using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeDonation;

public readonly struct ExPledgeDonationInfoPacket: IOutgoingPacket
{
    private readonly int _curPoints;
    private readonly bool _accepted;
	
    public ExPledgeDonationInfoPacket(int curPoints, bool accepted)
    {
        _curPoints = curPoints;
        _accepted = accepted;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_DONATION_INFO);
        
        writer.WriteInt32(_curPoints);
        writer.WriteByte(!_accepted);
    }
}