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
				int x = ((activeChar.getX() - World.WORLD_X_MIN) >> 15) + World.TILE_X_MIN;
				int y = ((activeChar.getY() - World.WORLD_Y_MIN) >> 15) + World.TILE_Y_MIN;
				int topLeftX = (x - World.TILE_ZERO_COORD_X) * World.TILE_SIZE;
				int topLeftY = (y - World.TILE_ZERO_COORD_Y) * World.TILE_SIZE;
				int bottomRightX = (((x - World.TILE_ZERO_COORD_X) * World.TILE_SIZE) + World.TILE_SIZE) - 1;
				int bottomRightY = (((y - World.TILE_ZERO_COORD_Y) * World.TILE_SIZE) + World.TILE_SIZE) - 1;
				BuilderUtil.sendSysMessage(activeChar, "GeoMap: " + x + "_" + y + " (" + topLeftX + "," + topLeftY + " to " + bottomRightX + "," + bottomRightY + ")");
				List<int> results = [];
				foreach (WorldObject obj in World.getInstance().getVisibleObjects())
				{
					if (obj.isNpc() //
						&& !obj.isMonster() //
						&& !(obj.isArtefact()) //
						&& !(obj is BroadcastingTower) //
						&& !(obj is FlyTerrainObject) //
						&& !results.Contains(obj.getId()))
					{
						Npc npc = (Npc) obj;
						if ((npc.Location.X > topLeftX) && (npc.Location.X < bottomRightX) && (npc.Location.Y > topLeftY) && (npc.Location.Y < bottomRightY) && npc.isTalkable() && !npc.Events.HasSubscribers<OnNpcFirstTalk>())
						{
							if ((npc.getHtmlPath(npc.getId(), 0, null).equals("html/npcdefault.htm"))//
								|| ((obj is Fisherman) && (HtmCache.getInstance().getHtm(null, "html/fisherman/" + npc.getId() + ".htm") == null)) //
								|| ((obj is Warehouse) && (HtmCache.getInstance().getHtm(null, "html/warehouse/" + npc.getId() + ".htm") == null)) //
								|| (((obj is Merchant) && !(obj is Fisherman)) && (HtmCache.getInstance().getHtm(null, "html/merchant/" + npc.getId() + ".htm") == null)) //
								|| ((obj is Guard) && (HtmCache.getInstance().getHtm(null, "html/guard/" + npc.getId() + ".htm") == null)))
							{
								results.Add(npc.getId());
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
						&& !(obj.isArtefact()) //
						&& !(obj is BroadcastingTower) //
						&& !(obj is FlyTerrainObject) //
						&& !results.Contains(obj.getId()))
					{
						Npc npc = (Npc) obj;
						if (npc.isTalkable() && !npc.Events.HasSubscribers<OnNpcFirstTalk>())
						{
							if ((npc.getHtmlPath(npc.getId(), 0, null).equals("html/npcdefault.htm")) //
								|| ((obj is Fisherman) && (HtmCache.getInstance().getHtm(null, "html/fisherman/" + npc.getId() + ".htm") == null)) //
								|| ((obj is Warehouse) && (HtmCache.getInstance().getHtm(null, "html/warehouse/" + npc.getId() + ".htm") == null)) //
								|| (((obj is Merchant) && !(obj is Fisherman)) && (HtmCache.getInstance().getHtm(null, "html/merchant/" + npc.getId() + ".htm") == null)) //
								|| ((obj is Guard) && (HtmCache.getInstance().getHtm(null, "html/guard/" + npc.getId() + ".htm") == null)))
							{
								results.Add(npc.getId());
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
						&& !(obj.isArtefact()) //
						&& !(obj is BroadcastingTower) //
						&& !(obj is FlyTerrainObject))
					{
						Npc npc = (Npc) obj;
						if (npc.isTalkable() && !npc.Events.HasSubscribers<OnNpcFirstTalk>())
						{
							if ((npc.getHtmlPath(npc.getId(), 0, null).equals("html/npcdefault.htm")) //
								|| ((obj is Fisherman) && (HtmCache.getInstance().getHtm(null, "html/fisherman/" + npc.getId() + ".htm") == null)) //
								|| ((obj is Warehouse) && (HtmCache.getInstance().getHtm(null, "html/warehouse/" + npc.getId() + ".htm") == null)) //
								|| (((obj is Merchant) && !(obj is Fisherman)) && (HtmCache.getInstance().getHtm(null, "html/merchant/" + npc.getId() + ".htm") == null)) //
								|| ((obj is Guard) && (HtmCache.getInstance().getHtm(null, "html/guard/" + npc.getId() + ".htm") == null)))
							{
								activeChar.teleToLocation(npc.Location);
								BuilderUtil.sendSysMessage(activeChar, "NPC " + npc.getId() + " does not have a default html.");
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
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
