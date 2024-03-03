using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCubeGameTeamListPacket: IOutgoingPacket
{
    // Players Lists
    private readonly List<Player> _bluePlayers;
    private readonly List<Player> _redPlayers;
    // Common Values
    private readonly int _roomNumber;
	
    /**
     * Show Minigame Waiting List to Player
     * @param redPlayers Red Players List
     * @param bluePlayers Blue Players List
     * @param roomNumber Arena/Room ID
     */
    public ExCubeGameTeamListPacket(List<Player> redPlayers, List<Player> bluePlayers, int roomNumber)
    {
        _redPlayers = redPlayers;
        _bluePlayers = bluePlayers;
        _roomNumber = roomNumber - 1;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLOCK_UP_SET_LIST);
        
        writer.WriteInt32(0);
        writer.WriteInt32(_roomNumber);
        writer.WriteInt32(-1);
        writer.WriteInt32(_bluePlayers.Count);
        foreach (Player player in _bluePlayers)
        {
            writer.WriteInt32(player.getObjectId());
            writer.WriteString(player.getName());
        }
        writer.WriteInt32(_redPlayers.Count);
        foreach (Player player in _redPlayers)
        {
            writer.WriteInt32(player.getObjectId());
            writer.WriteString(player.getName());
        }
    }
}