using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerPKChanged: IBaseEvent
{
	private readonly Player _player;
	private readonly int _oldPoints;
	private readonly int _newPoints;
	
	public OnPlayerPKChanged(Player player, int oldPoints, int newPoints)
	{
		_player = player;
		_oldPoints = oldPoints;
		_newPoints = newPoints;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getOldPoints()
	{
		return _oldPoints;
	}
	
	public int getNewPoints()
	{
		return _newPoints;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_PK_CHANGED;
	}
}