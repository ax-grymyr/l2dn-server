using System.Globalization;
using System.Text;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.CommunityBoard;

/**
 * Region board.
 * @author Zoey76
 */
public class RegionBoard: IWriteBoardHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(RegionBoard));

	// Region data
	// @formatter:off
	private static readonly int[] REGIONS = [ 1049, 1052, 1053, 1057, 1060, 1059, 1248, 1247, 1056 ];
	// @formatter:on
	private static readonly  string[] COMMANDS =
    [
        "_bbsloc",
    ];

	public string[] getCommunityBoardCommands()
	{
		return COMMANDS;
	}

	public bool parseCommunityBoardCommand(string command, Player player)
	{
		if (command.equals("_bbsloc"))
		{
			CommunityBoardHandler.getInstance().addBypass(player, "Region", command);

			string list = HtmCache.getInstance().getHtm("html/CommunityBoard/region_list.html", player.getLang());
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < REGIONS.Length; i++)
			{
				Castle? castle = CastleManager.getInstance().getCastleById(i + 1);
				Clan? clan = castle != null ? ClanTable.getInstance().getClan(castle.getOwnerId()) : null;
                int taxPercent = castle?.getTaxPercent(TaxType.BUY) ?? 0;

				string link = list.Replace("%region_id%", i.ToString());
				link = link.Replace("%region_name%", REGIONS[i].ToString());
				link = link.Replace("%region_owning_clan%", clan != null ? clan.getName() : "NPC");
				link = link.Replace("%region_owning_clan_alliance%", clan != null && clan.getAllyName() != null ? clan.getAllyName() : "");
				link = link.Replace("%region_tax_rate%", taxPercent + "%");
				sb.Append(link);
			}

			string html = HtmCache.getInstance().getHtm("html/CommunityBoard/region.html", player.getLang());
			html = html.Replace("%region_list%", sb.ToString());
			CommunityBoardHandler.separateAndSend(html, player);
		}
		else if (command.startsWith("_bbsloc;"))
		{
			CommunityBoardHandler.getInstance().addBypass(player, "Region>", command);

			string id = command.Replace("_bbsloc;", "");
			if (!int.TryParse(id, CultureInfo.InvariantCulture, out int value))
			{
				_logger.Warn(nameof(RegionBoard) + ": " + player + " sent and invalid region bypass " + command + "!");
				return false;
			}

			// TODO: Implement.
		}
		return true;
	}

	public bool writeCommunityBoardCommand(Player player, string arg1, string arg2, string arg3, string arg4, string arg5)
	{
		// TODO: Implement.
		return false;
	}
}