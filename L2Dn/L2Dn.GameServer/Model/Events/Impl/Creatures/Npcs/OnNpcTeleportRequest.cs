using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * Player teleport request listner - called from {@link TeleportHolder#doTeleport(Player, Npc, int)}
 * @author malyelfik
 */
public class OnNpcTeleportRequest: IBaseEvent
{
	private readonly Player _player;
	private readonly Npc _npc;
	private readonly TeleportLocation _loc;
	
	public OnNpcTeleportRequest(Player player, Npc npc, TeleportLocation loc)
	{
		_player = player;
		_npc = npc;
		_loc = loc;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public Npc getNpc()
	{
		return _npc;
	}
	
	public TeleportLocation getLocation()
	{
		return _loc;
	}
	
	public EventType getType()
	{
		return EventType.ON_NPC_TELEPORT_REQUEST;
	}
}