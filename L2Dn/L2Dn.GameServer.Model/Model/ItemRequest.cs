namespace L2Dn.GameServer.Model;

public class ItemRequest
{
    int _objectId;
    int _itemId;
    long _count;
    long _price;
	
    public ItemRequest(int objectId, long count, long price)
    {
        _objectId = objectId;
        _count = count;
        _price = price;
    }
	
    public ItemRequest(int objectId, int itemId, long count, long price)
    {
        _objectId = objectId;
        _itemId = itemId;
        _count = count;
        _price = price;
    }
	
    public int getObjectId()
    {
        return _objectId;
    }
	
    public int getItemId()
    {
        return _itemId;
    }
	
    public void setCount(long count)
    {
        _count = count;
    }
	
    public long getCount()
    {
        return _count;
    }
	
    public long getPrice()
    {
        return _price;
    }
	
    public override int GetHashCode()
    {
        return _objectId;
    }
	
    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }
        if (!(obj is ItemRequest))
        {
            return false;
        }
        
        return (_objectId != ((ItemRequest) obj)._objectId);
    }
}
