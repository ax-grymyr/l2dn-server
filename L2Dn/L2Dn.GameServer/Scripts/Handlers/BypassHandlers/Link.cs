using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Handlers.BypassHandlers;

public class Link: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(Link));

	private static readonly string[] COMMANDS =
	{
		"Link"
	};
	
	private static readonly Set<String> VALID_LINKS = new();
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
	
	public bool useBypass(String command, Player player, Creature target)
	{
		String htmlPath = command.Substring(4).Trim();
		if (htmlPath.isEmpty())
		{
			_logger.Warn(player + " sent empty link html!");
			return false;
		}
		
		if (htmlPath.contains(".."))
		{
			_logger.Warn(player + " sent invalid link html: " + htmlPath);
			return false;
		}
		
		String content = VALID_LINKS.Contains(htmlPath) ? HtmCache.getInstance().getHtm(player, "html/" + htmlPath) : null;
		// Precaution.
		if (htmlPath.startsWith("teleporter/") && !(player.getTarget() is Teleporter))
		{
			content = null;
		}
		
		if (content != null)
		{
			content = content.Replace("%objectId%", (target?.getObjectId() ?? 0).ToString());
		}
		
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(target != null ? target.getObjectId() : 0, content);
		player.sendPacket(html);
		return true;
	}
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
}