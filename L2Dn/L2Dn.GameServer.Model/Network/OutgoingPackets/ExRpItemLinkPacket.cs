using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExRpItemLinkPacket: IOutgoingPacket
{
    private readonly Item _item;
	
    public ExRpItemLinkPacket(Item item)
    {
        _item = item;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RP_ITEM_LINK);
        
        Player player = _item.getActingPlayer();
        if ((player != null) && player.isOnline())
        {
            writer.WriteByte(1);
            writer.WriteInt32(player.ObjectId);
        }
        else
        {
            writer.WriteByte(0);
            writer.WriteInt32(0);
        }
        
        InventoryPacketHelper.WriteItem(writer, _item);
    }
}