using L2Dn.Events;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerFishing: EventBase
{
	private readonly Player _player;
	private readonly FishingEndReason _reason;
	
	public OnPlayerFishing(Player player, FishingEndReason reason)
	{
		_player = player;
		_reason = reason;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public FishingEndReason getReason()
	{
		return _reason;
	}
}