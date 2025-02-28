using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Actor.Request;

public class ShapeShiftingItemRequest: AbstractRequest
{
    private Item _appearanceStone;
    private Item? _appearanceExtractItem;

    public ShapeShiftingItemRequest(Player player, Item appearanceStone): base(player)
    {
        _appearanceStone = appearanceStone;
    }

    public Item getAppearanceStone()
    {
        return _appearanceStone;
    }

    public void setAppearanceStone(Item appearanceStone)
    {
        _appearanceStone = appearanceStone;
    }

    public Item? getAppearanceExtractItem()
    {
        return _appearanceExtractItem;
    }

    public void setAppearanceExtractItem(Item? appearanceExtractItem)
    {
        _appearanceExtractItem = appearanceExtractItem;
    }

    public override bool isItemRequest()
    {
        return true;
    }

    public override bool canWorkWith(AbstractRequest request)
    {
        return !request.isItemRequest();
    }

    public override bool isUsing(int objectId)
    {
        if (_appearanceStone == null || _appearanceExtractItem == null)
        {
            return false;
        }

        return objectId > 0 && (objectId == _appearanceStone.ObjectId ||
                                  objectId == _appearanceExtractItem.ObjectId);
    }
}