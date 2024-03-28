using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct BuyListPacket: IOutgoingPacket
{
    private readonly int _listId;
    private readonly ICollection<Product> _list;
    private readonly long _money;
    private readonly int _inventorySlots;
    private readonly double _castleTaxRate;
	
    public BuyListPacket(ProductList list, Player player, double castleTaxRate)
    {
        _listId = list.getListId();
        _list = list.getProducts();
        _money = player.getAdena();
        _inventorySlots = player.getInventory().getNonQuestSize();
        _castleTaxRate = castleTaxRate;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BUY_SELL_LIST);
        
        writer.WriteInt32(0); // Type BUY
        writer.WriteInt64(_money); // current money
        writer.WriteInt32(_listId);
        writer.WriteInt32(_inventorySlots);
        writer.WriteInt16((short)_list.Count);
        foreach (Product product in _list)
        {
            if ((product.getCount() > 0) || !product.hasLimitedStock())
            {
                InventoryPacketHelper.WriteItem(writer, product);
                writer.WriteInt64((long)(product.getPrice() * (1.0 + _castleTaxRate + product.getBaseTaxRate())));
            }
        }
    }
}