namespace L2Dn.GameServer.Model;

public class SeedProduction
{
    private readonly int _seedId;
    private readonly long _price;
    private readonly long _startAmount;
    private long _amount;
	
    public SeedProduction(int id, long amount, long price, long startAmount)
    {
        _seedId = id;
        _amount = amount;
        _price = price;
        _startAmount = startAmount;
    }
	
    public int getId()
    {
        return _seedId;
    }
	
    public long getAmount()
    {
        return Interlocked.Read(ref _amount);
    }
	
    public long getPrice()
    {
        return _price;
    }
	
    public long getStartAmount()
    {
        return _startAmount;
    }
	
    public void setAmount(long amount)
    {
        Interlocked.Exchange(ref _amount, amount);
    }
	
    public bool decreaseAmount(long value)
    {
        long current;
        long next;
        do
        {
            current = Interlocked.Read(ref _amount);
            next = current - value;
            if (next < 0)
            {
                return false;
            }
        }
        while (Interlocked.CompareExchange(ref _amount, next, current) != current);
        
        return true;
    }
}