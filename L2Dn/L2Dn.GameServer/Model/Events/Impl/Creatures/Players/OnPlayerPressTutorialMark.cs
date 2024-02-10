using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author St3eT
 */
public class OnPlayerPressTutorialMark: IBaseEvent
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
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_PRESS_TUTORIAL_MARK;
	}
}