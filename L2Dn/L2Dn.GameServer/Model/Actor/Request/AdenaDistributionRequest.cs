namespace L2Dn.GameServer.Model.Actor.Request;

public class AdenaDistributionRequest: AbstractRequest
{
    private readonly Player _distributor;
    private readonly List<Player> _players;
    private readonly int _adenaObjectId;
    private readonly long _adenaCount;

    public AdenaDistributionRequest(Player player, Player distributor, List<Player> players, int adenaObjectId,
        long adenaCount): base(player)
    {
        _distributor = distributor;
        _adenaObjectId = adenaObjectId;
        _players = players;
        _adenaCount = adenaCount;
    }

    public Player getDistributor()
    {
        return _distributor;
    }

    public List<Player> getPlayers()
    {
        return _players;
    }

    public int getAdenaObjectId()
    {
        return _adenaObjectId;
    }

    public long getAdenaCount()
    {
        return _adenaCount;
    }

    public override bool isUsing(int objectId)
    {
        return objectId == _adenaObjectId;
    }

    public override void onTimeout()
    {
        base.onTimeout();
        _players.ForEach(p =>
        {
            p.removeRequest<AdenaDistributionRequest>();
            p.sendPacket(ExDivideAdenaCancel.STATIC_PACKET);
        });
    }
}
