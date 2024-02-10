using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author St3eT
 */
public class OnPlayerSocialAction: IBaseEvent
{
	private readonly Player _player;
	private readonly int _socialActionId;

	public OnPlayerSocialAction(Player player, int socialActionId)
	{
		_player = player;
		_socialActionId = socialActionId;
	}

	public Player getPlayer()
	{
		return _player;
	}

	public int getSocialActionId()
	{
		return _socialActionId;
	}

	public EventType getType()
	{
		return EventType.ON_PLAYER_SOCIAL_ACTION;
	}
}