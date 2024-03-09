using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class ChatLink: IBypassHandler
{
	private static readonly string[] COMMANDS =
	{
		"Chat"
	};
	
	public bool useBypass(String command, Player player, Creature target)
	{
		if (!target.isNpc())
		{
			return false;
		}
		
		int val = 0;
		try
		{
			val = int.Parse(command.Substring(5));
		}
		catch (Exception e)
		{
			// Handled above.
		}
		
		Npc npc = (Npc) target;
		if ((val == 0) && npc.hasListener(EventType.ON_NPC_FIRST_TALK))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnNpcFirstTalk(npc, player), npc);
		}
		else
		{
			npc.showChatWindow(player, val);
		}
		return false;
	}
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
}