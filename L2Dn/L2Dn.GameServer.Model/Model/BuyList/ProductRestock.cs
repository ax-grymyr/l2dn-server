namespace L2Dn.GameServer.Model.BuyList;

public class ProductRestock(long count, TimeSpan delay)
{
    public long Count => count;
    public TimeSpan Delay => delay;
}