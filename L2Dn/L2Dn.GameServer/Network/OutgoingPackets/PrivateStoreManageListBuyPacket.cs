using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PrivateStoreManageListBuyPacket: IOutgoingPacket
{
    private readonly int _sendType;
    private readonly int _objId;
    private readonly long _playerAdena;
    private readonly ICollection<Item> _itemList;
    private readonly ICollection<TradeItem> _buyList;
	
    public PrivateStoreManageListBuyPacket(int sendType, Player player)
    {
        _sendType = sendType;
        _objId = player.getObjectId();
        _playerAdena = player.getAdena();
        _itemList = player.getInventory().getUniqueItems(false, true);
        _buyList = player.getBuyList().getItems();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PRIVATE_STORE_BUY_MANAGE_LIST);
        
        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt32(_itemList.Count);
            writer.WriteInt32(_itemList.Count);
            foreach (Item item in _itemList)
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt64(item.getTemplate().getReferencePrice() * 2);
            }
        }
        else
        {
            writer.WriteInt32(_objId);
            writer.WriteInt64(_playerAdena);
            writer.WriteInt32(0);
            foreach (Item item in _itemList)
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt64(item.getTemplate().getReferencePrice() * 2);
            }
            
            writer.WriteInt32(0);
            foreach (TradeItem item2 in _buyList)
            {
                InventoryPacketHelper.WriteItem(writer, item2);
                writer.WriteInt64(item2.getPrice());
                writer.WriteInt64(item2.getItem().getReferencePrice() * 2);
                writer.WriteInt64(item2.getCount());
            }
        }
    }
}