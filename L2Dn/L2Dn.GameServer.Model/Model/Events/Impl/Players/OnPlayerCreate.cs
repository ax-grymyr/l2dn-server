using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerCreate: EventBase
{
	private readonly Player _player;
	private readonly int _objectId;
	private readonly String _name;
	private readonly GameSession _client;
	
	public OnPlayerCreate(Player player, int objectId, String name, GameSession client)
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
	
	public GameSession getClient()
	{
		return _client;
	}
}