using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GMHidePacket: IOutgoingPacket
{
    private readonly int _mode;
	
    /**
     * @param mode (0 = display windows, 1 = hide windows)
     */
    public GMHidePacket(int mode)
    {
        _mode = mode;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GM_HIDE);
        
        writer.WriteInt32(_mode);
    }
}