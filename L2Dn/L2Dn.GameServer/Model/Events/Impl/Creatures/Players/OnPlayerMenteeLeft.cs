using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerMenteeLeft: IBaseEvent
{
	private readonly Mentee _mentor;
	private readonly Player _mentee;
	
	public OnPlayerMenteeLeft(Mentee mentor, Player mentee)
	{
		_mentor = mentor;
		_mentee = mentee;
	}
	
	public Mentee getMentor()
	{
		return _mentor;
	}
	
	public Player getMentee()
	{
		return _mentee;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_MENTEE_LEFT;
	}
}