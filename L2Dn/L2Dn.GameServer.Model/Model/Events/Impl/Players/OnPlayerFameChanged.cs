using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerFameChanged: EventBase
{
	private readonly Player _player;
	private readonly int _oldFame;
	private readonly int _newFame;
	
	public OnPlayerFameChanged(Player player, int oldFame, int newFame)
	{
		_player = player;
		_oldFame = oldFame;
		_newFame = newFame;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getOldFame()
	{
		return _oldFame;
	}
	
	public int getNewFame()
	{
		return _newFame;
	}
}