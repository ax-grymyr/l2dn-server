using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.ItemContainers;

public class PlayerWarehouse: Warehouse
{
    private readonly Player _owner;

    public PlayerWarehouse(Player owner)
    {
        _owner = owner;
    }

    public override string getName()
    {
        return "Warehouse";
    }

    public override Player getOwner()
    {
        return _owner;
    }

    public override ItemLocation getBaseLocation()
    {
        return ItemLocation.WAREHOUSE;
    }

    public override bool validateCapacity(long slots)
    {
        return _items.size() + slots <= _owner.getWareHouseLimit();
    }
}