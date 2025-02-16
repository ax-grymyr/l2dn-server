using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCubeGameAddPlayerPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly bool _isRedTeam;
	
    /**
     * Add Player To Minigame Waiting List
     * @param player Player Instance
     * @param isRedTeam Is Player from Red Team?
     */
    public ExCubeGameAddPlayerPacket(Player player, bool isRedTeam)
    {
        _player = player;
        _isRedTeam = isRedTeam;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLOCK_UP_SET_LIST); // TODO packet code?

        writer.WriteInt32(1);
        writer.WriteInt32(-1);
        writer.WriteInt32(_isRedTeam);
        writer.WriteInt32(_player.ObjectId);
        writer.WriteString(_player.getName());
    }
}