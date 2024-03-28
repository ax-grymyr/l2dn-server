using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerMentorStatus: EventBase
{
	private readonly Player _mentor;
	private readonly bool _isOnline;
	
	public OnPlayerMentorStatus(Player mentor, bool isOnline)
	{
		_mentor = mentor;
		_isOnline = isOnline;
	}
	
	public Player getMentor()
	{
		return _mentor;
	}
	
	public bool isMentorOnline()
	{
		return _isOnline;
	}
}