using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Network.Enums;

public class ShopItem
{
    private readonly TradeItem _item;
    private readonly Player _owner;
    private readonly PrivateStoreType _storeType;
		
    public ShopItem(TradeItem item, Player owner, PrivateStoreType storeType)
    {
        _item = item;
        _owner = owner;
        _storeType = storeType;
    }
		
    public long getCount()
    {
        return _item.getCount();
    }
		
    public ItemInfo getItemInfo()
    {
        return new ItemInfo(_item);
    }
		
    public Player getOwner()
    {
        return _owner;
    }
		
    public PrivateStoreType getStoreType()
    {
        return _storeType;
    }
		
    public long getPrice()
    {
        return _item.getPrice();
    }
}