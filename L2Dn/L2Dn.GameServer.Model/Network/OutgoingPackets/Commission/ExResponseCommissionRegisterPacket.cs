using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Commission;

public readonly struct ExResponseCommissionRegisterPacket: IOutgoingPacket
{
    public static readonly ExResponseCommissionRegisterPacket SUCCEED = new(true);
    public static readonly ExResponseCommissionRegisterPacket FAILED = new(false);
	
    private readonly bool _success;
	
    public ExResponseCommissionRegisterPacket(bool success)
    {
        _success = success;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_COMMISSION_REGISTER);
        writer.WriteInt32(_success);
    }
}