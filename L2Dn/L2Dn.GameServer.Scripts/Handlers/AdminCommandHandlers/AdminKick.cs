using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

public class AdminKick: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_kick",
		"admin_kick_non_gm",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.startsWith("admin_kick"))
		{
			StringTokenizer st = new StringTokenizer(command);
			if (st.countTokens() > 1)
			{
				st.nextToken();
				string player = st.nextToken();
				Player plyr = World.getInstance().getPlayer(player);
				if (plyr != null)
				{
					Disconnection.of(plyr).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
					BuilderUtil.sendSysMessage(activeChar, "You kicked " + plyr.getName() + " from the game.");
				}
			}
		}
		if (command.startsWith("admin_kick_non_gm"))
		{
			int counter = 0;
			foreach (Player player in World.getInstance().getPlayers())
			{
				if (!player.isGM())
				{
					counter++;
					Disconnection.of(player).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
				}
			}
			BuilderUtil.sendSysMessage(activeChar, "Kicked " + counter + " players.");
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
