using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCubeGameExtendedChangePointsPacket: IOutgoingPacket
{
    private readonly int _timeLeft;
    private readonly int _bluePoints;
    private readonly int _redPoints;
    private readonly bool _isRedTeam;
    private readonly Player _player;
    private readonly int _playerPoints;
	
    /**
     * Update a Secret Point Counter (used by client when receive ExCubeGameEnd)
     * @param timeLeft Time Left before Minigame's End
     * @param bluePoints Current Blue Team Points
     * @param redPoints Current Blue Team points
     * @param isRedTeam Is Player from Red Team?
     * @param player Player Instance
     * @param playerPoints Current Player Points
     */
    public ExCubeGameExtendedChangePointsPacket(int timeLeft, int bluePoints, int redPoints, bool isRedTeam, Player player, int playerPoints)
    {
        _timeLeft = timeLeft;
        _bluePoints = bluePoints;
        _redPoints = redPoints;
        _isRedTeam = isRedTeam;
        _player = player;
        _playerPoints = playerPoints;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLOCK_UP_SET_STATE);
        
        writer.WriteInt32(0);
        writer.WriteInt32(_timeLeft);
        writer.WriteInt32(_bluePoints);
        writer.WriteInt32(_redPoints);
        writer.WriteInt32(_isRedTeam);
        writer.WriteInt32(_player.ObjectId);
        writer.WriteInt32(_playerPoints);
    }
}