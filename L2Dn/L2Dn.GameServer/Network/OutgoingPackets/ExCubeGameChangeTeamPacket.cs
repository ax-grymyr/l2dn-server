using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCubeGameChangeTeamPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly bool _fromRedTeam;
	
    /**
     * Move Player from Team x to Team y
     * @param player Player Instance
     * @param fromRedTeam Is Player from Red Team?
     */
    public ExCubeGameChangeTeamPacket(Player player, bool fromRedTeam)
    {
        _player = player;
        _fromRedTeam = fromRedTeam;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLOCK_UP_SET_LIST);
        
        writer.WriteInt32(5);
        writer.WriteInt32(_player.getObjectId());
        writer.WriteInt32(_fromRedTeam);
        writer.WriteInt32(!_fromRedTeam);
    }
}