using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCubeGameChangePointsPacket: IOutgoingPacket
{
    private readonly int _timeLeft;
    private readonly int _bluePoints;
    private readonly int _redPoints;
	
    /**
     * Change Client Point Counter
     * @param timeLeft Time Left before Minigame's End
     * @param bluePoints Current Blue Team Points
     * @param redPoints Current Red Team Points
     */
    public ExCubeGameChangePointsPacket(int timeLeft, int bluePoints, int redPoints)
    {
        _timeLeft = timeLeft;
        _bluePoints = bluePoints;
        _redPoints = redPoints;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLOCK_UP_SET_STATE); // TODO: packet code?
        
        writer.WriteInt32(2);
        writer.WriteInt32(_timeLeft);
        writer.WriteInt32(_bluePoints);
        writer.WriteInt32(_redPoints);
    }
}