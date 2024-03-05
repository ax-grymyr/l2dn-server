using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct CameraModePacket: IOutgoingPacket
{
    private readonly int _mode;
	
    /**
     * Forces client camera mode change
     * @param mode 0 - third person cam 1 - first person cam
     */
    public CameraModePacket(int mode)
    {
        _mode = mode;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.CAMERA_MODE);
        
        writer.WriteInt32(_mode);
    }
}