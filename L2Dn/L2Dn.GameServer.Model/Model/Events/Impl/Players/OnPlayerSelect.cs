using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;
using L2Dn.GameServer.Network;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerSelect: TerminateEventBase
{
	private readonly Player _player;
	private readonly int _objectId;
	private readonly string _name;
	private readonly GameSession _client;
	
	public OnPlayerSelect(Player player, int objectId, string name, GameSession client)
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
	
	public string getName()
	{
		return _name;
	}
	
	public GameSession getClient()
	{
		return _client;
	}
}