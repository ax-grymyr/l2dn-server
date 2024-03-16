using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerMenteeAdd: IBaseEvent
{
	private readonly Player _mentor;
	private readonly Player _mentee;
	
	public OnPlayerMenteeAdd(Player mentor, Player mentee)
	{
		_mentor = mentor;
		_mentee = mentee;
	}
	
	public Player getMentor()
	{
		return _mentor;
	}
	
	public Player getMentee()
	{
		return _mentee;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_MENTEE_ADD;
	}
}