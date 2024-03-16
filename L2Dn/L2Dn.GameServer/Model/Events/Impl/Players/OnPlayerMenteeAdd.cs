using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerMenteeAdd: EventBase
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
}