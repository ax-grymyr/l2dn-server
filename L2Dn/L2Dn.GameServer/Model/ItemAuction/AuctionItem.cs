using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.ItemAuction;

public class AuctionItem
{
    private readonly int _auctionItemId;
    private readonly int _auctionLength;
    private readonly long _auctionInitBid;
	
    private readonly int _itemId;
    private readonly long _itemCount;
    private readonly StatSet _itemExtra;
	
    public AuctionItem(int auctionItemId, int auctionLength, long auctionInitBid, int itemId, long itemCount, StatSet itemExtra)
    {
        _auctionItemId = auctionItemId;
        _auctionLength = auctionLength;
        _auctionInitBid = auctionInitBid;
        _itemId = itemId;
        _itemCount = itemCount;
        _itemExtra = itemExtra;
    }
	
    public bool checkItemExists()
    {
        return ItemData.getInstance().getTemplate(_itemId) != null;
    }
	
    public int getAuctionItemId()
    {
        return _auctionItemId;
    }
	
    public int getAuctionLength()
    {
        return _auctionLength;
    }
	
    public long getAuctionInitBid()
    {
        return _auctionInitBid;
    }
	
    public int getItemId()
    {
        return _itemId;
    }
	
    public long getItemCount()
    {
        return _itemCount;
    }
	
    public Item createNewItemInstance()
    {
        Item item = new Item(IdManager.getInstance().getNextId(), _itemId);
        World.getInstance().addObject(item);
        item.setCount(_itemCount);
        item.setEnchantLevel(item.getTemplate().getDefaultEnchantLevel());
        return item;
    }
}