using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCubeGameChangeTimeToStartPacket: IOutgoingPacket
{
    private readonly int _seconds;
	
    /**
     * Update Minigame Waiting List Time to Start
     * @param seconds
     */
    public ExCubeGameChangeTimeToStartPacket(int seconds)
    {
        _seconds = seconds;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLOCK_UP_SET_LIST);
        
        writer.WriteInt32(3);
        writer.WriteInt32(_seconds);
    }
}