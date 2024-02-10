using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author Mathael
 */
public class OnPlayerSummonAgathion: IBaseEvent
{
	private readonly Player _player;
	private readonly int _agathionId;
	
	public OnPlayerSummonAgathion(Player player, int agathionId)
	{
		_player = player;
		_agathionId = agathionId;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getAgathionId()
	{
		return _agathionId;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_SUMMON_AGATHION;
	}
}