using System.Text;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

public class Lang: IVoicedCommandHandler
{
	private static readonly string[] VOICED_COMMANDS =
	{
		"lang"
	};
	
	public bool useVoicedCommand(string command, Player activeChar, string @params)
	{
		if (!Config.MULTILANG_ENABLE || !Config.MULTILANG_VOICED_ALLOW)
		{
			return false;
		}
		
		if (@params == null)
		{
			StringBuilder html = new StringBuilder(100);
			foreach (string lang in Config.MULTILANG_ALLOWED)
            {
                html.Append("<button value=\"" + lang.toUpperCase() + "\" action=\"bypass -h voice .lang " + lang +
                    "\" width=60 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"><br>");
            }
			
            HtmlContent msg = HtmlContent.LoadFromFile("html/mods/Lang/LanguageSelect.htm", null);
			msg.Replace("%list%", html.ToString());
            
            activeChar.sendPacket(new NpcHtmlMessagePacket(0, 0, msg));
			return true;
		}
		
		StringTokenizer st = new StringTokenizer(@params);
		if (st.hasMoreTokens())
		{
			string lang = st.nextToken().Trim();
			if (activeChar.setLang(lang))
			{
                HtmlContent msg = HtmlContent.LoadFromFile("html/mods/Lang/Ok.htm", null);
                activeChar.sendPacket(new NpcHtmlMessagePacket(0, 0, msg));
				foreach (WorldObject obj in World.getInstance().getVisibleObjects())
				{
					if (obj.isNpc() && NpcNameLocalisationData.getInstance().hasLocalisation(obj.getId()))
					{
						activeChar.sendPacket(new DeleteObjectPacket(obj.getObjectId()));
						ThreadPool.schedule(() => activeChar.sendPacket(new NpcInfoPacket((Npc) obj)), 1000);
					}
				}
				activeChar.setTarget(null);
				return true;
			}
            
            HtmlContent msg1 = HtmlContent.LoadFromFile("html/mods/Lang/Error.htm", null);
            activeChar.sendPacket(new NpcHtmlMessagePacket(0, 0, msg1));
			return true;
		}
		return false;
	}
	
	public string[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}