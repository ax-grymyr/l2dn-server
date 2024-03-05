using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.StoreReviews;

public readonly struct ExPrivateStoreSearchItemPacket: IOutgoingPacket
{
    private readonly int _page;
    private readonly int _maxPage;
    private readonly int _nSize;
    private readonly List<ShopItem> _items;
	
    public ExPrivateStoreSearchItemPacket(int page, int maxPage, int nSize, List<ShopItem> items)
    {
        _page = page;
        _maxPage = maxPage;
        _nSize = nSize;
        _items = items;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PRIVATE_STORE_SEARCH_ITEM);
        
        writer.WriteByte((byte)_page); // cPage
        writer.WriteByte((byte)_maxPage); // cMaxPage
        writer.WriteInt32(_nSize); // nSize
        if (_nSize > 0)
        {
            for (int itemIndex = (_page - 1) * ExRequestPrivateStoreSearchList.MAX_ITEM_PER_PAGE; (itemIndex < (_page * ExRequestPrivateStoreSearchList.MAX_ITEM_PER_PAGE)) && (itemIndex < _items.Count); itemIndex++)
            {
                ShopItem shopItem = _items[itemIndex];
                writer.WriteSizedString(shopItem.getOwner().getName()); // Vendor name
                writer.WriteInt32(shopItem.getOwner().getObjectId());
                writer.WriteByte((byte)(shopItem.getStoreType() == PrivateStoreType.PACKAGE_SELL ? 2 : shopItem.getStoreType() == PrivateStoreType.SELL ? 0 : 1)); // store type (maybe "sold"/buy/Package (translated as Total Score...))
                writer.WriteInt64(shopItem.getPrice()); // Price
                writer.WriteInt32(shopItem.getOwner().getX()); // X
                writer.WriteInt32(shopItem.getOwner().getY()); // Y
                writer.WriteInt32(shopItem.getOwner().getZ()); // Z
                writer.WriteInt32(InventoryPacketHelper.CalculatePacketSize(shopItem.getItemInfo() /* , shopItem.getCount() */)); // size
                InventoryPacketHelper.WriteItem(writer, shopItem.getItemInfo(), shopItem.getCount()); // itemAssemble
            }
        }
    }
}