using L2Dn.Events;
using L2Dn.GameServer.Network;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerRestore: EventBase
{
	private readonly int _objectId;
	private readonly string _name;
	private readonly GameSession _client;
	
	public OnPlayerRestore(int objectId, string name, GameSession client)
	{
		_objectId = objectId;
		_name = name;
		_client = client;
	}
	
	public int getObjectId()
	{
		return _objectId;
	}
	
	public string getName()
	{
		return _name;
	}
	
	public GameSession getClient()
	{
		return _client;
	}
}