using L2Dn.Events;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerChat: EventBase
{
	private readonly Player _player;
	private readonly string _target;
	private readonly string _text;
	private readonly ChatType _type;
	
	public OnPlayerChat(Player player, string target, string text, ChatType type)
	{
		_player = player;
		_target = target;
		_text = text;
		_type = type;
		FilteredText = text;
		FilteredType = type;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public string getTarget()
	{
		return _target;
	}
	
	public string getText()
	{
		return _text;
	}
	
	public ChatType getChatType()
	{
		return _type;
	}
	
	public string FilteredText { get; set; }
	public ChatType FilteredType { get; set; }
}