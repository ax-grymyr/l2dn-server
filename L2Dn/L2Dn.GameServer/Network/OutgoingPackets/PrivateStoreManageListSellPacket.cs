using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PrivateStoreManageListSellPacket: IOutgoingPacket
{
    private readonly int _sendType;
    private readonly int _objId;
    private readonly long _playerAdena;
    private readonly bool _packageSale;
    private readonly ICollection<TradeItem> _itemList;
    private readonly ICollection<TradeItem> _sellList;
	
    public PrivateStoreManageListSellPacket(int sendType, Player player, bool isPackageSale)
    {
        _sendType = sendType;
        _objId = player.getObjectId();
        _playerAdena = player.getAdena();
        player.getSellList().updateItems();
        _packageSale = isPackageSale;
        _itemList = player.getInventory().getAvailableItems(player.getSellList());
        _sellList = player.getSellList().getItems();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PRIVATE_STORE_MANAGE_LIST);
        
        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt32(_itemList.Count);
            writer.WriteInt32(_itemList.Count);
            foreach (TradeItem item in _itemList)
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt64(item.getItem().getReferencePrice() * 2);
            }
        }
        else
        {
            writer.WriteInt32(_objId);
            writer.WriteInt32(_packageSale);
            writer.WriteInt64(_playerAdena);
            writer.WriteInt32(0);
            
            foreach (TradeItem item in _itemList)
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt64(item.getItem().getReferencePrice() * 2);
            }
            
            writer.WriteInt32(0);
            
            foreach (TradeItem item2 in _sellList)
            {
                InventoryPacketHelper.WriteItem(writer, item2);
                writer.WriteInt64(item2.getPrice());
                writer.WriteInt64(item2.getItem().getReferencePrice() * 2);
            }
        }
    }
}