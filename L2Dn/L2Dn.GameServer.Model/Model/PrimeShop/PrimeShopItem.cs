using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.PrimeShop;

public class PrimeShopItem: ItemHolder
{
    private readonly int _weight;
    private readonly int _isTradable;

    public PrimeShopItem(int itemId, int count, int weight, int isTradable): base(itemId, count)
    {
        _weight = weight;
        _isTradable = isTradable;
    }

    public int getWeight()
    {
        return _weight;
    }

    public int isTradable()
    {
        return _isTradable;
    }
}