using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerMoveRequest: TerminateEventBase
{
	private readonly Player _player;
	private readonly Location3D _location;
	
	public OnPlayerMoveRequest(Player player, Location3D loc)
	{
		_player = player;
		_location = loc;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public Location3D getLocation()
	{
		return _location;
	}
}