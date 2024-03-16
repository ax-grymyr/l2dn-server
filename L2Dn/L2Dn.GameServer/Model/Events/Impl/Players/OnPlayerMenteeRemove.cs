using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerMenteeRemove: IBaseEvent
{
	private readonly Player _mentor;
	private readonly Mentee _mentee;
	
	public OnPlayerMenteeRemove(Player mentor, Mentee mentee)
	{
		_mentor = mentor;
		_mentee = mentee;
	}
	
	public Player getMentor()
	{
		return _mentor;
	}
	
	public Mentee getMentee()
	{
		return _mentee;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_MENTEE_REMOVE;
	}
}