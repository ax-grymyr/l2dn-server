using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct Ex2NdPasswordAckPacket: IOutgoingPacket
{
    // TODO: Enum
    public const int SUCCESS = 0;
    public const int WRONG_PATTERN = 1;
	
    private readonly int _status;
    private readonly int _response;
	
    public Ex2NdPasswordAckPacket(int status, int response)
    {
        _status = status;
        _response = response;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_2ND_PASSWORD_ACK);

        writer.WriteByte((byte)_status);
        writer.WriteInt32(_response == WRONG_PATTERN);
        writer.WriteInt32(0);
    }
}