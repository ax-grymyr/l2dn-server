using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class Link: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(Link));

	private static readonly string[] COMMANDS =
	{
		"Link"
	};
	
	private static readonly Set<string> VALID_LINKS = new();
	static Link()
	{
		VALID_LINKS.add("common/craft_01.htm");
		VALID_LINKS.add("common/craft_02.htm");
		VALID_LINKS.add("common/runes_01.htm");
		VALID_LINKS.add("common/skill_enchant_help_01.htm");
		VALID_LINKS.add("common/skill_enchant_help_02.htm");
		VALID_LINKS.add("common/skill_enchant_help_03.htm");
		VALID_LINKS.add("default/BlessingOfProtection.htm");
		VALID_LINKS.add("default/SupportMagic.htm");
		VALID_LINKS.add("fisherman/fishing_manual001.htm");
		VALID_LINKS.add("fisherman/fishing_manual002.htm");
		VALID_LINKS.add("fisherman/fishing_manual003.htm");
		VALID_LINKS.add("fisherman/fishing_manual004.htm");
		VALID_LINKS.add("fisherman/fishing_manual008.htm");
		VALID_LINKS.add("fortress/foreman.htm");
		VALID_LINKS.add("petmanager/evolve.htm");
		VALID_LINKS.add("petmanager/exchange.htm");
		VALID_LINKS.add("petmanager/instructions.htm");
		VALID_LINKS.add("warehouse/clanwh.htm");
		VALID_LINKS.add("warehouse/privatewh.htm");
	}
	
	public bool useBypass(string command, Player player, Creature target)
	{
		string htmlPath = command.Substring(4).Trim();
		if (string.IsNullOrEmpty(htmlPath))
		{
			_logger.Warn(player + " sent empty link html!");
			return false;
		}
		
		if (htmlPath.contains(".."))
		{
			_logger.Warn(player + " sent invalid link html: " + htmlPath);
			return false;
		}

		if (VALID_LINKS.Contains(htmlPath) && (!htmlPath.startsWith("teleporter/") || player.getTarget() is Teleporter))
		{
			HtmlContent htmlContent = HtmlContent.LoadFromFile("html/" + htmlPath, player);
			htmlContent.Replace("%objectId%", (target?.ObjectId ?? 0).ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(target?.ObjectId, 0, htmlContent);
			player.sendPacket(html);
		}

		return true;
	}
	
	public string[] getBypassList()
	{
		return COMMANDS;
	}
}