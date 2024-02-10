using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author Krunchy
 * @since 2.6.0.0
 */
public class OnPlayerProfessionCancel: IBaseEvent
{
	private readonly Player _player;
	private readonly int _classId;
	
	public OnPlayerProfessionCancel(Player player, int classId)
	{
		_player = player;
		_classId = classId;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getClassId()
	{
		return _classId;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_PROFESSION_CANCEL;
	}
}