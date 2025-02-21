using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class TerritoryStatus: IBypassHandler
{
	private static readonly string[] COMMANDS =
    [
        "TerritoryStatus",
    ];

	public bool useBypass(string command, Player player, Creature? target)
	{
		if (target == null || !target.isNpc())
			return false;

		Npc npc = (Npc)target;
        Castle? castle = npc.getCastle();
        Clan? clan = castle == null ? null : ClanTable.getInstance().getClan(castle.getOwnerId());

        HtmlContent htmlContent;
		if (castle != null && castle.getOwnerId() > 0 && clan != null)
		{
			htmlContent = HtmlContent.LoadFromFile("html/territorystatus.htm", player);
			htmlContent.Replace("%clanname%", clan.getName());
			htmlContent.Replace("%clanleadername%", clan.getLeaderName());
		}
		else
		{
			htmlContent = HtmlContent.LoadFromFile("html/territorynoclan.htm", player);
		}

		htmlContent.Replace("%castlename%", castle?.getName() ?? string.Empty);
		htmlContent.Replace("%taxpercent%", castle?.getTaxPercent(TaxType.BUY).ToString() ?? string.Empty);
		htmlContent.Replace("%objectId%", npc.ObjectId.ToString());

        htmlContent.Replace("%territory%",
            castle?.getResidenceId() > 6 ? "The Kingdom of Elmore" : "The Kingdom of Aden");

        NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(npc.ObjectId, 0, htmlContent);
		player.sendPacket(html);
		return true;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}