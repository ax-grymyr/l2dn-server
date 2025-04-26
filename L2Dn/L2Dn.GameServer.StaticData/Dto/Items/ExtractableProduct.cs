namespace L2Dn.GameServer.Model;

public class ExtractableProduct
{
    private readonly int _id;
    private readonly long _min;
    private readonly long _max;
    private readonly int _chance;
    private readonly int _minEnchant;
    private readonly int _maxEnchant;

    /**
     * Create Extractable product
     * @param id create item id
     * @param min item count max
     * @param max item count min
     * @param chance chance for creating
     * @param minEnchant item min enchant
     * @param maxEnchant item max enchant
     */
    public ExtractableProduct(int id, long min, long max, double chance, int minEnchant, int maxEnchant)
    {
        _id = id;
        _min = min;
        _max = max;
        _chance = (int)(chance * 1000);
        _minEnchant = minEnchant;
        _maxEnchant = maxEnchant;
    }

    public int getId()
    {
        return _id;
    }

    public long getMin()
    {
        return _min;
    }

    public long getMax()
    {
        return _max;
    }

    public int getChance()
    {
        return _chance;
    }

    public int getMinEnchant()
    {
        return _minEnchant;
    }

    public int getMaxEnchant()
    {
        return _maxEnchant;
    }
}
