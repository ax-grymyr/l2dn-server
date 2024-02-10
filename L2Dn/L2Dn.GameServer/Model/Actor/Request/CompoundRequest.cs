using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Actor.Request;

public class CompoundRequest: AbstractRequest
{
    private int _itemOne;
    private int _itemTwo;

    public CompoundRequest(Player player): base(player)
    {
    }

    public Item getItemOne()
    {
        return getActiveChar().getInventory().getItemByObjectId(_itemOne);
    }

    public void setItemOne(int itemOne)
    {
        _itemOne = itemOne;
    }

    public Item getItemTwo()
    {
        return getActiveChar().getInventory().getItemByObjectId(_itemTwo);
    }

    public void setItemTwo(int itemTwo)
    {
        _itemTwo = itemTwo;
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
        return (objectId > 0) && ((objectId == _itemOne) || (objectId == _itemTwo));
    }
}
