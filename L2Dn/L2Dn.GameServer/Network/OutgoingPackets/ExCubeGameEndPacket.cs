using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCubeGameEndPacket: IOutgoingPacket
{
    private readonly bool _isRedTeamWin;
	
    /**
     * Show Minigame Results
     * @param isRedTeamWin Is Red Team Winner?
     */
    public ExCubeGameEndPacket(bool isRedTeamWin)
    {
        _isRedTeamWin = isRedTeamWin;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLOCK_UP_SET_STATE);
        
        writer.WriteInt32(1);
        writer.WriteInt32(_isRedTeamWin);
        writer.WriteInt32(0); // TODO: Find me!
    }
}