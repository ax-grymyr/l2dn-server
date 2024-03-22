using System.Text;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * Small typo fix by Zoey76 24/02/2011
 */
public class AdminZone: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_zone_check",
		"admin_zone_visual",
		"admin_zone_visual_clear"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if (activeChar == null)
		{
			return false;
		}
		
		StringTokenizer st = new StringTokenizer(command, " ");
		String actualCommand = st.nextToken(); // Get actual command
		
		// String val = "";
		// if (st.countTokens() >= 1) {val = st.nextToken();}
		if (actualCommand.equalsIgnoreCase("admin_zone_check"))
		{
			showHtml(activeChar);
			BuilderUtil.sendSysMessage(activeChar, "MapRegion: x:" + MapRegionManager.getInstance().getMapRegionX(activeChar.getX()) + " y:" + MapRegionManager.getInstance().getMapRegionY(activeChar.getY()) + " (" + MapRegionManager.getInstance().getMapRegionLocId(activeChar) + ")");
			getGeoRegionXY(activeChar);
			BuilderUtil.sendSysMessage(activeChar, "Closest Town: " + MapRegionManager.getInstance().getClosestTownName(activeChar));
			
			// Prevent exit instance variable deletion.
			if (!activeChar.isInInstance())
			{
				Location loc;
				loc = MapRegionManager.getInstance().getTeleToLocation(activeChar, TeleportWhereType.CASTLE);
				BuilderUtil.sendSysMessage(activeChar, "TeleToLocation (Castle): x:" + loc.getX() + " y:" + loc.getY() + " z:" + loc.getZ());
				loc = MapRegionManager.getInstance().getTeleToLocation(activeChar, TeleportWhereType.CLANHALL);
				BuilderUtil.sendSysMessage(activeChar, "TeleToLocation (ClanHall): x:" + loc.getX() + " y:" + loc.getY() + " z:" + loc.getZ());
				loc = MapRegionManager.getInstance().getTeleToLocation(activeChar, TeleportWhereType.SIEGEFLAG);
				BuilderUtil.sendSysMessage(activeChar, "TeleToLocation (SiegeFlag): x:" + loc.getX() + " y:" + loc.getY() + " z:" + loc.getZ());
				loc = MapRegionManager.getInstance().getTeleToLocation(activeChar, TeleportWhereType.TOWN);
				BuilderUtil.sendSysMessage(activeChar, "TeleToLocation (Town): x:" + loc.getX() + " y:" + loc.getY() + " z:" + loc.getZ());
			}
		}
		else if (actualCommand.equalsIgnoreCase("admin_zone_visual"))
		{
			String next = st.nextToken();
			if (next.equalsIgnoreCase("all"))
			{
				foreach (ZoneType zone in ZoneManager.getInstance().getZones(activeChar))
				{
					zone.visualizeZone(activeChar.getZ());
				}
				foreach (SpawnTerritory territory in ZoneManager.getInstance().getSpawnTerritories(activeChar))
				{
					territory.visualizeZone(activeChar.getZ());
				}
				showHtml(activeChar);
			}
			else
			{
				int zoneId = int.Parse(next);
				ZoneManager.getInstance().getZoneById(zoneId).visualizeZone(activeChar.getZ());
			}
		}
		else if (actualCommand.equalsIgnoreCase("admin_zone_visual_clear"))
		{
			ZoneManager.getInstance().clearDebugItems();
			showHtml(activeChar);
		}
		return true;
	}
	
	private void showHtml(Player activeChar)
	{
		String htmContent = HtmCache.getInstance().getHtm(activeChar, "data/html/admin/zone.htm");
		HtmlPacketHelper helper = new HtmlPacketHelper(htmContent);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(0, 1, helper);
		helper.Replace("%PEACE%", activeChar.isInsideZone(ZoneId.PEACE) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%PVP%", activeChar.isInsideZone(ZoneId.PVP) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%SIEGE%", activeChar.isInsideZone(ZoneId.SIEGE) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%CASTLE%", activeChar.isInsideZone(ZoneId.CASTLE) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%FORT%", activeChar.isInsideZone(ZoneId.FORT) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%HQ%", activeChar.isInsideZone(ZoneId.HQ) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%CLANHALL%", activeChar.isInsideZone(ZoneId.CLAN_HALL) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%LAND%", activeChar.isInsideZone(ZoneId.LANDING) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%NOLAND%", activeChar.isInsideZone(ZoneId.NO_LANDING) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%NOSUMMON%", activeChar.isInsideZone(ZoneId.NO_SUMMON_FRIEND) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%WATER%", activeChar.isInsideZone(ZoneId.WATER) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%FISHING%", activeChar.isInsideZone(ZoneId.FISHING) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%SWAMP%", activeChar.isInsideZone(ZoneId.SWAMP) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%DANGER%", activeChar.isInsideZone(ZoneId.DANGER_AREA) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%NOSTORE%", activeChar.isInsideZone(ZoneId.NO_STORE) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%SCRIPT%", activeChar.isInsideZone(ZoneId.SCRIPT) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		helper.Replace("%TAX%", (activeChar.isInsideZone(ZoneId.TAX) ? "<font color=\"LEVEL\">YES</font>" : "NO"));
		
		StringBuilder zones = new StringBuilder(100);
		foreach (ZoneType zone in ZoneManager.getInstance().getZones(activeChar))
		{
			if (zone.getName() != null)
			{
				zones.Append(zone.getName());
				if (zone.getId() < 300000)
				{
					zones.Append(" (");
					zones.Append(zone.getId());
					zones.Append(")");
				}
				zones.Append("<br1>");
			}
			else
			{
				zones.Append(zone.getId());
			}
			zones.Append(" ");
		}
		foreach (SpawnTerritory territory in ZoneManager.getInstance().getSpawnTerritories(activeChar))
		{
			zones.Append(territory.getName());
			zones.Append("<br1>");
		}
		helper.Replace("%ZLIST%", zones.ToString());
		activeChar.sendPacket(adminReply);
	}
	
	private void getGeoRegionXY(Player activeChar)
	{
		int worldX = activeChar.getX();
		int worldY = activeChar.getY();
		int geoX = (((worldX - -327680) >> 4) >> 11) + 10;
		int geoY = (((worldY - -262144) >> 4) >> 11) + 10;
		BuilderUtil.sendSysMessage(activeChar, "GeoRegion: " + geoX + "_" + geoY + "");
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
