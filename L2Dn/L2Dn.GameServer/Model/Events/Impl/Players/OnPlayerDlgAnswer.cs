using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerDlgAnswer: IBaseEvent
{
	private readonly Player _player;
	private readonly int _messageId;
	private readonly int _answer;
	private readonly int _requesterId;
	
	public OnPlayerDlgAnswer(Player player, int messageId, int answer, int requesterId)
	{
		_player = player;
		_messageId = messageId;
		_answer = answer;
		_requesterId = requesterId;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getMessageId()
	{
		return _messageId;
	}
	
	public int getAnswer()
	{
		return _answer;
	}
	
	public int getRequesterId()
	{
		return _requesterId;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_DLG_ANSWER;
	}
}