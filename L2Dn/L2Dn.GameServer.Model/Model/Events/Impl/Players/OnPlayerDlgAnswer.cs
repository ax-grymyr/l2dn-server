using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerDlgAnswer: TerminateEventBase
{
	private readonly Player _player;
	private readonly SystemMessageId _messageId;
	private readonly int _answer;
	private readonly int _requesterId;
	
	public OnPlayerDlgAnswer(Player player, SystemMessageId messageId, int answer, int requesterId)
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
	
	public SystemMessageId getMessageId()
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
}