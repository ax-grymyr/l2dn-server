using System.Text;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

public class Premium: IVoicedCommandHandler
{
	private static readonly string[] VOICED_COMMANDS =
	{
		"premium"
	};
	
	public bool useVoicedCommand(string command, Player activeChar, string target)
	{
		if (command.startsWith("premium") && Config.PREMIUM_SYSTEM_ENABLED)
		{
			DateTime? endDate = PremiumManager.getInstance().getPremiumExpiration(activeChar.getAccountId());
			StringBuilder html = new StringBuilder();
			if (endDate is null)
			{
				html.Append("<html><body><title>Account Details</title><center>");
				html.Append("<table>");
				html.Append("<tr><td><center>Account Status: <font color=\"LEVEL\">Normal<br></font></td></tr>");
				html.Append("<tr><td>Rate XP: <font color=\"LEVEL\"> x" + Config.RATE_XP + "<br1></font></td></tr>");
				html.Append("<tr><td>Rate SP: <font color=\"LEVEL\"> x" + Config.RATE_SP + "<br1></font></td></tr>");
				html.Append("<tr><td>Drop Chance: <font color=\"LEVEL\"> x" + Config.RATE_DEATH_DROP_CHANCE_MULTIPLIER + "<br1></font></td></tr><br>");
				html.Append("<tr><td>Drop Amount: <font color=\"LEVEL\"> x" + Config.RATE_DEATH_DROP_AMOUNT_MULTIPLIER + "<br1></font></td></tr><br>");
				html.Append("<tr><td>Spoil Chance: <font color=\"LEVEL\"> x" + Config.RATE_SPOIL_DROP_CHANCE_MULTIPLIER + "<br1></font></td></tr><br>");
				html.Append("<tr><td>Spoil Amount: <font color=\"LEVEL\"> x" + Config.RATE_SPOIL_DROP_AMOUNT_MULTIPLIER + "<br><br></font></td></tr><br>");
				html.Append("<tr><td><center>Premium Info & Rules<br></td></tr>");
				html.Append("<tr><td>Rate XP: <font color=\"LEVEL\"> x" + (Config.RATE_XP * Config.PREMIUM_RATE_XP) + "<br1></font></td></tr>");
				html.Append("<tr><td>Rate SP: <font color=\"LEVEL\"> x" + (Config.RATE_SP * Config.PREMIUM_RATE_SP) + "<br1></font></td></tr>");
				html.Append("<tr><td>Drop Chance: <font color=\"LEVEL\"> x" + (Config.RATE_DEATH_DROP_CHANCE_MULTIPLIER * Config.PREMIUM_RATE_DROP_CHANCE) + "<br1></font></td></tr>");
				html.Append("<tr><td>Drop Amount: <font color=\"LEVEL\"> x" + (Config.RATE_DEATH_DROP_AMOUNT_MULTIPLIER * Config.PREMIUM_RATE_DROP_AMOUNT) + "<br1></font></td></tr>");
				html.Append("<tr><td>Spoil Chance: <font color=\"LEVEL\"> x" + (Config.RATE_SPOIL_DROP_CHANCE_MULTIPLIER * Config.PREMIUM_RATE_SPOIL_CHANCE) + "<br1></font></td></tr>");
				html.Append("<tr><td>Spoil Amount: <font color=\"LEVEL\"> x" + (Config.RATE_SPOIL_DROP_AMOUNT_MULTIPLIER * Config.PREMIUM_RATE_SPOIL_AMOUNT) + "<br1></font></td></tr>");
				html.Append("<tr><td> <font color=\"70FFCA\">1. Premium benefits CAN NOT BE TRANSFERED.<br1></font></td></tr>");
				html.Append("<tr><td> <font color=\"70FFCA\">2. Premium does not effect party members.<br1></font></td></tr>");
				html.Append("<tr><td> <font color=\"70FFCA\">3. Premium benefits effect ALL characters in same account.</font></td></tr>");
			}
			else
			{
				html.Append("<html><body><title>Premium Account Details</title><center>");
				html.Append("<table>");
				html.Append("<tr><td><center>Account Status: <font color=\"LEVEL\">Premium<br></font></td></tr>");
				html.Append("<tr><td>Rate XP: <font color=\"LEVEL\">x" + (Config.RATE_XP * Config.PREMIUM_RATE_XP) + " <br1></font></td></tr>");
				html.Append("<tr><td>Rate SP: <font color=\"LEVEL\">x" + (Config.RATE_SP * Config.PREMIUM_RATE_SP) + "  <br1></font></td></tr>");
				html.Append("<tr><td>Drop Chance: <font color=\"LEVEL\">x" + (Config.RATE_DEATH_DROP_CHANCE_MULTIPLIER * Config.PREMIUM_RATE_DROP_CHANCE) + " <br1></font></td></tr>");
				html.Append("<tr><td>Drop Amount: <font color=\"LEVEL\">x" + (Config.RATE_DEATH_DROP_AMOUNT_MULTIPLIER * Config.PREMIUM_RATE_DROP_AMOUNT) + " <br1></font></td></tr>");
				html.Append("<tr><td>Spoil Chance: <font color=\"LEVEL\">x" + (Config.RATE_SPOIL_DROP_CHANCE_MULTIPLIER * Config.PREMIUM_RATE_SPOIL_CHANCE) + " <br1></font></td></tr>");
				html.Append("<tr><td>Spoil Amount: <font color=\"LEVEL\">x" + (Config.RATE_SPOIL_DROP_AMOUNT_MULTIPLIER * Config.PREMIUM_RATE_SPOIL_AMOUNT) + " <br1></font></td></tr>");
				html.Append("<tr><td>Expires: <font color=\"00A5FF\">" + endDate.Value.ToString("dd.MM.yyyy HH:mm") + "</font></td></tr>");
				html.Append("<tr><td>Current Date: <font color=\"70FFCA\">" + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "<br><br></font></td></tr>");
				html.Append("<tr><td><center>Premium Info & Rules<br></center></td></tr>");
				html.Append("<tr><td><font color=\"70FFCA\">1. Premium accounts CAN NOT BE TRANSFERED.<br1></font></td></tr>");
				html.Append("<tr><td><font color=\"70FFCA\">2. Premium does not effect party members.<br1></font></td></tr>");
				html.Append("<tr><td><font color=\"70FFCA\">3. Premium account effects ALL characters in same account.<br><br><br></font></td></tr>");
				html.Append("<tr><td><center>Thank you for supporting our server.</td></tr>");
			}
			html.Append("</table>");
			html.Append("</center></body></html>");
            
            HtmlContent content = HtmlContent.LoadFromText(html.ToString(), null);
            activeChar.sendPacket(new NpcHtmlMessagePacket(5, 0, content));
        }
		else
		{
			return false;
		}

        return true;
	}
	
	public string[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}