using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExOlympiadModePacket: IOutgoingPacket
{
    private readonly int _mode;
	
    /**
     * @param mode (0 = return, 3 = spectate)
     */
    public ExOlympiadModePacket(int mode)
    {
        _mode = mode;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_OLYMPIAD_MODE);
        
        writer.WriteByte((byte)_mode);
    }
}