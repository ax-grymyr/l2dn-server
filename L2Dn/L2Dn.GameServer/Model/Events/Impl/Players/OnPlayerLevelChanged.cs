using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerLevelChanged: EventBase
{
	private readonly Player _player;
	private readonly int _oldLevel;
	private readonly int _newLevel;
	
	public OnPlayerLevelChanged(Player player, int oldLevel, int newLevel)
	{
		_player = player;
		_oldLevel = oldLevel;
		_newLevel = newLevel;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getOldLevel()
	{
		return _oldLevel;
	}
	
	public int getNewLevel()
	{
		return _newLevel;
	}
}