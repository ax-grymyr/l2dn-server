namespace L2Dn.GameServer.Model;

public class PremiumItem
{
    private readonly int _itemId;
    private long _count;
    private readonly string _sender;
	
    public PremiumItem(int itemid, long count, string sender)
    {
        _itemId = itemid;
        _count = count;
        _sender = sender;
    }
	
    public void updateCount(long newcount)
    {
        _count = newcount;
    }
	
    public int getItemId()
    {
        return _itemId;
    }
	
    public long getCount()
    {
        return _count;
    }
	
    public string getSender()
    {
        return _sender;
    }
}
