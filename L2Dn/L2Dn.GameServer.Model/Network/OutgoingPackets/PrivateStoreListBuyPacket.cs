using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PrivateStoreListBuyPacket: IOutgoingPacket
{
    private readonly int _objId;
    private readonly long _playerAdena;
    private readonly ICollection<TradeItem> _items;
	
    public PrivateStoreListBuyPacket(Player player, Player storePlayer)
    {
        _objId = storePlayer.getObjectId();
        _playerAdena = player.getAdena();
        storePlayer.getSellList().updateItems(); // Update SellList for case inventory content has changed
        _items = storePlayer.getBuyList().getAvailableItems(player.getInventory());
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PRIVATE_STORE_BUY_LIST);
        
        writer.WriteInt32(_objId);
        writer.WriteInt64(_playerAdena);
        writer.WriteInt32(0); // Viewer's item count?
        writer.WriteInt32(_items.Count);
        int slotNumber = 0;
        foreach (TradeItem item in _items)
        {
            slotNumber++;
            InventoryPacketHelper.WriteItem(writer, item);
            writer.WriteInt32(slotNumber); // Slot in shop
            writer.WriteInt64(item.getPrice());
            writer.WriteInt64(item.getItem().getReferencePrice() * 2);
            writer.WriteInt64(item.getStoreCount());
        }
    }
}