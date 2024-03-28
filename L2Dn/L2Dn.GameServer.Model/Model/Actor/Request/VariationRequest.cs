using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Actor.Request;

public class VariationRequest: AbstractRequest
{
    private Item _augmented;
    private Item _mineral;
    private VariationInstance _augmentation;

    public VariationRequest(Player player): base(player)
    {
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void setAugmentedItem(int objectId)
    {
        _augmented = getActiveChar().getInventory().getItemByObjectId(objectId);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Item getAugmentedItem()
    {
        return _augmented;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void setMineralItem(int objectId)
    {
        _mineral = getActiveChar().getInventory().getItemByObjectId(objectId);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Item getMineralItem()
    {
        return _mineral;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void setAugment(VariationInstance augment)
    {
        _augmentation = augment;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public VariationInstance getAugment()
    {
        return _augmentation;
    }

    public override bool isUsing(int objectId)
    {
        return false;
    }
}