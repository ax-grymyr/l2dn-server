namespace L2Dn.GameServer.Model.Actor.Request;

public class RandomCraftRequest: AbstractRequest
{
    public RandomCraftRequest(Player player): base(player)
    {
    }

    public override bool isUsing(int objectId)
    {
        return false;
    }
}
