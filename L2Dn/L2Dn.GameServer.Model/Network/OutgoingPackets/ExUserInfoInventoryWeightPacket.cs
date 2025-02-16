using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

/**
 * @author Sdw
 */
public readonly struct ExUserInfoInventoryWeightPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExUserInfoInventoryWeightPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_INFO_INVEN_WEIGHT);
        writer.WriteInt32(_player.ObjectId);
        writer.WriteInt32(_player.getCurrentLoad());
        writer.WriteInt32(_player.getMaxLoad());
    }
}