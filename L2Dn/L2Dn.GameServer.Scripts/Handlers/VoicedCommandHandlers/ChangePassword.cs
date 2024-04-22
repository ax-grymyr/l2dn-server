using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.NetworkAuthServer;
using L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

/**
 * @author Nik
 */
public class ChangePassword: IVoicedCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ChangePassword));
	
	private static readonly String[] VOICED_COMMANDS =
	{
		"changepassword"
	};
	
	public bool useVoicedCommand(String command, Player activeChar, String target)
	{
		if (target != null)
		{
			StringTokenizer st = new StringTokenizer(target);
			try
			{
				String curpass = null;
				String newpass = null;
				String repeatnewpass = null;
				if (st.hasMoreTokens())
				{
					curpass = st.nextToken();
				}
				if (st.hasMoreTokens())
				{
					newpass = st.nextToken();
				}
				if (st.hasMoreTokens())
				{
					repeatnewpass = st.nextToken();
				}
				
				if (!((curpass == null) || (newpass == null) || (repeatnewpass == null)))
				{
					if (!newpass.equals(repeatnewpass))
					{
						activeChar.sendMessage("The new password doesn't match with the repeated one!");
						return false;
					}
                    
					if (newpass.Length < 3)
					{
						activeChar.sendMessage("The new password is shorter than 3 chars! Please try with a longer one.");
						return false;
					}
                    
					if (newpass.Length > 30)
					{
						activeChar.sendMessage("The new password is longer than 30 chars! Please try with a shorter one.");
						return false;
					}

                    ChangePasswordPacket changePasswordPacket = new(activeChar.getAccountId(), curpass, newpass);
					AuthServerSession.Send(ref changePasswordPacket);
				}
				else
				{
					activeChar.sendMessage("Invalid password data! You have to fill all boxes.");
					return false;
				}
			}
			catch (Exception e)
			{
				activeChar.sendMessage("A problem occured while changing password!");
				LOGGER.Warn(e);
			}
		}
		else
		{
			// showHTML(activeChar);
			String html = HtmCache.getInstance().getHtm("html/mods/ChangePassword.htm");
			if (html == null)
			{
				html = "<html><body><br><br><center><font color=LEVEL>404:</font> File Not Found</center></body></html>";
			}

            HtmlContent content = HtmlContent.LoadFromText(html, null);
            activeChar.sendPacket(new NpcHtmlMessagePacket(0, 0, content));
			return true;
		}
		return true;
	}
	
	public String[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}