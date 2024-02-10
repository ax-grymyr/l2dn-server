using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model;

public class ExtractableProductItem
{
    private readonly List<RestorationItemHolder> _items;
    private readonly double _chance;

    public ExtractableProductItem(List<RestorationItemHolder> items, double chance)
    {
        _items = items;
        _chance = chance;
    }

    /**
     * @return the the production list.
     */
    public List<RestorationItemHolder> getItems()
    {
        return _items;
    }

    /**
     * @return the chance of the production list.
     */
    public double getChance()
    {
        return _chance;
    }
}
