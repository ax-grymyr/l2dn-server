using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author NosBit
 */
public class AdminHtml: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_html",
		"admin_loadhtml"
	};
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken();
		switch (actualCommand.toLowerCase())
		{
			case "admin_html":
			{
				if (!st.hasMoreTokens())
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //html path");
					return false;
				}
				
				string path = st.nextToken();
				showAdminHtml(activeChar, path);
				break;
			}
			case "admin_loadhtml":
			{
				if (!st.hasMoreTokens())
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //loadhtml path");
					return false;
				}
				
				string path = st.nextToken();
				showHtml(activeChar, path, true);
				break;
			}
		}
		return true;
	}
	
	/**
	 * Shows a html message to activeChar
	 * @param activeChar activeChar where html is shown
	 * @param path relative path from directory data/html/admin/ to html
	 */
	public static void showAdminHtml(Player activeChar, string path)
	{
		showHtml(activeChar, "html/admin/" + path, false);
	}
	
	/**
	 * Shows a html message to activeChar.
	 * @param activeChar activeChar where html message is shown.
	 * @param path relative path from Config.DATAPACK_ROOT to html.
	 * @param reload {@code true} will reload html and show it {@code false} will show it from cache.
	 */
	private static void showHtml(Player activeChar, string path, bool reload)
	{
		string? content;
		if (!reload)
		{
			content = HtmCache.getInstance().getHtm(path, activeChar.getLang());
		}
		else
		{
			content = HtmCache.getInstance().loadFile(Path.Combine(Config.DATAPACK_ROOT_PATH, path));
		}

		HtmlContent htmlContent =
			HtmlContent.LoadFromText(content ?? "<html><body>My text is missing:<br>" + path + "</body></html>",
				activeChar);
		
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);

		activeChar.sendPacket(html);
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
