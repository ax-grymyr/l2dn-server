using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerReputationChanged: EventBase
{
	private readonly Player _player;
	private readonly int _oldReputation;
	private readonly int _newReputation;
	
	public OnPlayerReputationChanged(Player player, int oldReputation, int newReputation)
	{
		_player = player;
		_oldReputation = oldReputation;
		_newReputation = newReputation;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getOldReputation()
	{
		return _oldReputation;
	}
	
	public int getNewReputation()
	{
		return _newReputation;
	}
}