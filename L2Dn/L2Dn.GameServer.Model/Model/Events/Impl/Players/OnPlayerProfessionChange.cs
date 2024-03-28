using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerProfessionChange: EventBase
{
	private readonly Player _player;
	private readonly PlayerTemplate _template;
	private readonly bool _isSubClass;
	
	public OnPlayerProfessionChange(Player player, PlayerTemplate template, bool isSubClass)
	{
		_player = player;
		_template = template;
		_isSubClass = isSubClass;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public PlayerTemplate getTemplate()
	{
		return _template;
	}
	
	public bool isSubClass()
	{
		return _isSubClass;
	}
}