using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Npcs;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class ChatLink: IBypassHandler
{
	private static readonly string[] COMMANDS =
    [
        "Chat",
    ];

	public bool useBypass(string command, Player player, Creature? target)
	{
		if (target == null || !target.isNpc())
		{
			return false;
		}

		int val = 0;
		try
		{
			val = int.Parse(command.Substring(5));
		}
		catch (Exception)
		{
            // TODO: we need to respond to the player that the command is invalid and with the suggested command.
			// Handled above.
		}

		Npc npc = (Npc) target;
		if (val == 0 && npc.Events.HasSubscribers<OnNpcFirstTalk>())
		{
			npc.Events.NotifyAsync(new OnNpcFirstTalk(npc, player));
		}
		else
		{
			npc.showChatWindow(player, val);
		}
		return false;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}