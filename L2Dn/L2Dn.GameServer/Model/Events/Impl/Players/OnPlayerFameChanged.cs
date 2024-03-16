using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerFameChanged: IBaseEvent
{
	private readonly Player _player;
	private readonly int _oldFame;
	private readonly int _newFame;
	
	public OnPlayerFameChanged(Player player, int oldFame, int newFame)
	{
		_player = player;
		_oldFame = oldFame;
		_newFame = newFame;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getOldFame()
	{
		return _oldFame;
	}
	
	public int getNewFame()
	{
		return _newFame;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_FAME_CHANGED;
	}
}