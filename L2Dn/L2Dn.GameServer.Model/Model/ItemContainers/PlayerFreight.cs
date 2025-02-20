using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.ItemContainers;

public class PlayerFreight: ItemContainer
{
    private readonly Player _owner;
    private readonly int _ownerId;

    public PlayerFreight(int objectId)
    {
        _owner = null;
        _ownerId = objectId;
        restore();
    }

    public PlayerFreight(Player owner)
    {
        _owner = owner;
        _ownerId = owner.ObjectId;
    }

    public override int getOwnerId()
    {
        return _ownerId;
    }

    public override Player getOwner()
    {
        return _owner;
    }

    public override ItemLocation getBaseLocation()
    {
        return ItemLocation.FREIGHT;
    }

    public override string getName()
    {
        return "Freight";
    }

    public override bool validateCapacity(long slots)
    {
        return getSize() + slots <= Config.ALT_FREIGHT_SLOTS;
    }

    protected override void refreshWeight()
    {
    }
}