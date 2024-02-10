namespace L2Dn.GameServer.Model.Actor.Request;

public class BlessingItemRequest: AbstractRequest
{
    private volatile int _blessScrollId;

    public BlessingItemRequest(Player player, int itemId): base(player)
    {
        _blessScrollId = itemId;
    }

    public int getBlessScrollId()
    {
        return _blessScrollId;
    }

    public override bool isUsing(int objectId)
    {
        return false;
    }
}