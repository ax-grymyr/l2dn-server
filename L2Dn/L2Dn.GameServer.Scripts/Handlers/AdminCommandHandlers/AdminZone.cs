using System.Text;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

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
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (activeChar == null)
		{
			return false;
		}
		
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken(); // Get actual command
		
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
				BuilderUtil.sendSysMessage(activeChar, "TeleToLocation (Castle): x:" + loc.X + " y:" + loc.Y + " z:" + loc.Z);
				loc = MapRegionManager.getInstance().getTeleToLocation(activeChar, TeleportWhereType.CLANHALL);
				BuilderUtil.sendSysMessage(activeChar, "TeleToLocation (ClanHall): x:" + loc.X + " y:" + loc.Y + " z:" + loc.Z);
				loc = MapRegionManager.getInstance().getTeleToLocation(activeChar, TeleportWhereType.SIEGEFLAG);
				BuilderUtil.sendSysMessage(activeChar, "TeleToLocation (SiegeFlag): x:" + loc.X + " y:" + loc.Y + " z:" + loc.Z);
				loc = MapRegionManager.getInstance().getTeleToLocation(activeChar, TeleportWhereType.TOWN);
				BuilderUtil.sendSysMessage(activeChar, "TeleToLocation (Town): x:" + loc.X + " y:" + loc.Y + " z:" + loc.Z);
			}
		}
		else if (actualCommand.equalsIgnoreCase("admin_zone_visual"))
		{
			string next = st.nextToken();
			if (next.equalsIgnoreCase("all"))
			{
				foreach (ZoneType zone in ZoneManager.getInstance().getZones(activeChar.Location.Location3D))
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
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/zone.htm", activeChar);
		htmlContent.Replace("%PEACE%", activeChar.isInsideZone(ZoneId.PEACE) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%PVP%", activeChar.isInsideZone(ZoneId.PVP) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%SIEGE%", activeChar.isInsideZone(ZoneId.SIEGE) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%CASTLE%", activeChar.isInsideZone(ZoneId.CASTLE) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%FORT%", activeChar.isInsideZone(ZoneId.FORT) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%HQ%", activeChar.isInsideZone(ZoneId.HQ) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%CLANHALL%", activeChar.isInsideZone(ZoneId.CLAN_HALL) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%LAND%", activeChar.isInsideZone(ZoneId.LANDING) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%NOLAND%", activeChar.isInsideZone(ZoneId.NO_LANDING) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%NOSUMMON%", activeChar.isInsideZone(ZoneId.NO_SUMMON_FRIEND) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%WATER%", activeChar.isInsideZone(ZoneId.WATER) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%FISHING%", activeChar.isInsideZone(ZoneId.FISHING) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%SWAMP%", activeChar.isInsideZone(ZoneId.SWAMP) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%DANGER%", activeChar.isInsideZone(ZoneId.DANGER_AREA) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%NOSTORE%", activeChar.isInsideZone(ZoneId.NO_STORE) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%SCRIPT%", activeChar.isInsideZone(ZoneId.SCRIPT) ? "<font color=\"LEVEL\">YES</font>" : "NO");
		htmlContent.Replace("%TAX%", (activeChar.isInsideZone(ZoneId.TAX) ? "<font color=\"LEVEL\">YES</font>" : "NO"));
		
		StringBuilder zones = new StringBuilder(100);
		foreach (ZoneType zone in ZoneManager.getInstance().getZones(activeChar.Location.Location3D))
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
		
		htmlContent.Replace("%ZLIST%", zones.ToString());

		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
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
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
