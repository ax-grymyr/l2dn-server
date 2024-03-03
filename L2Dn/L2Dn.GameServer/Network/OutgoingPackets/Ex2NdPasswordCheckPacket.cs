using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct Ex2NdPasswordCheckPacket: IOutgoingPacket
{
    // TODO: Enum
    public const int PASSWORD_NEW = 0;
    public const int PASSWORD_PROMPT = 1;
    public const int PASSWORD_OK = 2;
	
    private readonly int _windowType;
	
    public Ex2NdPasswordCheckPacket(int windowType)
    {
        _windowType = windowType;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_2ND_PASSWORD_CHECK);

        writer.WriteInt32(_windowType);
        writer.WriteInt32(0);
    }
}