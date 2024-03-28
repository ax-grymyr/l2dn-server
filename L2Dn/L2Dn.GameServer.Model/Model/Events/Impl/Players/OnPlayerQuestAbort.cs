using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author Sdw
 */
public class OnPlayerQuestAbort: EventBase
{
	private readonly Player _player;
	private readonly int _questId;
	
	public OnPlayerQuestAbort(Player player, int questId)
	{
		_player = player;
		_questId = questId;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getQuestId()
	{
		return _questId;
	}
}