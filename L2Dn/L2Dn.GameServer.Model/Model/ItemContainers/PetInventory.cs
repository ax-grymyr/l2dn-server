using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.ItemContainers;

public class PetInventory: Inventory
{
    private readonly Pet _owner;

    public PetInventory(Pet owner)
    {
        _owner = owner;
    }

    public override Pet getOwner()
    {
        return _owner;
    }

    public override int getOwnerId()
    {
        return getOwner() == null ? 0 : _owner.getControlObjectId();
    }

    /**
     * Refresh the weight of equipment loaded
     */
    protected override void refreshWeight()
    {
        base.refreshWeight();
        _owner.updateAndBroadcastStatus(1);
    }

    public override ICollection<Item> getItems()
    {
        List<Item> equippedItems = new();
        foreach (Item item in base.getItems())
        {
            if (item.isEquipped())
            {
                equippedItems.Add(item);
            }
        }

        return equippedItems;
    }

    public bool validateCapacity(Item item)
    {
        int slots = 0;
        if (!(item.isStackable() && getItemByItemId(item.Id) != null) &&
            !item.getTemplate().hasExImmediateEffect())
        {
            slots++;
        }

        return validateCapacity(slots);
    }

    public override bool validateCapacity(long slots)
    {
        return _items.size() + slots <= _owner.getInventoryLimit();
    }

    public bool validateWeight(Item item, long count)
    {
        long weight = 0;
        ItemTemplate? template = ItemData.getInstance().getTemplate(item.Id);
        if (template == null)
        {
            return false;
        }

        weight += count * template.getWeight();
        return validateWeight(weight);
    }

    public override bool validateWeight(long weight)
    {
        return _totalWeight + weight <= _owner.getMaxLoad();
    }

    public override ItemLocation getBaseLocation()
    {
        return ItemLocation.PET;
    }

    protected override ItemLocation getEquipLocation()
    {
        return ItemLocation.PET_EQUIP;
    }

    public void transferItemsToOwner()
    {
        foreach (Item item in _items)
        {
            getOwner().transferItem("return", item.ObjectId, item.getCount(), getOwner().getOwner().getInventory(),
                getOwner().getOwner(), getOwner());
        }
    }
}