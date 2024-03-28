using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerMoveRequest: TerminateEventBase
{
	private readonly Player _player;
	private readonly Location _location;
	
	public OnPlayerMoveRequest(Player player, Location loc)
	{
		_player = player;
		_location = loc;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public Location getLocation()
	{
		return _location;
	}
}