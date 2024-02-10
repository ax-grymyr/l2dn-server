using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerSelect: IBaseEvent
{
	private readonly Player _player;
	private readonly int _objectId;
	private readonly String _name;
	private readonly GameClient _client;
	
	public OnPlayerSelect(Player player, int objectId, String name, GameClient client)
	{
		_player = player;
		_objectId = objectId;
		_name = name;
		_client = client;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getObjectId()
	{
		return _objectId;
	}
	
	public String getName()
	{
		return _name;
	}
	
	public GameClient getClient()
	{
		return _client;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_SELECT;
	}
}