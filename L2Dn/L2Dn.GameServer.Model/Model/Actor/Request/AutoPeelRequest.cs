using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Actor.Request;

public class AutoPeelRequest: AbstractRequest
{
    private readonly Item _item;
    private long _totalPeelCount;
    private long _remainingPeelCount;

    public AutoPeelRequest(Player player, Item item): base(player)
    {
        _item = item;
    }

    public Item getItem()
    {
        return _item;
    }

    public long getTotalPeelCount()
    {
        return _totalPeelCount;
    }

    public void setTotalPeelCount(long count)
    {
        _totalPeelCount = count;
    }

    public long getRemainingPeelCount()
    {
        return _remainingPeelCount;
    }

    public void setRemainingPeelCount(long count)
    {
        _remainingPeelCount = count;
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
        return _item.getObjectId() == objectId;
    }
}
