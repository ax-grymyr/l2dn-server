using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Commission;

public readonly struct ExResponseCommissionDeletePacket: IOutgoingPacket
{
    public static readonly ExResponseCommissionDeletePacket SUCCEED = new(true);
    public static readonly ExResponseCommissionDeletePacket FAILED = new(false);
	
    private readonly bool _success;
	
    public ExResponseCommissionDeletePacket(bool success)
    {
        _success = success;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_COMMISSION_DELETE);
        writer.WriteInt32(_success);
    }
}