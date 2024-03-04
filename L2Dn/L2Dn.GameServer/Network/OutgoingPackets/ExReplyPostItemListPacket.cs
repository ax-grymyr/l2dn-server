using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExReplyPostItemListPacket: IOutgoingPacket
{
    private readonly int _sendType;
    private readonly Player _player;
    private readonly ICollection<Item> _itemList;
	
    public ExReplyPostItemListPacket(int sendType, Player player)
    {
        _sendType = sendType;
        _player = player;
        _itemList = _player.getInventory().getAvailableItems(true, false, false);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_REPLY_POST_ITEM_LIST);

        writer.WriteByte((byte)_sendType);
        writer.WriteInt32(_itemList.Count);
        if (_sendType == 2)
        {
            writer.WriteInt32(_itemList.Count);
            foreach (Item item in _itemList)
            {
                InventoryPacketHelper.WriteItem(writer, item);
            }
        }
    }
}