using System.Globalization;
using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * Handler provides ability to override server's conditions for admin.<br>
 * Note: //setparam command uses any XML value and ignores case sensitivity.<br>
 * For best results by //setparam enable the maximum stats PcCondOverride here.
 * @author UnAfraid, Nik
 */
public class AdminPcCondOverride: IAdminCommandHandler
{
	// private static int SETPARAM_ORDER = 0x90;
	
	private static readonly string[] COMMANDS =
    [
        "admin_exceptions",
		"admin_set_exception",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command);
		if (st.hasMoreTokens())
		{
			switch (st.nextToken())
			// command
			{
				case "admin_exceptions":
				{
					HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/cond_override.htm", activeChar);
					
					NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(null, 1, htmlContent);
					StringBuilder sb = new StringBuilder();
					foreach (PlayerCondOverride ex in EnumUtil.GetValues<PlayerCondOverride>())
					{
						sb.Append("<tr><td fixwidth=\"180\">" + ex + ":</td><td><a action=\"bypass -h admin_set_exception " + ex + "\">" + (activeChar.canOverrideCond(ex) ? "Disable" : "Enable") + "</a></td></tr>");
					}
					htmlContent.Replace("%cond_table%", sb.ToString());
					activeChar.sendPacket(msg);
					break;
				}
				case "admin_set_exception":
				{
					if (st.hasMoreTokens())
					{
						string token = st.nextToken();
						if (int.TryParse(token, CultureInfo.InvariantCulture, out int tokenInt))
						{
							PlayerCondOverride ex = (PlayerCondOverride)tokenInt;
							if (Enum.IsDefined(ex))
							{
								if (activeChar.canOverrideCond(ex))
								{
									activeChar.removeOverridedCond(ex);
									BuilderUtil.sendSysMessage(activeChar, "You've disabled " + ex);
								}
								else
								{
									activeChar.addOverrideCond(ex);
									BuilderUtil.sendSysMessage(activeChar, "You've enabled " + ex);
								}
							}
						}
						else
						{
							switch (token)
							{
								case "enable_all":
								{
									foreach (PlayerCondOverride ex in EnumUtil.GetValues<PlayerCondOverride>())
									{
										if (!activeChar.canOverrideCond(ex))
										{
											activeChar.addOverrideCond(ex);
										}
									}
									BuilderUtil.sendSysMessage(activeChar, "All condition exceptions have been enabled.");
									break;
								}
								case "disable_all":
								{
									foreach (PlayerCondOverride ex in EnumUtil.GetValues<PlayerCondOverride>())
									{
										if (activeChar.canOverrideCond(ex))
										{
											activeChar.removeOverridedCond(ex);
										}
									}
									BuilderUtil.sendSysMessage(activeChar, "All condition exceptions have been disabled.");
									break;
								}
							}
						}
						useAdminCommand(COMMANDS[0], activeChar);
					}
					break;
				}
			}
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return COMMANDS;
	}
}
