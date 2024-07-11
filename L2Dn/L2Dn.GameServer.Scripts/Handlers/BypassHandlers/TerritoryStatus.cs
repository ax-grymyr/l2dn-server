using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class TerritoryStatus: IBypassHandler
{
	private static readonly string[] COMMANDS =
	{
		"TerritoryStatus"
	};
	
	public bool useBypass(string command, Player player, Creature target)
	{
		if (!target.isNpc())
		{
			return false;
		}
		
		Npc npc = (Npc) target;
		HtmlContent htmlContent;
		{
			if (npc.getCastle().getOwnerId() > 0)
			{
				htmlContent = HtmlContent.LoadFromFile("html/territorystatus.htm", player);
				Clan clan = ClanTable.getInstance().getClan(npc.getCastle().getOwnerId());
				htmlContent.Replace("%clanname%", clan.getName());
				htmlContent.Replace("%clanleadername%", clan.getLeaderName());
			}
			else
			{
				htmlContent = HtmlContent.LoadFromFile("html/territorynoclan.htm", player);
			}
		}

		htmlContent.Replace("%castlename%", npc.getCastle().getName());
		htmlContent.Replace("%taxpercent%", npc.getCastle().getTaxPercent(TaxType.BUY).ToString());
		htmlContent.Replace("%objectId%", npc.getObjectId().ToString());
		
		{
			if (npc.getCastle().getResidenceId() > 6)
			{
				htmlContent.Replace("%territory%", "The Kingdom of Elmore");
			}
			else
			{
				htmlContent.Replace("%territory%", "The Kingdom of Aden");
			}
		}
		
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(npc.getObjectId(), 0, htmlContent);
		player.sendPacket(html);
		return true;
	}
	
	public string[] getBypassList()
	{
		return COMMANDS;
	}
}