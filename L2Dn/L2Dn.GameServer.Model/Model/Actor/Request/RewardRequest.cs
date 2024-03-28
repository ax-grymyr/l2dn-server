namespace L2Dn.GameServer.Model.Actor.Request;

public class RewardRequest: AbstractRequest
{
    public RewardRequest(Player player): base(player)
    {
    }

    public override bool isUsing(int objectId)
    {
        return false;
    }
}