using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Request;

public class PartyRequest: AbstractRequest
{
    private readonly Player _targetPlayer;
    private readonly Party _party;

    public PartyRequest(Player player, Player targetPlayer, Party party): base(player)
    {
        ArgumentNullException.ThrowIfNull(targetPlayer);
        ArgumentNullException.ThrowIfNull(party);
        _targetPlayer = targetPlayer;
        _party = party;
    }

    public Player getTargetPlayer()
    {
        return _targetPlayer;
    }

    public Party getParty()
    {
        return _party;
    }

    public override bool isUsing(int objectId)
    {
        return false;
    }

    public override void onTimeout()
    {
        base.onTimeout();
        getActiveChar().removeRequest<PartyRequest>();
        _targetPlayer.removeRequest<PartyRequest>();
    }
}
