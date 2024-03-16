using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerChat: IBaseEvent
{
	private readonly Player _player;
	private readonly String _target;
	private readonly String _text;
	private readonly ChatType _type;
	
	public OnPlayerChat(Player player, String target, String text, ChatType type)
	{
		_player = player;
		_target = target;
		_text = text;
		_type = type;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public String getTarget()
	{
		return _target;
	}
	
	public String getText()
	{
		return _text;
	}
	
	public ChatType getChatType()
	{
		return _type;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_CHAT;
	}
}