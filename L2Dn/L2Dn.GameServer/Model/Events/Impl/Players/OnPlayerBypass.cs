using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerBypass: IBaseEvent
{
	private readonly Player _player;
	private readonly String _command;
	
	public OnPlayerBypass(Player player, String command)
	{
		_player = player;
		_command = command;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public String getCommand()
	{
		return _command;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_BYPASS;
	}
}