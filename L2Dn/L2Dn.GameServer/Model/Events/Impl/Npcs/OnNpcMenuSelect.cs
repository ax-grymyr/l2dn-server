using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

/**
 * @author St3eT
 */
public class OnNpcMenuSelect: EventBase
{
	private readonly Player _player;
	private readonly Npc _npc;
	private readonly int _ask;
	private readonly int _reply;
	
	/**
	 * @param player
	 * @param npc
	 * @param ask
	 * @param reply
	 */
	public OnNpcMenuSelect(Player player, Npc npc, int ask, int reply)
	{
		_player = player;
		_npc = npc;
		_ask = ask;
		_reply = reply;
	}
	
	public Player getTalker()
	{
		return _player;
	}
	
	public Npc getNpc()
	{
		return _npc;
	}
	
	public int getAsk()
	{
		return _ask;
	}
	
	public int getReply()
	{
		return _reply;
	}
}