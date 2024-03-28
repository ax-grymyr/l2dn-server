using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCubeGameRemovePlayerPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly bool _isRedTeam;
	
    /**
     * Remove Player from Minigame Waiting List
     * @param player Player to Remove
     * @param isRedTeam Is Player from Red Team?
     */
    public ExCubeGameRemovePlayerPacket(Player player, bool isRedTeam)
    {
        _player = player;
        _isRedTeam = isRedTeam;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLOCK_UP_SET_LIST);
        
        writer.WriteInt32(2);
        writer.WriteInt32(-1);
        writer.WriteInt32(_isRedTeam);
        writer.WriteInt32(_player.getObjectId());
    }
}