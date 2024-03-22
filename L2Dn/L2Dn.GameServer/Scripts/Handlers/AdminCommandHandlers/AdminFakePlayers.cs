using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Mobius
 */
public class AdminFakePlayers: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_fakechat"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if (command.startsWith("admin_fakechat"))
		{
			String[] words = command.Substring(15).Split(" ");
			if (words.Length < 3)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //fakechat playername fpcname message");
				return false;
			}
			Player player = World.getInstance().getPlayer(words[0]);
			if (player == null)
			{
				BuilderUtil.sendSysMessage(activeChar, "Player not found.");
				return false;
			}
			String fpcName = FakePlayerData.getInstance().getProperName(words[1]);
			if (fpcName == null)
			{
				BuilderUtil.sendSysMessage(activeChar, "Fake player not found.");
				return false;
			}
			String message = "";
			for (int i = 0; i < words.Length; i++)
			{
				if (i < 2)
				{
					continue;
				}
				message += (words[i] + " ");
			}
			
			FakePlayerChatManager.getInstance().sendChat(player, fpcName, message);
			BuilderUtil.sendSysMessage(activeChar, "Your message has been sent.");
		}
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
