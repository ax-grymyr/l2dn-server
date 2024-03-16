using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerTransform: IBaseEvent
{
	private readonly Player _player;
	private readonly int _transformId;
	
	public OnPlayerTransform(Player player, int transformId)
	{
		_player = player;
		_transformId = transformId;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getTransformId()
	{
		return _transformId;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_TRANSFORM;
	}
}