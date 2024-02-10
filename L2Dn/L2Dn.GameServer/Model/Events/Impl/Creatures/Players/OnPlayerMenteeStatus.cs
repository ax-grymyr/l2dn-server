using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerMenteeStatus: IBaseEvent
{
	private readonly Player _mentee;
	private readonly bool _isOnline;
	
	public OnPlayerMenteeStatus(Player mentee, bool isOnline)
	{
		_mentee = mentee;
		_isOnline = isOnline;
	}
	
	public Player getMentee()
	{
		return _mentee;
	}
	
	public bool isMenteeOnline()
	{
		return _isOnline;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_MENTEE_STATUS;
	}
}