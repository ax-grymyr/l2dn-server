using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct Ex2NdPasswordVerifyPacket: IOutgoingPacket
{
    // TODO: Enum
    public const int PASSWORD_OK = 0;
    public const int PASSWORD_WRONG = 1;
    public const int PASSWORD_BAN = 2;
	
    private readonly int _wrongTentatives;
    private readonly int _mode;
	
    public Ex2NdPasswordVerifyPacket(int mode, int wrongTentatives)
    {
        _mode = mode;
        _wrongTentatives = wrongTentatives;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_2ND_PASSWORD_VERIFY);

        writer.WriteInt32(_mode);
        writer.WriteInt32(_wrongTentatives);
    }
}