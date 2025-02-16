using System.Globalization;
using System.Text;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.CommunityBoard;

/**
 * Favorite board.
 * @author Zoey76
 */
public class FavoriteBoard: IParseBoardHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(FavoriteBoard));
	
	private static readonly string[] COMMANDS =
	{
		"_bbsgetfav",
		"bbs_add_fav",
		"_bbsdelfav_"
	};
	
	public string[] getCommunityBoardCommands()
	{
		return COMMANDS;
	}
	
	public bool parseCommunityBoardCommand(string command, Player player)
	{
		// None of this commands can be added to favorites.
		if (command.startsWith("_bbsgetfav"))
		{
			// Load Favorite links
			string list = HtmCache.getInstance().getHtm("html/CommunityBoard/favorite_list.html", player.getLang());
			StringBuilder sb = new StringBuilder();
			try
            {
                int playerId = player.ObjectId; 
                using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                IOrderedQueryable<DbBbsFavorite> query = ctx.BbsFavorites
                    .Where(r => r.PlayerId == playerId)
                    .OrderByDescending(r => r.Created);
                
				foreach (DbBbsFavorite record in query)
				{
					string link = list.Replace("%fav_bypass%", record.ByPass);
					link = link.Replace("%fav_title%", record.Title);
					link = link.Replace("%fav_add_date%", record.Created.ToString("yyyy-MM-dd HH:mm:ss"));
					link = link.Replace("%fav_id%", record.Id.ToString());
					sb.Append(link);
				}

                string html = HtmCache.getInstance().getHtm("html/CommunityBoard/favorite.html", player.getLang());
				html = html.Replace("%fav_list%", sb.ToString());
				CommunityBoardHandler.separateAndSend(html, player);
			}
			catch (Exception e)
			{
                _logger.Warn(nameof(FavoriteBoard) + ": Couldn't load favorite links for " + player);
			}
		}
		else if (command.startsWith("bbs_add_fav"))
		{
			string bypass = CommunityBoardHandler.getInstance().removeBypass(player);
			if (bypass != null)
			{
				string[] parts = bypass.Split("&", 2);
				if (parts.Length != 2)
				{
					_logger.Warn(nameof(FavoriteBoard) + ": Couldn't add favorite link, " + bypass + " it's not a valid bypass!");
					return false;
				}
				
				try 
				{
                    using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                    ctx.BbsFavorites.Add(new DbBbsFavorite()
                    {
                        Created = DateTime.UtcNow,
                        Title = parts[0].Trim(),
                        ByPass = parts[1].Trim(),
                        PlayerId = player.ObjectId,
                    });

                    ctx.SaveChanges();
                    
                    // Callback
					parseCommunityBoardCommand("_bbsgetfav", player);
				}
				catch (Exception e)
				{
                    _logger.Warn(nameof(FavoriteBoard) + ": Couldn't add favorite link " + bypass + " for " + player);
				}
			}
		}
		else if (command.startsWith("_bbsdelfav_"))
		{
			string favId = command.Replace("_bbsdelfav_", "");
			if (!int.TryParse(favId, CultureInfo.InvariantCulture, out int id))
			{
                _logger.Warn(nameof(FavoriteBoard) + ": Couldn't delete favorite link, " + favId + " it's not a valid ID!");
				return false;
			}
			
			try
            {
                int playerId = player.ObjectId;
                using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                ctx.BbsFavorites.Where(r => r.Id == id && r.PlayerId == playerId).ExecuteDelete();

                // Callback
				parseCommunityBoardCommand("_bbsgetfav", player);
			}
			catch (Exception e)
			{
                _logger.Warn(nameof(FavoriteBoard) + ": Couldn't delete favorite link ID " + favId + " for " + player);
			}
		}
		return true;
	}
}
