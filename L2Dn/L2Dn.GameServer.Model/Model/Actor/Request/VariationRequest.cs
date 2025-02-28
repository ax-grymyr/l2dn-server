using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Actor.Request;

public class VariationRequest: AbstractRequest
{
    private Item? _augmented;
    private Item? _mineral;
    private VariationInstance? _augmentation;

    public VariationRequest(Player player): base(player)
    {
    }

    public void setAugmentedItem(int objectId)
    {
        _augmented = getActiveChar().getInventory().getItemByObjectId(objectId);
    }

    public Item? getAugmentedItem()
    {
        return _augmented;
    }

    public void setMineralItem(int objectId)
    {
        _mineral = getActiveChar().getInventory().getItemByObjectId(objectId);
    }

    public Item? getMineralItem()
    {
        return _mineral;
    }

    public void setAugment(VariationInstance? augment)
    {
        _augmentation = augment;
    }

    public VariationInstance? getAugment()
    {
        return _augmentation;
    }

    public override bool isUsing(int objectId)
    {
        return false;
    }
}