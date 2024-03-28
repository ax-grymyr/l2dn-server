using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author Krunchy
 * @since 2.6.0.0
 */
public class OnPlayerProfessionCancel: EventBase
{
	private readonly Player _player;
	private readonly CharacterClass _classId;
	
	public OnPlayerProfessionCancel(Player player, CharacterClass classId)
	{
		_player = player;
		_classId = classId;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public CharacterClass getClassId()
	{
		return _classId;
	}
}