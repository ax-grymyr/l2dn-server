using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Mobius
 */
public class AdminMissingHtmls: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_geomap_missing_htmls",
		"admin_world_missing_htmls",
		"admin_next_missing_html",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken();
		switch (actualCommand.toLowerCase())
		{
			case "admin_geomap_missing_htmls":
			{
				int x = ((activeChar.getX() - WorldMap.WorldXMin) >> 15) + WorldMap.TileXMin;
				int y = ((activeChar.getY() - WorldMap.WorldYMin) >> 15) + WorldMap.TileYMin;
				int topLeftX = (x - WorldMap.TileZeroCoordX) * WorldMap.TileSize;
				int topLeftY = (y - WorldMap.TileZeroCoordY) * WorldMap.TileSize;
				int bottomRightX = (x - WorldMap.TileZeroCoordX) * WorldMap.TileSize + WorldMap.TileSize - 1;
				int bottomRightY = (y - WorldMap.TileZeroCoordY) * WorldMap.TileSize + WorldMap.TileSize - 1;
				BuilderUtil.sendSysMessage(activeChar, "GeoMap: " + x + "_" + y + " (" + topLeftX + "," + topLeftY + " to " + bottomRightX + "," + bottomRightY + ")");
				List<int> results = [];
				foreach (WorldObject obj in World.getInstance().getVisibleObjects())
				{
					if (obj.isNpc() //
						&& !obj.isMonster() //
						&& !obj.isArtefact() //
						&& !(obj is BroadcastingTower) //
						&& !(obj is FlyTerrainObject) //
						&& !results.Contains(obj.Id))
					{
						Npc npc = (Npc) obj;
						if (npc.Location.X > topLeftX && npc.Location.X < bottomRightX && npc.Location.Y > topLeftY && npc.Location.Y < bottomRightY && npc.isTalkable() && !npc.Events.HasSubscribers<OnNpcFirstTalk>())
						{
							if (npc.getHtmlPath(npc.Id, 0, null).equals("html/npcdefault.htm")//
								|| (obj is Fisherman && HtmlMissing("html/fisherman/" + npc.Id + ".htm")) //
								|| (obj is Warehouse && HtmlMissing("html/warehouse/" + npc.Id + ".htm")) //
								|| (obj is Merchant && !(obj is Fisherman) && HtmlMissing("html/merchant/" + npc.Id + ".htm")) //
								|| (obj is Guard && HtmlMissing("html/guard/" + npc.Id + ".htm")))
							{
								results.Add(npc.Id);
							}
						}
					}
				}

				results.Sort();
				foreach (int id in results)
				{
					BuilderUtil.sendSysMessage(activeChar, "NPC " + id + " does not have a default html.");
				}
				BuilderUtil.sendSysMessage(activeChar, "Found " + results.Count + " results.");
				break;
			}
			case "admin_world_missing_htmls":
			{
				BuilderUtil.sendSysMessage(activeChar, "Missing htmls for the whole world.");
				List<int> results = [];
				foreach (WorldObject obj in World.getInstance().getVisibleObjects())
				{
					if (obj.isNpc() //
						&& !obj.isMonster() //
						&& !obj.isArtefact() //
						&& !(obj is BroadcastingTower) //
						&& !(obj is FlyTerrainObject) //
						&& !results.Contains(obj.Id))
					{
						Npc npc = (Npc) obj;
						if (npc.isTalkable() && !npc.Events.HasSubscribers<OnNpcFirstTalk>())
						{
							if (npc.getHtmlPath(npc.Id, 0, null).equals("html/npcdefault.htm") //
								|| (obj is Fisherman && HtmlMissing("html/fisherman/" + npc.Id + ".htm")) //
								|| (obj is Warehouse && HtmlMissing("html/warehouse/" + npc.Id + ".htm")) //
								|| (obj is Merchant && !(obj is Fisherman) && HtmlMissing("html/merchant/" + npc.Id + ".htm")) //
								|| (obj is Guard && HtmlMissing("html/guard/" + npc.Id + ".htm")))
							{
								results.Add(npc.Id);
							}
						}
					}
				}

				results.Sort();
				foreach (int id in results)
				{
					BuilderUtil.sendSysMessage(activeChar, "NPC " + id + " does not have a default html.");
				}
				BuilderUtil.sendSysMessage(activeChar, "Found " + results.Count + " results.");
				break;
			}
			case "admin_next_missing_html":
			{
				foreach (WorldObject obj in World.getInstance().getVisibleObjects())
				{
					if (obj.isNpc() //
						&& !obj.isMonster() //
						&& !obj.isArtefact() //
						&& !(obj is BroadcastingTower) //
						&& !(obj is FlyTerrainObject))
					{
						Npc npc = (Npc) obj;
						if (npc.isTalkable() && !npc.Events.HasSubscribers<OnNpcFirstTalk>())
						{
							if (npc.getHtmlPath(npc.Id, 0, null).equals("html/npcdefault.htm") //
								|| (obj is Fisherman && HtmlMissing("html/fisherman/" + npc.Id + ".htm")) //
								|| (obj is Warehouse && HtmlMissing("html/warehouse/" + npc.Id + ".htm")) //
								|| (obj is Merchant && !(obj is Fisherman) && HtmlMissing( "html/merchant/" + npc.Id + ".htm")) //
								|| (obj is Guard && HtmlMissing("html/guard/" + npc.Id + ".htm")))
							{
								activeChar.teleToLocation(npc.Location);
								BuilderUtil.sendSysMessage(activeChar, "NPC " + npc.Id + " does not have a default html.");
								break;
							}
						}
					}
				}
				break;
			}
		}
		return true;
	}

    private static bool HtmlMissing(string path)
    {
        try
        {
            HtmCache.getInstance().getHtm(path);
            return false;
        }
        catch (Exception)
        {
            return true;
        }
    }

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}