using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

using TeleportLocation = Teleporters.TeleportLocation;

/**
 * Player teleport request listner - called from {@link TeleportHolder#doTeleport(Player, Npc, int)}
 * @author malyelfik
 */
public class OnNpcTeleportRequest: TerminateEventBase
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
}