using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author St3eT
 */
public class OnPlayerPressTutorialMark: EventBase
{
	private readonly Player _player;
	private readonly int _markId;
	
	public OnPlayerPressTutorialMark(Player player, int markId)
	{
		_player = player;
		_markId = markId;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getMarkId()
	{
		return _markId;
	}
}