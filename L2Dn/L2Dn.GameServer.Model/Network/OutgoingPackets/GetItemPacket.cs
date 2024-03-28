using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GetItemPacket: IOutgoingPacket
{
    private readonly Item _item;
    private readonly int _playerId;
	
    public GetItemPacket(Item item, int playerId)
    {
        _item = item;
        _playerId = playerId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GET_ITEM);
        
        writer.WriteInt32(_playerId);
        writer.WriteInt32(_item.getObjectId());
        writer.WriteInt32(_item.getX());
        writer.WriteInt32(_item.getY());
        writer.WriteInt32(_item.getZ());
    }
}