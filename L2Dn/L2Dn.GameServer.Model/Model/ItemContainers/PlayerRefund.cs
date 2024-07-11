using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.ItemContainers;

public class PlayerRefund: ItemContainer
{
    private readonly Player _owner;

    public PlayerRefund(Player owner)
    {
        _owner = owner;
    }

    public override string getName()
    {
        return "Refund";
    }

    public override Player getOwner()
    {
        return _owner;
    }

    public override ItemLocation getBaseLocation()
    {
        return ItemLocation.REFUND;
    }

    protected override void addItem(Item item)
    {
        base.addItem(item);
        try
        {
            if (getSize() > 12)
            {
                Item? removedItem = _items.FirstOrDefault(); // TODO: ordered container
                if (removedItem != null && _items.remove(removedItem))
                {
                    ItemData.getInstance().destroyItem("ClearRefund", removedItem, getOwner(), null);
                    removedItem.updateDatabase(true);
                }
            }
        }
        catch (Exception e)
        {
            LOGGER.Error("addItem(): " + e);
        }
    }

    protected override void refreshWeight()
    {
    }

    public override void deleteMe()
    {
        try
        {
            foreach (Item item in _items)
            {
                ItemData.getInstance().destroyItem("ClearRefund", item, getOwner(), null);
                item.updateDatabase(true);
            }
        }
        catch (Exception e)
        {
            LOGGER.Error("deleteMe(): " + e);
        }

        _items.clear();
    }

    public override void restore()
    {
    }
}