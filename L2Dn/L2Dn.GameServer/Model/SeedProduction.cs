namespace L2Dn.GameServer.Model;

public class SeedProduction
{
    private readonly int _seedId;
    private readonly long _price;
    private readonly long _startAmount;
    private readonly AtomicLong _amount;
	
    public SeedProduction(int id, long amount, long price, long startAmount)
    {
        _seedId = id;
        _amount = new AtomicLong(amount);
        _price = price;
        _startAmount = startAmount;
    }
	
    public int getId()
    {
        return _seedId;
    }
	
    public long getAmount()
    {
        return _amount.get();
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
        _amount.set(amount);
    }
	
    public bool decreaseAmount(long value)
    {
        long current;
        long next;
        do
        {
            current = _amount.get();
            next = current - value;
            if (next < 0)
            {
                return false;
            }
        }
        while (!_amount.compareAndSet(current, next));
        return true;
    }
}