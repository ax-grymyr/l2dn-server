using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class TerritoryStatus: IBypassHandler
{
	private static readonly string[] COMMANDS =
	{
		"TerritoryStatus"
	};
	
	public bool useBypass(String command, Player player, Creature target)
	{
		if (!target.isNpc())
		{
			return false;
		}
		
		Npc npc = (Npc) target;
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "");
		{
			if (npc.getCastle().getOwnerId() > 0)
			{
				helper = new HtmlPacketHelper(DataFileLocation.Data, "html/territorystatus.htm");
				Clan clan = ClanTable.getInstance().getClan(npc.getCastle().getOwnerId());
				helper.Replace("%clanname%", clan.getName());
				helper.Replace("%clanleadername%", clan.getLeaderName());
			}
			else
			{
				helper = new HtmlPacketHelper(DataFileLocation.Data, "html/territorynoclan.htm");
			}
		}

		helper.Replace("%castlename%", npc.getCastle().getName());
		helper.Replace("%taxpercent%", npc.getCastle().getTaxPercent(TaxType.BUY).ToString());
		helper.Replace("%objectId%", npc.getObjectId().ToString());
		
		{
			if (npc.getCastle().getResidenceId() > 6)
			{
				helper.Replace("%territory%", "The Kingdom of Elmore");
			}
			else
			{
				helper.Replace("%territory%", "The Kingdom of Aden");
			}
		}
		
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(npc.getObjectId(), helper);
		player.sendPacket(html);
		return true;
	}
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
}