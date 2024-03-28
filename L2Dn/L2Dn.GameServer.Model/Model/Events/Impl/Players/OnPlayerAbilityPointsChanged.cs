using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author St3eT
 */
public class OnPlayerAbilityPointsChanged: EventBase
{
	private readonly Player _player;
	private readonly int _newAbilityPoints;
	private readonly int _oldAbilityPoints;
	
	public OnPlayerAbilityPointsChanged(Player player, int newAbilityPoints, int oldAbilityPoints)
	{
		_player = player;
		_newAbilityPoints = newAbilityPoints;
		_oldAbilityPoints = oldAbilityPoints;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public long getNewAbilityPoints()
	{
		return _newAbilityPoints;
	}
	
	public long getOldAbilityPoints()
	{
		return _oldAbilityPoints;
	}
}