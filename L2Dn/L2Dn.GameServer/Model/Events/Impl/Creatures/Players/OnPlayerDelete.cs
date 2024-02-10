namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerDelete: IBaseEvent
{
	private readonly int _objectId;
	private readonly String _name;
	private readonly GameClient _client;
	
	public OnPlayerDelete(int objectId, String name, GameClient client)
	{
		_objectId = objectId;
		_name = name;
		_client = client;
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
		return EventType.ON_PLAYER_DELETE;
	}
}